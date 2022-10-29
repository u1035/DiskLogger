using System;

namespace DiskLogger
{
    /// <summary>
    /// Contains information about log record
    /// </summary>
    public class LogRecord
    {
        /// <summary>
        /// Record time
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// Name of context or type of class that logger was created for
        /// </summary>
        public string Context { get; }

        /// <summary>
        /// Sender
        /// </summary>
        public string Sender { get; }

        /// <summary>
        /// Log message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Message severity level
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Caller member name
        /// </summary>
        public string CallerMemberName { get; }

        /// <summary>
        /// Caller source file
        /// </summary>
        public string SourceFilePath { get; }

        /// <summary>
        /// Caller source file line
        /// </summary>
        public int SourceFileLine { get; }

        /// <summary>
        /// Creates new LogRecord object
        /// </summary>
        /// <param name="time">Record time</param>
        /// <param name="context">Name of context or type of class that logger was created for</param>
        /// <param name="sender">Sender</param>
        /// <param name="message">Log message</param>
        /// <param name="level">Message severity level</param>
        /// <param name="callerMemberName">Caller member name</param>
        /// <param name="sourceFilePath">Caller source file</param>
        /// <param name="sourceFileLine">Caller source file line</param>
        public LogRecord(DateTime time, string context, string sender, string message, LogLevel level, string callerMemberName, string sourceFilePath, int sourceFileLine)
        {
            Time = time;
            Context = context;
            Sender = sender;
            Message = message;
            Level = level;
            CallerMemberName = callerMemberName;
            SourceFilePath = sourceFilePath;
            SourceFileLine = sourceFileLine;
        }
    }
}
