using Microsoft.Framework.PackageManager;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Logging;

namespace DocNuget.Models.Loader {
    internal class LoggerReport : IReport {
        private readonly LogLevel _logLevel;
        private readonly Microsoft.Framework.Logging.ILogger _logger;

        public LoggerReport(LogLevel logLevel, ILogger logger) {
            _logLevel = logLevel;
            _logger = logger;
        }

        public void WriteLine(string message) {
            _logger.Log(_logLevel, 0, message, null, null);
        }
    }
}
