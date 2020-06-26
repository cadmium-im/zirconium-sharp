using System;

namespace Zirconium.Core.Logging
{
    public class Log
    {
        private static Logger _defaultLogger = new Logger("", true, false, true);
        private static object loggerMutex = new Object();

        public static Logger Initialize(string name, bool verbose, bool systemLog) {
            var logger = new Logger(name, verbose, systemLog, false);
            if (!_defaultLogger.isInitialized) {
                lock (loggerMutex) {
                    if (!_defaultLogger.isInitialized) {
                        var defLogger = new Logger(name, verbose, systemLog, true);
                        _defaultLogger = defLogger;
                        _defaultLogger.isInitialized = true;
                    }
                }
            }
            return logger;
        }

        public static void Close() {
            lock (loggerMutex) {
                _defaultLogger.Close();
            }
        }

        public static void Error(string message) {
            _defaultLogger.Error(message);
        }

        public static void Warning(string message) {
            _defaultLogger.Warning(message);
        }

        public static void Fatal(string message) {
            _defaultLogger.Fatal(message);
        }

        public static void Info(string message) {
            _defaultLogger.Info(message);
        }
    }
}