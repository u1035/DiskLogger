using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace DiskLogger
{
    /// <summary>
    /// Creates and manages loggers
    /// </summary>
    [PublicAPI]
    public sealed class LogManager : IDisposable
    {
        private readonly DiskOperationsEngine _diskOperationsEngine;

        private readonly ConcurrentDictionary<string, Logger> _loggers = new();
        private readonly object _loggersLock = new();

        /// <summary>
        /// Initializes a new instance of <see cref="LogManager"/>
        /// </summary>
        /// <param name="logsFolder">Folder that will contain log files, one per day</param>
        /// <param name="logFilePrefix">Prefix for log file names (example for value "prefix": prefix-2022-10-29.log)</param>
        public LogManager(string logsFolder, string logFilePrefix)
        {
            _diskOperationsEngine = new DiskOperationsEngine(logsFolder, logFilePrefix);
        }

        /// <summary>
        /// Returns logger instance for specified type name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Logger ForContext<T>()
        {
            return ForContext(typeof(T).Name);
        }

        /// <summary>
        /// Returns logger instance for specified context name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Logger ForContext(string context)
        {
            lock (_loggersLock)
            {
                if (!_loggers.TryGetValue(context, out var logger))
                    logger = new Logger(context, _diskOperationsEngine);

                return logger;
            }
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            lock (_loggersLock) 
                _loggers.Clear();
            _diskOperationsEngine?.Dispose();
        }
    }
}
