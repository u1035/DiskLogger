#nullable enable

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DiskLogger
{
    internal sealed class DiskOperationsEngine : IDisposable
    {
        /// <summary>
        /// Contains message ready to be written to disk and timestamp.
        /// Timestamp is used for detecting beginning of new day - when we need to start new log file.
        /// </summary>
        private readonly struct Record
        {
            public DateTime Timestamp { get; }
            public string FormattedMessage { get; }

            public Record(DateTime timestamp, string formattedMessage)
            {
                Timestamp = timestamp;
                FormattedMessage = formattedMessage;
            }
        }


        private readonly string _folder;
        private readonly string _fileNamePrefix;

        #region Private fields

        /// <summary>
        /// Time between disk operations
        /// </summary>
        private const int FileOperationsInterval = 1000; //we flush _pendingLogRecords collection to disk once per second (1000 ms)

        /// <summary>
        /// Contains date of current log file. Used to watch for date change
        /// </summary>
        private DateTime _today;

        /// <summary>
        /// Current log file stream, opened for append
        /// </summary>
        private StreamWriter? _logFileStream;

        private readonly object _logFileStreamLock = new();

        /// <summary>
        /// Timer, that writes to disk contents of <see cref="_pendingLogRecords"/> collection
        /// </summary>
        private readonly Timer _fileOperationsTimer;

        /// <summary>
        /// Queue of records waiting for writing to disk
        /// </summary>
        private readonly ConcurrentQueue<Record> _pendingLogRecords = new();
        private readonly object _pendingLogRecordsLock = new();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="DiskOperationsEngine"/>
        /// </summary>
        /// <param name="folder">Folder that will contain log files, one per day</param>
        /// <param name="fileNamePrefix">Prefix for log file names (example for value "prefix": prefix_2022-10-29.log)</param>
        public DiskOperationsEngine(string folder, string fileNamePrefix = "")
        {
            _folder = folder;
            _fileNamePrefix = fileNamePrefix;
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            OpenFile();
            _fileOperationsTimer = new Timer(_ => ProcessQueue(), null, FileOperationsInterval, FileOperationsInterval);
        }

        #endregion

        #region Disk operations

        /// <summary>
        /// Opens for append file with current date in name.
        /// Stores date of current file in <see cref="_today"/> variable
        /// https://docs.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#no-asynchronous-logger-methods
        /// </summary>
        /// <returns></returns>
        private void OpenFile()
        {
            try
            {
                var today = DateTime.Today;
                _today = today;
                var filename = Path.Combine(_folder, GetFileName(today, _fileNamePrefix));
                
                lock (_logFileStreamLock)
                {
                    Debug.Assert(_logFileStream is null);
                    _logFileStream = File.AppendText(filename);
                }

            }
            catch (Exception)
            {
                //exception ignored
            }
        }

        private static string GetFileName(DateTime date, string prefix)
        {
            if (prefix.Length == 0)
                return $@"{date:yyyy-MM-dd}.log";

            //in case if prefix contains invalid path characters - we replace them with '_' symbol
            foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
                prefix = prefix.Replace(invalidFileNameChar, '_');

            return $@"{prefix}_{date:yyyy-MM-dd}.log";
        }

        /// <summary>
        /// Closes current log file
        /// </summary>
        private void CloseFile()
        {
            lock (_logFileStreamLock)
            {
                if (_logFileStream != null)
                {
                    _logFileStream.Close();
                    _logFileStream.Dispose();
                    _logFileStream = null;
                }
            }
        }

        /// <summary>
        /// We separate receiving log messages and writing it to file, doing it asynchronously.
        /// It helps to speed up returning control back to program (code does not wait for writing to disk and flushing).
        /// </summary>
        private void ProcessQueue()
        {
            var recordsWritten = 0;

            while (true)
            {
                Record record;
                lock (_pendingLogRecordsLock)
                    if (!_pendingLogRecords.TryDequeue(out record))
                        break;

                recordsWritten++;
                //if we meet record with new date - we assume that new day started and rotate log file
                if (record.Timestamp.Date != _today)
                {
                    CloseFile();
                    OpenFile();
                }

                lock (_logFileStreamLock)
                    _logFileStream?.WriteLine(record.FormattedMessage);
            }

            if (recordsWritten > 0)
                lock (_logFileStreamLock)
                    _logFileStream?.Flush();
        }

        #endregion

        #region Queue operations

        /// <summary>
        /// Adds new log message to disk queue
        /// Queue flushed by <see cref="ProcessQueue"/> every <see cref="FileOperationsInterval"/> milliseconds by <see cref="_fileOperationsTimer"/>
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="formattedMessage"></param>
        public void Enqueue(DateTime timestamp, string formattedMessage)
        {
            lock (_pendingLogRecordsLock)
                _pendingLogRecords.Enqueue(new Record(timestamp, formattedMessage));
        }

        #endregion

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _fileOperationsTimer.Dispose();

            //we force to log all messages, if any left in queue
            ProcessQueue();
            CloseFile();
        }

        #endregion
    }
}