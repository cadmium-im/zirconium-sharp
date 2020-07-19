using System;
using System.Threading;
using CommandLine;
using Nett;
using Zirconium.Core.Logging;

namespace Zirconium.Core
{
    public class RunOptions
    {
        [Option('c', "config", Required = true, HelpText = "Set config path")]
        public string ConfigPath { get; set; }
    }
    class Program
    {
        private static EventWaitHandle wailtHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        static void Main(string[] args)
        {
            string configPath = null;
            Parser.Default.ParseArguments<RunOptions>(args)
                   .WithParsed<RunOptions>(o =>
                   {
                       configPath = o.ConfigPath;
                   })
                   .WithNotParsed(errs =>
                   {
                       foreach (var err in errs)
                       {
                           Log.Error($"Error occured when parsing run options - {err.Tag}");
                       }
                       Environment.Exit(1);
                   });
            Config config = null;
            try
            {
                config = Toml.ReadFile<Config>(configPath);
            }
            catch (Exception e)
            {
                Log.Fatal($"Error occured when parsing config - {e.Message}");
            }
            App app = new App(config);
            app.Run();
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                app.Destroy();
                wailtHandle.Set();
            };
            wailtHandle.WaitOne();
        }
    }
}
