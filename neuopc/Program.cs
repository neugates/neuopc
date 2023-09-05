using System;
using System.Windows.Forms;
using Serilog;

namespace neuopc
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            var _ = NeuSinkChannel.GetChannel();
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.File("log/neuopc.log", rollingInterval: RollingInterval.Day)
                    .WriteTo.NeuSink()
                    .CreateLogger();

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
            }

            Log.CloseAndFlush();
            NeuSinkChannel.CloseChannel();
        }
    }
}
