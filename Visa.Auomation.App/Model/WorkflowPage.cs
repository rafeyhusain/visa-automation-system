using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public partial class WorkflowPage
{
    private readonly WorkflowConfig _config;
    public WorkflowPage(WorkflowConfig config)
    {
        _config = config;
    }

    public void Run(WebPage page)
    {
        _config.Driver.Url = GetUrl(page.Url);

        try
        {
            _config.Driver.Navigate();

            foreach (var task in page.Tasks)
            {
                switch (task.Type)
                {
                    case "fill":
                        TaskFill(task);
                        break;

                    case "cloudflare-captcha":
                        TaskCloudflare();
                        break;

                    case "button-click":
                        TaskButtonClick(task);
                        break;

                    default:
                        _config.Logger.Log($"Unsupported task type: {task.Type}");
                        return;
                }
            }

            _config.Logger.Log($"Tasks for page [{page.Title}] completed successfully.");
        }
        catch (Exception ex)
        {
            _config.Logger.Log("Unexpected Error", ex);
        }
    }

    private void TaskButtonClick(WebPageTask task)
    {
        foreach (var item in task.Data)
        {
            try
            {
                _config.Wait.Until(d => d.FindElement(By.Id(item.Id)).Displayed);
                _config.Driver.FindElement(By.Id(item.Id)).Click();
            }
            catch (Exception ex)
            {
                _config.Logger.Log($"Button Click Error: ID '{item.Id}'", ex);
                return;
            }
        }
    }

    private void TaskCloudflare()
    {
        //_config.Logger.Log("Please manually solve the Cloudflare CAPTCHA.");
    }

    private void TaskFill(WebPageTask task)
    {
        foreach (var field in task.Data)
        {
            try
            {
                IWebElement element;

                string fieldType = field.Type?.ToLowerInvariant() ?? "textbox";

                if (fieldType == "hidden")
                {
                    element = _config.Driver.FindElement(By.Id(field.Id)); // No wait for visibility
                }
                else
                {
                    _config.Wait.Until(d => d.FindElement(By.Id(field.Id)).Displayed);
                    element = _config.Driver.FindElement(By.Id(field.Id));
                }

                switch (fieldType)
                {
                    case "textbox":
                    case "email":
                    case "password":
                    case "number":
                    case "date":
                    case "text":
                        element.Clear();
                        element.SendKeys(field.Value);
                        break;

                    case "hidden":
                        var js = (IJavaScriptExecutor)_config.Driver;
                        js.ExecuteScript("arguments[0].value = arguments[1];", element, field.Value);
                        break;

                    case "textarea":
                        element.Clear();
                        element.SendKeys(field.Value);
                        break;

                    case "select":
                        var select = new SelectElement(element);
                        try
                        {
                            select.SelectByValue(field.Value);
                        }
                        catch (NoSuchElementException)
                        {
                            select.SelectByText(field.Value);
                        }
                        break;

                    case "checkbox":
                        bool check = bool.TryParse(field.Value, out bool result) && result;
                        if (element.Selected != check)
                            element.Click();
                        break;

                    case "radio":
                        if (!element.Selected)
                            element.Click();
                        break;

                    default:
                        _config.Logger.Log($"Unsupported field type: {field.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _config.Logger.Log($"Field Error: ID '{field.Id}'", ex);
                return;
            }
        }
    }

    private static string GetUrl(string url)
    {
        if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            string fullPath = Path.GetFullPath(url);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("HTML file not found.", fullPath);
            url = "file:///" + fullPath.Replace('\\', '/');
        }
        return url;
    }
}
