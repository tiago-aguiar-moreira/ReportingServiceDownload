namespace ReportingServices.Core.Model
{
    /// <summary>
    /// Configuration to connect and download files from Reporting Service 
    /// </summary>
    public class DownloadConfiguration
    {
        /// <summary>
        /// Directory to download files
        /// </summary>
        public string DownloadDirectory { get; set; }

        /// <summary>
        /// User to access the Reporting Service
        /// </summary>
        public string ReportingServiceUser { get; set; }

        /// <summary>
        /// Password to access the Reporting Service
        /// </summary>
        public string ReportingServicePassword { get; set; }

        /// <summary>
        /// URL to access the folder contains reports in Reporting Service
        /// </summary>
        public string ReportingServiceURL { get; set; }

        /// <summary>
        /// Compress all files after download
        /// </summary>
        public bool ZipFiles { get; set; }

        /// <summary>
        /// Download interval in seconds
        /// </summary>
        public ushort DownloadInterval { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort TimeOutLoadPage { get; set; }
    }
}
