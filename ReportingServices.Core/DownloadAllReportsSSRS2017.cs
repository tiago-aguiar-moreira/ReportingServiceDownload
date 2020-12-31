using OpenQA.Selenium;
using ReportingServices.Core.EventArguments;
using ReportingServices.Core.Model;
using ReportingServices.Core.Enumerator;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace ReportingServices.Core
{
    public delegate void OnProgressChangedEventHandler(object sender, LogArgs e);

    public class DownloadAllReportsSSRS2017 : DownloadAllReportsBase
    {
        public override event OnProgressEventHandler OnProgress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public DownloadAllReportsSSRS2017(DownloadConfiguration config) : base(config)
        {

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~DownloadAllReportsSSRS2017()
        {
            OnProgress = null;
        }

        public override void Execute()
        {
            try
            {
                PrepareDirectory();

                _driver.Navigate().GoToUrl(GetCredential());

                var areaGroupReports = GetReportCardsContainer();

                if (areaGroupReports != null)
                {
                    var cards = GetReportCards(areaGroupReports);
                    var cardsCount = cards.Count();

                    for (int index = 0; index < cardsCount; index++)
                    {
                        try
                        {
                            OpenPopupDetailsFromCard(cards[index]);
                            GetNameAndURL(out _reportName, out var url);
                            OnProgress?.Invoke(this, new LogArgs(_reportName, cardsCount, index + 1, DownloadState.Started));
                            ClosePopup();
                            Download(_reportName, url);
                            OnProgress?.Invoke(this, new LogArgs(_reportName, cardsCount, index + 1, DownloadState.Success));
                        }
                        catch (Exception ex)
                        {
                            OnProgress?.Invoke(this, new LogArgs(_reportName, cardsCount, index + 1, DownloadState.Fail, ex));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnProgress?.Invoke(this, new LogArgs(ex));
            }
        }

        private IWebElement GetReportCardsContainer()
            => _driver.FindElement(By.XPath("//*[@id='main']/div/div/section[2]/tiles-view/section[2]/div/div/div/ul"));

        private ReadOnlyCollection<IWebElement> GetReportCards(IWebElement webElement)
            => webElement.FindElements(By.TagName("li"));

        private void OpenPopupDetailsFromCard(IWebElement webElement)
        {
            webElement.FindElements(By.TagName("a")).Last().Click();
            _popupDetails = _driver.FindElement(By.XPath("//*[@id='content']/section"));
        }

        private void GetNameAndURL(out string name, out string url)
        {
            var actionGroupDownload = _popupDetails.FindElement(By.TagName("footer")).FindElements(By.TagName("div")).LastOrDefault();

            var menuItemDownload = actionGroupDownload.FindElements(By.TagName("a")).FirstOrDefault();

            name = menuItemDownload.GetAttribute("title");
            url = menuItemDownload.GetAttribute("href");
        }

        private void Download(string rptName, string uri)
        {
            _client.DownloadFile(new Uri(uri), $"{_config.DownloadDirectory}\\{rptName}");
            if (_config.DownloadInterval > 0)
            {
                Thread.Sleep(_config.DownloadInterval);
            }
        }

        private void ClosePopup()
            => _popupDetails.FindElement(By.ClassName("close")).Click();
    }
}
