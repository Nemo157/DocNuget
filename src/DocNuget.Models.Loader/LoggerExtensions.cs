using Microsoft.Framework.PackageManager;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Logging;

namespace DocNuget.Models.Loader {
    internal static class LoggerExtensions {
        public static Reports CreateReports(this ILogger logger) {
            return new Reports {
                Information = new LoggerReport(LogLevel.Information, logger),
                Verbose = new LoggerReport(LogLevel.Verbose, logger),
                Quiet = new LoggerReport(LogLevel.Debug, logger),
                Error = new LoggerReport(LogLevel.Error, logger),
            };
        }
    }
}
