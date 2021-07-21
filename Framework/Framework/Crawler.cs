using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Holism.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace Holism.Framework
{
    public class Crawler
    {
        public static bool LogRequestUrls = false;
        static ProxyServer proxyServer;
        public static Action<SessionEventArgs> ResponseHandler;
        public static bool EnableVerboseLogging = false;
        static IWebDriver driver;

        public static IDocument Get(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var document = BrowsingContext.New(config).OpenAsync(url).Result;
            return document;
        }

        public static IDocument Parse(string html)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);
            return document;
        }

        public static IWebDriver SetupBrowser(bool hasProxy = false, bool waitForPageFullLoad = true)
        {
            int port = 18882;
            if (hasProxy)
            {
                proxyServer = new ProxyServer
                {
                };
                proxyServer.CertificateManager.CertificateEngine = Titanium.Web.Proxy.Network.CertificateEngine.DefaultWindows; proxyServer.CertificateManager.EnsureRootCertificate();
                proxyServer.CertificateManager.TrustRootCertificate(true);
                var explicitEndPoint = new ExplicitProxyEndPoint(System.Net.IPAddress.Any, port, true);
                proxyServer.AddEndPoint(explicitEndPoint);
                proxyServer.Start();
                proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
                proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
                proxyServer.BeforeRequest += OnRequestEventHandler;
                proxyServer.BeforeResponse += (sender, e) =>
                {
                    ResponseHandler?.Invoke(e);
                    return Task.CompletedTask;
                };
            }
            var proxy = new OpenQA.Selenium.Proxy
            {
                HttpProxy = $"http://localhost:{port}",
                SslProxy = $"http://localhost:{port}",
                FtpProxy = $"http://localhost:{port}"
            };
            var options = new ChromeOptions
            {
            };
            if (!waitForPageFullLoad)
            {
                options.PageLoadStrategy = PageLoadStrategy.None;
            }
            if (hasProxy)
            {
                options.Proxy = proxy;
            }
            if (!EnableVerboseLogging)
            {
                driver = new ChromeDriver(options);
                return driver;
            }
            else
            {
                var driverService = ChromeDriverService.CreateDefaultService();
                driverService.EnableVerboseLogging = true;
                driverService.LogPath = Path.Combine(AppContext.BaseDirectory, "ChromeDriverLogs.txt");
                options.AddArgument("disable-infobars");
                options.AddUserProfilePreference("credentials_enable_service", false);
                options.AddUserProfilePreference("profile.password_manager_enabled", false);
                options.SetLoggingPreference(LogType.Browser, LogLevel.All);
                driver = new ChromeDriver(driverService, options);
                return driver;
            }
        }

        public static void StopBrowser()
        {
            if (proxyServer != null)
            {
                proxyServer.Stop();
                proxyServer = null;
            }
            if (driver != null)
            {
                try
                {

                    driver.Close();
                    driver.Quit();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                driver = null;
            }
        }

        private static Task OnRequestEventHandler(object sender, SessionEventArgs e)
        {
            if (LogRequestUrls)
            {
                Logger.LogInfo($"Requesting {e.HttpClient.Request.RequestUri.ToString()}");
            }
            return Task.CompletedTask;
        }

        public static void SetTimeout(IWebDriver driver, int timeoutInSeconds)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeoutInSeconds);
        }

        public static Func<IWebDriver, IWebDriver> FrameToBeAvailableAndSwitchToIt(string frameLocator)
        {
            return (driver) =>
            {
                try
                {
                    return driver.SwitchTo().Frame(frameLocator);
                }
                catch (NoSuchFrameException)
                {
                    return null;
                }
            };
        }
    }
}
