using System;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace DiskLogger
{
    /// <summary>
    /// Provides functions for writing log messages
    /// </summary>
    [PublicAPI]
    public sealed class Logger
    {
        private readonly string _contextName;
        private readonly DiskOperationsEngine _diskOperationsEngine;

        #region Constructor

        internal Logger(string contextName, DiskOperationsEngine diskOperationsEngine)
        {
            _contextName = contextName;
            _diskOperationsEngine = diskOperationsEngine;
        }

        #endregion

        #region Log records adding

        /// <summary>
        /// Adds fatal error message to log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="sender">Sender name (can be anything - name of module or function)</param>
        /// <param name="callerMember">Caller member</param>
        /// <param name="sourcePath">Caller source file</param>
        /// <param name="lineNumber">Caller source file line</param>
        public void Fatal(string message, string sender = "", [CallerMemberName] string callerMember = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AddRecord(new LogRecord(DateTime.Now, _contextName, sender, message, LogLevel.Fatal, callerMember, sourcePath, lineNumber));
        }

        /// <summary>
        /// Adds error message to log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="sender">Sender name (can be anything - name of module or function)</param>
        /// <param name="callerMember">Caller member</param>
        /// <param name="sourcePath">Caller source file</param>
        /// <param name="lineNumber">Caller source file line</param>
        public void Error(string message, string sender = "", [CallerMemberName] string callerMember = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AddRecord(new LogRecord(DateTime.Now, _contextName, sender, message, LogLevel.Error, callerMember, sourcePath, lineNumber));
        }

        /// <summary>
        /// Adds warning message to log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="sender">Sender name (can be anything - name of module or function)</param>
        /// <param name="callerMember">Caller member</param>
        /// <param name="sourcePath">Caller source file</param>
        /// <param name="lineNumber">Caller source file line</param>
        public void Warning(string message, string sender = "", [CallerMemberName] string callerMember = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AddRecord(new LogRecord(DateTime.Now, _contextName, sender, message, LogLevel.Warning, callerMember, sourcePath, lineNumber));
        }

        /// <summary>
        /// Adds notice message to log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="sender">Sender name (can be anything - name of module or function)</param>
        /// <param name="callerMember">Caller member</param>
        /// <param name="sourcePath">Caller source file</param>
        /// <param name="lineNumber">Caller source file line</param>
        public void Notice(string message, string sender = "", [CallerMemberName] string callerMember = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AddRecord(new LogRecord(DateTime.Now, _contextName, sender, message, LogLevel.Notice, callerMember, sourcePath, lineNumber));
        }

        /// <summary>
        /// Adds info message to log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="sender">Sender name (can be anything - name of module or function)</param>
        /// <param name="callerMember">Caller member</param>
        /// <param name="sourcePath">Caller source file</param>
        /// <param name="lineNumber">Caller source file line</param>
        public void Info(string message, string sender = "", [CallerMemberName] string callerMember = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AddRecord(new LogRecord(DateTime.Now, _contextName, sender, message, LogLevel.Info, callerMember, sourcePath, lineNumber));
        }

        /// <summary>
        /// Adds debug message to log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="sender">Sender name (can be anything - name of module or function)</param>
        /// <param name="callerMember">Caller member</param>
        /// <param name="sourcePath">Caller source file</param>
        /// <param name="lineNumber">Caller source file line</param>
        public void Debug(string message, string sender = "", [CallerMemberName] string callerMember = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AddRecord(new LogRecord(DateTime.Now, _contextName, sender, message, LogLevel.Debug, callerMember, sourcePath, lineNumber));
        }

        /// <summary>
        /// Adds trace message to log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="sender">Sender name (can be anything - name of module or function)</param>
        /// <param name="callerMember">Caller member</param>
        /// <param name="sourcePath">Caller source file</param>
        /// <param name="lineNumber">Caller source file line</param>
        public void Trace(string message, string sender = "", [CallerMemberName] string callerMember = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AddRecord(new LogRecord(DateTime.Now, _contextName, sender, message, LogLevel.Trace, callerMember, sourcePath, lineNumber));
        }

        /// <summary>
        /// Adds message to log
        /// </summary>
        /// <param name="level">Severity level of log message</param>
        /// <param name="message">Message text</param>
        /// <param name="sender">Sender name (can be anything - name of module or function)</param>
        /// <param name="callerMember">Caller member</param>
        /// <param name="sourcePath">Caller source file</param>
        /// <param name="lineNumber">Caller source file line</param>
        public void AddRecord(LogLevel level, string message, string sender = "", [CallerMemberName] string callerMember = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AddRecord(new LogRecord(DateTime.Now, _contextName, sender, message, level, callerMember, sourcePath, lineNumber));
        }

        /// <summary>
        /// Adds error message to log
        /// </summary>
        /// ///
        /// <param name="time">Time of log message</param>
        /// <param name="level">Severity level of log message</param>
        /// <param name="message">Message text</param>
        /// <param name="sender">Sender name (can be anything - name of module or function)</param>
        /// <param name="callerMember">Caller member</param>
        /// <param name="sourcePath">Caller source file</param>
        /// <param name="lineNumber">Caller source file line</param>
        public void AddRecord(DateTime time, LogLevel level, string message, string sender = "", [CallerMemberName] string callerMember = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AddRecord(new LogRecord(time, _contextName, sender, message, level, callerMember, sourcePath, lineNumber));
        }

        private void AddRecord(LogRecord record)
        {
            var sourceFile = (record.SourceFilePath != "") ? Path.GetFileName(record.SourceFilePath) : "";
            var formattedRecord = $"{record.Time:O}|{record.Context}|{record.Sender}|{record.CallerMemberName}|{sourceFile}|{record.SourceFileLine}|{record.Level}|{record.Message}";
            _diskOperationsEngine.Enqueue(record.Time, formattedRecord);
        }

        #endregion
    }
}
