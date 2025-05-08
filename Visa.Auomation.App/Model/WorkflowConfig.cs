using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

public class WorkflowConfig
{
    private readonly int _timeoutSeconds;
    private readonly int _pollingMilliseconds;
    private List<string> _proxies;
    private string _workflow;

    private IWebDriver _driver;
    private WebDriverWait _wait;

    private readonly Logger _logger;

    public WorkflowConfig()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        _pollingMilliseconds = config.GetSection("TimeoutSeconds").Get<int>();
        _pollingMilliseconds = config.GetSection("PollingIntervalMilliseconds").Get<int>();
        _proxies = config.GetSection("Proxies").Get<List<string>>()!;
        _workflow = config.GetSection("Workflow").Get<string>()!;

        _logger = new Logger();

        _driver = GetChromeDriver();
        _wait = GetWebDriverWait();
    }

    private IWebDriver GetChromeDriver()
    {
        var service = this.GetService();

        var options = this.GetOptions();

        // Create the driver using the service
        var driver = new ChromeDriver(service, options);

        // Websites can detect default Selenium behavior.
        // Use this trick to Remove webdriver property in JS
        // Or use undetected ChromeDriver versions or tools like Stealth WebDriver
        ((IJavaScriptExecutor)driver).ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");

        return driver;
    }

    private WebDriverWait GetWebDriverWait()
    {
        var wait = new WebDriverWait(new SystemClock(), _driver, TimeSpan.FromSeconds(_timeoutSeconds), TimeSpan.FromMilliseconds(_pollingMilliseconds));

        return wait;
    }

    private ChromeOptions GetOptions()
    {
        // Set Chrome options (if any)
        var options = new ChromeOptions();
        options.AddArgument("--disable-gpu");
        options.AddArgument("--no-sandbox");
        // options.AddArgument("--headless"); // Uncomment if you want to run headless
        
        options.Proxy = this.GetProxy();

        return options;
    }

    private ChromeDriverService GetService()
    {
        // Create a ChromeDriverService with hidden console window
        var service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true; // Hide the console
        service.SuppressInitialDiagnosticInformation = true; // Optional: suppress startup messages

        return service;
    }

    private Proxy? GetProxy()
    {
        string proxyToUse = _proxies[new Random().Next(_proxies.Count)];

        var proxy = new Proxy { HttpProxy = proxyToUse, SslProxy = proxyToUse };

        return proxy;
    }

    public int TimeoutSeconds => _timeoutSeconds;

    public int PollingMilliseconds => _pollingMilliseconds;
    public string Workflow => _workflow;
    
    public IWebDriver Driver { get => _driver; }
    public WebDriverWait Wait { get => _wait; }

    public Logger Logger => _logger;
}
