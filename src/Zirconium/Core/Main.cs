using System;
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
        }
    }
}
