using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log/neuopc.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("neuopc start...");

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Log.Error($"Exceptions not handled properly, error:{ex.Message}");
            }

            Log.CloseAndFlush();
        }
    }
}
