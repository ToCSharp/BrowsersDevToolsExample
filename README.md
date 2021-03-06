## We do not need to proxy Chrome DevTools WS traffic from Chrome 63  
https://developers.google.com/web/updates/2017/10/devtools-release-notes#multi-client  
This project is OUT OF DATE  
The opportunity of using Browser's Developer tools while using [AsyncChromeDriver](https://github.com/ToCSharp/AsyncChromeDriver), [AsyncOperaDriver](https://github.com/ToCSharp/AsyncOperaDriver) is good.  
And you may further use this project. But it opens unnecessary WebSocket server (we need remove this). Someday I do it if you ask in [![Join the chat at https://gitter.im/AsyncWebDriver/Lobby](https://badges.gitter.im/AsyncWebDriver/Lobby.svg)](https://gitter.im/AsyncWebDriver/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
  
   


### BrowsersDevToolsExample

Example of using Browser's Developer tools while using [AsyncChromeDriver](https://github.com/ToCSharp/AsyncChromeDriver), [AsyncOperaDriver](https://github.com/ToCSharp/AsyncOperaDriver) and [AsyncFirefoxDriver](https://github.com/ToCSharp/AsyncWebDriver/tree/master/AsyncFirefoxDriver).  

Look video: https://youtu.be/dNMuoeDmEME  
Run [built Example in release tab](https://github.com/ToCSharp/BrowsersDevToolsExample/releases).   

When we open Chrome or Opera BrowserDevTools, WebSocket server starts to proxy WS traffic. So your firewall will ask you for permission. By default WebSocket server and http proxy server for DevTools frontend accepts only local connections. So it is safe.

[![Join the chat at https://gitter.im/AsyncWebDriver/Lobby](https://badges.gitter.im/AsyncWebDriver/Lobby.svg)](https://gitter.im/AsyncWebDriver/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[AsyncChromeDriver](https://github.com/ToCSharp/AsyncChromeDriver) connects to Chrome DevTools through WebSocket, so we can open WS proxy and route AsyncChromeDriver and one more Chrome window with DevTools through this proxy. They will work in parallel.  

```csharp
   asyncChromeDriver = new AsyncChromeDriver(new ChromeDriverConfig().SetDoOpenBrowserDevTools());
   webDriver = new WebDriver(asyncChromeDriver);
   await asyncChromeDriver.Connect();
```

Then we can open one more Chrome DevTools in Opera for example.

```csharp
   asyncOperaDriver = new AsyncOperaDriver();
   await asyncOperaDriver.Connect();
   await asyncOperaDriver.Navigation.GoToUrl(asyncChromeDriver.GetBrowserDevToolsUrl());
```  
  
Http traffic for Chrome DevTools goes from first Chrome window, but we can also proxy it to change some DevTools frontend files (take them from local dir).  
To do it you can save http traffic first. 
```csharp
   asyncChromeDriver = new AsyncChromeDriver(
       new ChromeDriverConfig()
       .SetDoOpenBrowserDevTools()
       .SetWSProxyConfig(new ChromeWSProxyConfig { HTTPServerSaveRequestedFiles = true })
       );
   browsersToClose.Add(asyncChromeDriver);
   webDriver = new WebDriver(asyncChromeDriver);
   await asyncChromeDriver.Connect();
```  

Then change some files. Or take DevTools frontend from chromium project. And enable http proxy.  
It will try to find file in local dir, or load it from Chrome.  
```csharp
   var dir = tbDevToolsFilesDir.Text;
   if (string.IsNullOrWhiteSpace(dir)) dir = Directory.GetCurrentDirectory();
   dir = dir.TrimEnd('\\');
   if (dir.EndsWith("\\devtools")) dir = Path.GetDirectoryName(dir);

   asyncChromeDriver = new AsyncChromeDriver(
       new ChromeDriverConfig()
       .SetDoOpenBrowserDevTools()
       .SetWSProxyConfig(new ChromeWSProxyConfig { DevToolsFilesDir = dir, HTTPServerTryFindRequestedFileLocaly = true })
       );
   browsersToClose.Add(asyncChromeDriver);
   webDriver = new WebDriver(asyncChromeDriver);
   await asyncChromeDriver.Connect();
```  

Try to save http traffic localy. Open inspector.js file, search for "className":"Network.NetworkPanel", change "order" to 7 near it, save.  
Then open AsyncChromeDriver as shown in last code. NetworkPanel will be the first.

<br/>

#### Developer tools for opened Developer tools
Developer tools opens in full Chrome window, so you can open Developer tools for opened Developer tools. :)
Or in code: 
```csharp
   asyncChromeDriver = new AsyncChromeDriver(new ChromeDriverConfig().SetDoOpenBrowserDevTools());
   asyncChromeDriver.BrowserDevToolsConfig = new ChromeDriverConfig().SetDoOpenBrowserDevTools();
   browsersToClose.Add(asyncChromeDriver);
   webDriver = new WebDriver(asyncChromeDriver);
   await asyncChromeDriver.Connect();
   tbDevToolsRes.Text = "opened";
```

<br/>

#### BrowserDevTools as AsyncWebDriver
asyncChromeDriver.BrowserDevTools is AsyncChromeDriver, so we can interact with BrowserDevTools as AsyncWebDriver with some difficulties, because tabs behind shadowRoots. Look [example](https://github.com/ToCSharp/BrowsersDevToolsExample/blob/6ba69c0de25387a7a09a2fb67ad6bb54e212fe68/BrowsersDevToolsExample/MainWindow.xaml.cs#L470):  
```csharp
  var wd = new WebDriver(asyncChromeDriver.BrowserDevTools);
  var el = await wd.ExecuteScript(shadowFind + "return recursiveFindById(document, 'tab-sources', [])[0];") as AsyncWebElement;
  await el.Click();
```
Firefox BrowserDevTools also accessible, but here difficulties because they are in XUL:  
```csharp
  await asyncFirefoxDriver.BrowserDevTools.JavaScriptExecutor
    .ExecuteScript("frames[0].document.getElementById('toolbox-tab-netmonitor').click();");
```
