using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

public partial class Workflow
{
    private readonly WorkflowConfig _config;


    public Workflow()
    {
        _config = new WorkflowConfig();
    }

    public void Run(string? path = null)
    {
        if (path == null)
        {
            path = _config.Workflow;
        }

        try
        {
            new DriverManager().SetUpDriver(new ChromeConfig());

            if (!File.Exists(path))
            {
                _config.Logger.Log($"Workflow file not found: {path}");
                return;
            }

            List<WebPage>? pages = GetPages(path)!;

            foreach (var page in pages)
            {
                var pageWorkflow = new WorkflowPage(_config);
                pageWorkflow.Run(page);
            }
        }
        finally
        {
            _config.Driver.Quit();
        }
    }

    private List<WebPage>? GetPages(string path)
    {
        List<WebPage>? pages = null;

        try
        {
            var json = File.ReadAllText(path);
            pages = JsonSerializer.Deserialize<List<WebPage>>(json)!;
        }
        catch (Exception ex)
        {
            _config.Logger.Log("Error parsing workflow JSON", ex);
        }

        return pages;
    }
}
