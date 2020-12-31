using OpenQA.Selenium;
using ReportingServices.Core.Enumerator;
using ReportingServices.Core.EventArguments;
using ReportingServices.Core.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Web;

namespace ReportingServices.Core
{
    public class DownloadAllReportsSSRS2014 : DownloadAllReportsBase
    {
        public override event OnProgressEventHandler OnProgress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public DownloadAllReportsSSRS2014(DownloadConfiguration config) : base(config)
        {

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~DownloadAllReportsSSRS2014()
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
                            //_reportName = cards[index].Text;
                            OpenPopupDetailsFromCard(cards[index]);
                            GetNameAndURL(out _reportName, out var url);
                            OnProgress?.Invoke(this, new LogArgs(_reportName, cardsCount, index + 1, DownloadState.Started));
                            Download(_reportName, url, out var downloadState);
                            OnProgress?.Invoke(this, new LogArgs(_reportName, cardsCount, index + 1, downloadState));
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
            => _driver.FindElement(By.XPath("//*[@id='ui_form']/span/table/tbody/tr[2]/td/table/tbody/tr/td/span/table/tbody/tr/td/span/table/tbody/tr/td[2]/table/tbody"));

        private ReadOnlyCollection<IWebElement> GetReportCards(IWebElement webElement)
            => webElement.FindElements(By.ClassName("msrs-tileView"));

        private void OpenPopupDetailsFromCard(IWebElement webElement)
        {
            var body = webElement.FindElements(By.TagName("td"));

            body.Last().Click();

            _popupDetails = _driver.FindElement(By.XPath("//*[@id='contextMenuCollection']"));
        }

        private void GetNameAndURL(out string name, out string url)
        {
            var urlBuilder = new UriBuilder(_config.ReportingServiceURL);

            var query = HttpUtility.ParseQueryString(urlBuilder.Query);

            query["SelectedTabId"] = "PropertiesTab";
            query["Export"] = "true";

            urlBuilder.Query = query.ToString();

            url = urlBuilder.ToString();

            var input = _popupDetails.FindElement(By.TagName("input"));

            var resource = input.GetAttribute("value").Split('/').LastOrDefault();

            name = resource;
        }

        private void Download(string rptName, string uri, out DownloadState downloadState)
        {
            if (_popupDetails.FindElement(By.Id("ui_rcmdownload")).Displayed)
            {
                //btnDownload.FindElement(By.TagName("span")).Click();
                var extensionRpt = ".rdl";
                if (!rptName.Contains(extensionRpt))
                {
                    rptName += $"{rptName}{extensionRpt}";
                }

                _client.DownloadFile(new Uri(uri), $"{_config.DownloadDirectory}\\{rptName}");
                if (_config.DownloadInterval > 0)
                {
                    Thread.Sleep(_config.DownloadInterval);
                }

                downloadState = DownloadState.Success;
            }
            else
            {
                downloadState = DownloadState.Ignored;
            }
        }
    }
}