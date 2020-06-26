using System.Diagnostics;
using System;
using System.Drawing;
using Colorful;

namespace Zirconium.Core.Logging
{
    public class Logger
    {
        private bool _isVerbose;
        private bool _isDefaultLogger;
        internal bool isInitialized;

        internal Logger(string name, bool verbose, bool systemLog, bool isDefaultLogger) {
            this._isVerbose = verbose;
            this._isDefaultLogger = isDefaultLogger;
        }

        public void Close() {}

        public void Error(string message) {
            writeLog(LogType.Error, message);
        }

        public void Warning(string message) {
            if (_isVerbose) {
                writeLog(LogType.Warning, message);
            }
        }

        public void Info(string message) {
            if (_isVerbose) {
                writeLog(LogType.Info, message);
            }
        }

        public void Fatal(string message) {
            writeLog(LogType.Fatal, message);
            System.Environment.Exit(1);
        }

        private void writeLog(LogType logType, string message) {
            var st = new StackTrace();
            StackFrame stFrame;
            if (_isDefaultLogger) {
                stFrame = st.GetFrame(3);
            } else {
                stFrame = st.GetFrame(2);
            }
            var dateTimeFormatter = new Formatter($"{DateTime.Now}", Color.White);
            var frameFormatter = new Formatter($"{stFrame.GetMethod().ReflectedType.Name}", Color.DeepPink);

            string outputMessage = "[{0}] [{1}] | [{2}]: " + message;

            Formatter tagFormatter = null;
            switch (logType) {
                case LogType.Error: {
                    tagFormatter = new Formatter("ERROR", Color.Red);
                    break;
                }
                case LogType.Fatal: {
                    tagFormatter = new Formatter("FATAL", Color.DarkRed);
                    break;
                }
                case LogType.Info: {
                    tagFormatter = new Formatter("INFO", Color.Aqua);
                    break;
                }
                case LogType.Warning: {
                    tagFormatter = new Formatter("WARNING", Color.Yellow);
                    break;
                }
                default: {
                    tagFormatter = new Formatter("UNKNOWN", Color.Green);
                    break;
                }
            }
            var formatters = new Formatter[] {
                dateTimeFormatter,
                frameFormatter,
                tagFormatter
            };
            Colorful.Console.WriteLineFormatted(outputMessage, Color.Gray, formatters);
        }

        internal enum LogType {
            Error,
            Fatal,
            Info,
            Warning
        }
    }
}