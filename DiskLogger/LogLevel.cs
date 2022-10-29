using System;
using System.Collections.Generic;

namespace DiskLogger
{
    /// <summary>
    /// Log message severity level
    /// </summary>
    public class LogLevel : IComparable
    {
        /// <summary>
        /// Log Trace level
        /// </summary>
        public static LogLevel Trace { get; } = new LogLevel(0, "Trace");

        /// <summary>
        /// Log Debug level
        /// </summary>
        public static LogLevel Debug { get; } = new LogLevel(1, "Debug");

        /// <summary>
        /// Log Notice level
        /// </summary>
        public static LogLevel Notice { get; } = new LogLevel(2, "Notice");

        /// <summary>
        /// Log Info level
        /// </summary>
        public static LogLevel Info { get; } = new LogLevel(3, "Info");

        /// <summary>
        /// Log Warning level
        /// </summary>
        public static LogLevel Warning { get; } = new LogLevel(4, "Warning");

        /// <summary>
        /// Log Error level
        /// </summary>
        public static LogLevel Error { get; } = new LogLevel(5, "Error");

        /// <summary>
        /// Log Fatal level
        /// </summary>
        public static LogLevel Fatal { get; } = new LogLevel(6, "Fatal");

        private string Name { get; }
        private int Value { get; }

        private LogLevel(int val, string name)
        {
            Value = val;
            Name = name;
        }

        /// <summary>
        /// Returns collection of all available severity levels
        /// </summary>
        /// <returns>Collection of all available severity levels</returns>
        public static IEnumerable<LogLevel> ListAll()
        {
            return new[] { Trace, Debug, Notice, Info, Warning, Error, Fatal };
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is LogLevel otherValue))
            {
                return false;
            }

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <inheritdoc />
        int IComparable.CompareTo(object obj)
        {
            return Value.CompareTo(((LogLevel)obj).Value);
        }
    }
}
