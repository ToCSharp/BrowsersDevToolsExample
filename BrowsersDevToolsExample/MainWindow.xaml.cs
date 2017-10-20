using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Zu.AsyncWebDriver.Remote;
using Zu.Chrome;
using Zu.Firefox;
using Zu.Opera;
using Zu.WebBrowser;
using Zu.WebBrowser.BasicTypes;

namespace BrowsersDevToolsExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AsyncChromeDriver asyncChromeDriver;
        private AsyncOperaDriver asyncOperaDriver;
        private AsyncFirefoxDriver asyncFirefoxDriver;
        private WebDriver webDriver;

        private List<IAsyncWebBrowserClient> browsersToClose = new List<IAsyncWebBrowserClient>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await OpenChrome(new ChromeDriverConfig().SetDoOpenBrowserDevTools());
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (webDriver == null) return;
            await webDriver.GoToUrl(tbOpenUrl.Text);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            browsersToClose.Reverse();
            foreach (var br in browsersToClose)
            {
                try
                {
                    br.CloseSync();
                }
                catch { }
            }

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            browsersToClose.Reverse();
            foreach (var br in browsersToClose)
            {
                try
                {
                    await br.Close();
                }
                catch { }
            }

            AddInfo("browser closed");
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            await OpenChrome(new ChromeDriverConfig().SetHeadless().SetDoOpenBrowserDevTools());
        }

        private async Task OpenChrome(ChromeDriverConfig chromeDriverConfig)
        {
            try
            {
                if (webDriver != null) await webDriver.Close();
                asyncChromeDriver = new AsyncChromeDriver(chromeDriverConfig);
                browsersToClose.Add(asyncChromeDriver);
                webDriver = new WebDriver(asyncChromeDriver);
                await asyncChromeDriver.Connect();

                AddInfo($"opened on port {asyncChromeDriver.Port} in dir {asyncChromeDriver.UserDir} \nWhen close, dir will be DELETED");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }

        private async Task OpenOpera(DriverConfig driverConfig)
        {
            try
            {
                if (webDriver != null) await webDriver.Close();
                if (driverConfig is ChromeDriverConfig) asyncOperaDriver = new AsyncOperaDriver((ChromeDriverConfig)driverConfig);
                else asyncOperaDriver = new AsyncOperaDriver(driverConfig);
                browsersToClose.Add(asyncOperaDriver);
                webDriver = new WebDriver(asyncOperaDriver);
                await asyncOperaDriver.Connect();

                AddInfo($"opened on port {asyncOperaDriver.Port} in dir {asyncOperaDriver.UserDir} \nWhen close, dir will be DELETED");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }

        private void AddInfo(string mess)
        {
            tbOpenInfo.Text = mess;
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (webDriver == null) return;
            var screenshot = await webDriver.GetScreenshot();
            string path = GetFilePathToSaveScreenshot(tbOpenSaveScreenshotDir.Text);
            using (MemoryStream imageStream = new MemoryStream(screenshot.AsByteArray))
            {
                System.Drawing.Image screenshotImage = System.Drawing.Image.FromStream(imageStream);
                screenshotImage.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }

        }

        private static string GetFilePathToSaveScreenshot(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var i = 0;
            var path = "";
            do
            {
                i++;
                path = Path.Combine(dir, $"screenshot{i}.png");
            } while (File.Exists(path));
            return path;
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            await OpenOpera(new DriverConfig().SetDoOpenBrowserDevTools());
        }

        private async void Button_Click_6(object sender, RoutedEventArgs e)
        {
            await OpenOpera(new DriverConfig().SetHeadless().SetDoOpenBrowserDevTools());
        }

        private async void Button_Click_8(object sender, RoutedEventArgs e)
        {
            await OpenOpera(new DriverConfig().SetHeadless().SetDoOpenBrowserDevTools());
        }

        private async void Button_Click_28(object sender, RoutedEventArgs e)
        {
            try
            {
                if (webDriver != null) await webDriver.Close();
                var profileName = tbOpenProfileName.Text;
                var dir = Path.Combine(tbOpenProfileDir.Text, profileName);
                await FirefoxProfilesWorker.CreateFirefoxProfile(dir, profileName);
                asyncFirefoxDriver = new AsyncFirefoxDriver(new FirefoxDriverConfig()
                    .SetProfileName(profileName)
                    .SetDoOpenBrowserDevTools()
                    );
                browsersToClose.Add(asyncFirefoxDriver);
                webDriver = new WebDriver(asyncFirefoxDriver);
                await webDriver.Open();
                AddInfo("BrowserDevTools opened");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }

        private async void Button_Click_21(object sender, RoutedEventArgs e)
        {
            try
            {
                if (webDriver != null) await webDriver.Close();
                var profileName = tbOpenProfileName.Text;
                asyncFirefoxDriver = new AsyncFirefoxDriver(new FirefoxDriverConfig()
                    .SetProfileName(profileName)
                    .SetDoOpenBrowserDevTools()
                    );
                browsersToClose.Add(asyncFirefoxDriver);
                webDriver = new WebDriver(asyncFirefoxDriver);
                await webDriver.Open();
                AddInfo("BrowserDevTools opened");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }

        private async void Button_Click_18(object sender, RoutedEventArgs e)
        {
            try
            {
                if (webDriver != null) await webDriver.Close();
                var profileName = tbOpenProfileName.Text;
                asyncFirefoxDriver = new AsyncFirefoxDriver(new FirefoxDriverConfig()
                    .SetProfileName(profileName)
                    .SetIsMultiprocessFalse()
                    .SetDoSetDebuggerRemoteEnabled()
                    );
                webDriver = new WebDriver(asyncFirefoxDriver);
                browsersToClose.Add(asyncFirefoxDriver);
                await webDriver.Open();
                await asyncFirefoxDriver.OpenBrowserDevTools(new Random().Next(2000) + 22000, false);
                AddInfo("BrowserDevTools opened in XUL");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }

        private async void Button_Click_7(object sender, RoutedEventArgs e)
        {
            asyncChromeDriver = new AsyncChromeDriver(new ChromeDriverConfig().SetDoOpenWSProxy());
            webDriver = new WebDriver(asyncChromeDriver);
            browsersToClose.Add(asyncChromeDriver);
            await webDriver.Open();

            asyncFirefoxDriver = new AsyncFirefoxDriver();
            browsersToClose.Add(asyncFirefoxDriver);
            await asyncFirefoxDriver.Connect();
            await asyncFirefoxDriver.Navigation.GoToUrl(asyncChromeDriver.GetBrowserDevToolsUrl());
        }

        private async void Button_Click_9(object sender, RoutedEventArgs e)
        {
            asyncChromeDriver = new AsyncChromeDriver(new ChromeDriverConfig().SetDoOpenWSProxy());
            browsersToClose.Add(asyncChromeDriver);
            webDriver = new WebDriver(asyncChromeDriver);
            await webDriver.Open();

            asyncOperaDriver = new AsyncOperaDriver();
            browsersToClose.Add(asyncOperaDriver);
            await asyncOperaDriver.Connect();
            await asyncOperaDriver.Navigation.GoToUrl(asyncChromeDriver.GetBrowserDevToolsUrl());

        }

        private async void Button_Click_10(object sender, RoutedEventArgs e)
        {
            await OpenOpera(new ChromeDriverConfig().SetDoOpenWSProxy());
            await asyncOperaDriver.OpenBrowserDevTools();
            // OR
            //chromeDevTools = new AsyncChromeDriver();
            //await chromeDevTools.Navigation.GoToUrl(asyncOperaDriver.GetBrowserDevToolsUrl());

        }

        private async void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (asyncFirefoxDriver?.BrowserDevTools != null)
            {
                await asyncFirefoxDriver.BrowserDevTools.OpenBrowserDevTools(25000 + new Random().Next(2000));
            }

        }

        private async void Button_Click_12(object sender, RoutedEventArgs e)
        {
            if (webDriver == null) return;
            await webDriver.GoToUrl(tbOpenUrl2.Text);
        }

        private async void Button_Click_13(object sender, RoutedEventArgs e)
        {
            try
            {
                asyncChromeDriver = new AsyncChromeDriver(
                    new ChromeDriverConfig()
                    .SetDoOpenBrowserDevTools()
                    .SetWSProxyConfig(new ChromeWSProxyConfig { HTTPServerSaveRequestedFiles = true })
                    );
                browsersToClose.Add(asyncChromeDriver);
                webDriver = new WebDriver(asyncChromeDriver);
                await asyncChromeDriver.Connect();
                tbDevToolsFilesDir.Text = Path.Combine(asyncChromeDriver.Config.WSProxyConfig.DevToolsFilesDir, "devtools");
                tbDevToolsRes.Text = "opened";
            }
            catch (Exception ex)
            {
                tbDevToolsRes.Text = ex.ToString();
            }

        }

        private async void Button_Click_14(object sender, RoutedEventArgs e)
        {
            var dir = tbDevToolsFilesDir.Text;
            if (string.IsNullOrWhiteSpace(dir)) dir = Directory.GetCurrentDirectory();
            dir = dir.TrimEnd('\\');
            if (dir.EndsWith("\\devtools")) dir = Path.GetDirectoryName(dir);
            try
            {
                asyncChromeDriver = new AsyncChromeDriver(
                    new ChromeDriverConfig()
                    .SetDoOpenBrowserDevTools()
                    .SetWSProxyConfig(new ChromeWSProxyConfig { DevToolsFilesDir = dir, HTTPServerTryFindRequestedFileLocaly = true })
                    );
                browsersToClose.Add(asyncChromeDriver);
                webDriver = new WebDriver(asyncChromeDriver);
                await asyncChromeDriver.Connect();
                tbDevToolsRes.Text = "opened";
            }
            catch (Exception ex)
            {
                tbDevToolsRes.Text = ex.ToString();
            }

        }

        private async void Button_Click_15(object sender, RoutedEventArgs e)
        {
            if (webDriver == null) return;
            var input = await webDriver.FindElementByNameOrDefault("q");
            if (input == null) input = await webDriver.FindElementByTagNameOrDefault("input");
            if (input != null) await input.SendKeys(tbOpenSendKeysToInput.Text);
        }

        private async void Button_Click_16(object sender, RoutedEventArgs e)
        {
            if (webDriver == null) return;
            var input = await webDriver.FindElementByNameOrDefault("q");
            if (input == null) input = await webDriver.FindElementByTagNameOrDefault("input");
            if (input != null) await input.SendKeys(Keys.Enter);
        }

        private async void Button_Click_17(object sender, RoutedEventArgs e)
        {
            var acd = new AsyncChromeDriver();
            browsersToClose.Add(acd);
            await acd.Connect();
            await acd.Navigation.GoToUrl(asyncChromeDriver.GetBrowserDevToolsUrl());

        }

        private async void Button_Click_19(object sender, RoutedEventArgs e)
        {
            var aod = new AsyncOperaDriver();
            browsersToClose.Add(aod);
            await aod.Connect();
            await aod.Navigation.GoToUrl(asyncChromeDriver.GetBrowserDevToolsUrl());

        }

        private async void Button_Click_20(object sender, RoutedEventArgs e)
        {
            if (webDriver == null) return;
            var b = await webDriver.FindElementById("sb_form_go");
            if (b != null) await b.Click();
        }

        private async void Button_Click_22(object sender, RoutedEventArgs e)
        {
            try
            {
                asyncChromeDriver = new AsyncChromeDriver(new ChromeDriverConfig().SetDoOpenBrowserDevTools());
                asyncChromeDriver.BrowserDevToolsConfig = new ChromeDriverConfig().SetDoOpenBrowserDevTools();
                browsersToClose.Add(asyncChromeDriver);
                webDriver = new WebDriver(asyncChromeDriver);
                await asyncChromeDriver.Connect();
                tbDevToolsRes.Text = "opened";

            }
            catch (Exception ex)
            {
                tbDevToolsRes.Text = ex.ToString();
            }
        }

        private async void Button_Click_23(object sender, RoutedEventArgs e)
        {
            try
            {
                if (webDriver != null) await webDriver.Close();
                asyncFirefoxDriver = new AsyncFirefoxDriver(new FirefoxDriverConfig()
                    .SetDoOpenBrowserDevTools()
                    );
                browsersToClose.Add(asyncFirefoxDriver);
                webDriver = new WebDriver(asyncFirefoxDriver);
                await webDriver.Open();
                AddInfo("BrowserDevTools opened");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }

        }

        private async void Button_Click_24(object sender, RoutedEventArgs e)
        {
            try
            {
                if (asyncFirefoxDriver?.BrowserDevTools == null) return;
                await asyncFirefoxDriver.BrowserDevTools.JavaScriptExecutor.ExecuteScript("frames[0].document.getElementById('toolbox-tab-netmonitor').click();");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }

        private async void Button_Click_25(object sender, RoutedEventArgs e)
        {
            try
            {
                if (asyncFirefoxDriver?.BrowserDevTools == null) return;
                await asyncFirefoxDriver.BrowserDevTools.JavaScriptExecutor.ExecuteScript("frames[0].document.getElementById('toolbox-tab-webconsole').click();");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }

        private async void Button_Click_26(object sender, RoutedEventArgs e)
        {
            try
            {
                if (asyncFirefoxDriver?.BrowserDevTools == null) return;
                await asyncFirefoxDriver.BrowserDevTools.JavaScriptExecutor.ExecuteScript("frames[0].document.getElementById('toolbox-tab-inspector').click();");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }

        private async void Button_Click_27(object sender, RoutedEventArgs e)
        {
            try
            {
                if (asyncFirefoxDriver?.BrowserDevTools?.BrowserDevTools == null) return;
                await asyncFirefoxDriver.BrowserDevTools.BrowserDevTools.JavaScriptExecutor.ExecuteScript("frames[0].document.getElementById('toolbox-tab-netmonitor').click();");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }

        private async void Button_Click_29(object sender, RoutedEventArgs e)
        {
            try
            {
                if (asyncFirefoxDriver?.BrowserDevTools?.BrowserDevTools == null) return;
                await asyncFirefoxDriver.BrowserDevTools.BrowserDevTools.JavaScriptExecutor.ExecuteScript("frames[0].document.getElementById('toolbox-tab-inspector').click();");
            }
            catch (Exception ex)
            {
                AddInfo(ex.ToString());
            }
        }
    }
}
