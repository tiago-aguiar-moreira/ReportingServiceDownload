using ReportingServices.Core;
using ReportingServices.Core.Enumerator;
using ReportingServices.Core.EventArguments;
using ReportingServices.Core.Model;
using System;
using System.Configuration;

namespace ReportingServices.App
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var config = new DownloadConfiguration()
                {
                    DownloadDirectory = ConfigurationManager.AppSettings["downloadDirectory"],
                    ReportingServiceUser = ConfigurationManager.AppSettings["reportingServiceUser"],
                    ReportingServicePassword = ConfigurationManager.AppSettings["reportingServicePassword"],
                    ReportingServiceURL = ConfigurationManager.AppSettings["reportingServiceURL"],
                    ZipFiles = bool.Parse(ConfigurationManager.AppSettings["zipFiles"]),
                    DownloadInterval = ushort.Parse(ConfigurationManager.AppSettings["downloadInterval"]),
                    TimeOutLoadPage = ushort.Parse(ConfigurationManager.AppSettings["timeoutLoadPage"])
                    //DownloadDirectory = @"C:\Temp\Relatorio",
                    //ReportingServiceUser = "tiago.moreira",
                    //ReportingServicePassword = "uw3HypWN6%",
                    //ReportingServiceURL = "http://10.211.20.10/Reports/Pages/Folder.aspx?ItemPath=%2fPortalBematize&ViewMode=List",
                    //DownloadInterval = 0,
                    //TimeOutLoadPage = 0
                };

                var serverVersion = ConfigurationManager.AppSettings["SSRSVersion"];
                //var serverVersion = "2003";
                DownloadAllReportsBase downloadFiles;

                switch (serverVersion)
                {
                    case "2003":
                        downloadFiles = new DownloadAllReportsSSRS2014(config);
                        break;
                    case "2017":
                        downloadFiles = new DownloadAllReportsSSRS2017(config);
                        break;
                    default:
                        throw new ArgumentException("Version server is not supported or not supported");
                }

                downloadFiles.OnProgress += (object sender, LogArgs e) =>
                {
                    switch (e.DownloadState)
                    {
                        case DownloadState.Started:
                            Console.WriteLine($"Report: {e.FileName}");
                            break;
                        case DownloadState.Success:
                            Console.WriteLine("Download sucess!");
                            break;
                        case DownloadState.Fail:
                            Console.WriteLine("Download fail!");
                            break;
                    }
                };

                Console.WriteLine($"Started{Environment.NewLine}");

                downloadFiles.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Finished");
            }
        }
    }
}