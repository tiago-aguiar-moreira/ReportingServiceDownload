using ReportingServices.Core.Enumerator;
using System;

namespace ReportingServices.Core.EventArguments
{
    public class LogArgs : EventArgs
    {
        public string FileName { get; set; }

        public int Count { get; set; }

        public int Progress { get; set; }

        public DownloadState DownloadState { get; set; }

        public Exception Exception { get; set; }

        public LogArgs(Exception exception)
        {
            Exception = exception;
        }

        public LogArgs(string fileName, int count, int progress, DownloadState downloadState)
        {
            FileName = fileName;
            Count = count;
            Progress = progress;
            DownloadState = downloadState;
        }

        public LogArgs(string fileName, int count, int progress, DownloadState downloadState, Exception exception)
        {
            FileName = fileName;
            Count = count;
            Progress = progress;
            DownloadState = downloadState;
            Exception = exception;
        }
    }
}