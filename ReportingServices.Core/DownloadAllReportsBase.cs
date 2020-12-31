using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ReportingServices.Core.EventArguments;
using ReportingServices.Core.Model;
using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace ReportingServices.Core
{
    public delegate void OnProgressEventHandler(object sender, LogArgs e);

    public class DownloadAllReportsBase
    {
        protected string _reportName;
        protected DownloadConfiguration _config;
        protected ChromeDriver _driver;
        protected IWebElement _popupDetails;
        protected readonly WebClient _client;

        public virtual event OnProgressEventHandler OnProgress;

        public DownloadAllReportsBase(DownloadConfiguration config)
        {
            _config = config;
            _driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //_driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, config.TimeOutLoadPage);
            _driver.Manage().Window.Maximize();

            _client = new WebClient()
            {
                Credentials = new NetworkCredential(config.ReportingServiceUser, config.ReportingServicePassword)
            };
        }

        ~DownloadAllReportsBase()
        {
            _driver?.Dispose();
            _client?.Dispose();
        }

        public void PrepareDirectory()
        {
            if (!Directory.Exists(_config.DownloadDirectory))
            {
                Directory.CreateDirectory(_config.DownloadDirectory);
            }
        }

        public string GetCredential()
            => new UriBuilder(_config.ReportingServiceURL)
            {
                UserName = _config.ReportingServiceUser,
                Password = _config.ReportingServicePassword
            }.ToString();

        public virtual void Execute()
        {
            throw new NotImplementedException();
        }
    }
}