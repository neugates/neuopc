using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System.IO;
using System.Threading.Channels;

namespace neuopc
{
    public class NeuSinkChannel
    {
        private static Channel<string> _channel = null;

        public static Channel<string> GetChannel()
        {
            if (null == _channel)
            {
                _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(100)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                });
            }

            return _channel;
        }

        public static void CloseChannel()
        {
            if (null != _channel)
            {
                _channel.Writer.TryComplete();
                _channel = null;
            }
        }

        public static void Write(string message)
        {
            _channel?.Writer.TryWrite(message);
        }


        public static bool Read(out string message)
        {
            if (null != _channel)
            {
                if (_channel.Reader.TryRead(out message))
                {
                    return true;
                }
            }

            message = string.Empty;
            return false;
        }
    }

    public class NeuSink : ILogEventSink
    {
        private readonly ITextFormatter _textFormatter;

        public NeuSink(IFormatProvider formatProvider)
        {
            _textFormatter = new MessageTemplateTextFormatter("--->{Timestamp} [{Level}] {Message:lj}{NewLine}{Exception}", formatProvider);
        }

        public void Emit(LogEvent logEvent)
        {

            var stringWriter = new StringWriter();
            _textFormatter.Format(logEvent, stringWriter);
            var message = stringWriter.ToString();
            NeuSinkChannel.Write(message);
        }
    }

    public static class NeuSinkExtensions
    {
        public static LoggerConfiguration NeuSink(
                             this LoggerSinkConfiguration loggerConfiguration,
                                              IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new NeuSink(formatProvider), LogEventLevel.Verbose);
        }
    }
}
