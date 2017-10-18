#### BrowsersDevToolsExample

Example of using Browser's Developer tools while using [AsyncChromeDriver](https://github.com/ToCSharp/AsyncChromeDriver), [AsyncOperaDriver](https://github.com/ToCSharp/AsyncOperaDriver) and [AsyncFirefoxDriver](https://github.com/ToCSharp/AsyncWebDriver/tree/master/AsyncFirefoxDriver).  

Run built Example in release tab.  

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