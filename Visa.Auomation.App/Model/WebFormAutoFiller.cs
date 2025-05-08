using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

public class WebFormAutoFiller
{
    public void Fill(string url, string json)
    {
        List<WebFormTask> tasks;
        try
        {
            tasks = JsonSerializer.Deserialize<List<WebFormTask>>(json);
        }
        catch (Exception ex)
        {
            Log("JSON Parse Error", ex);
            return;
        }

        new DriverManager().SetUpDriver(new ChromeConfig());

        using IWebDriver driver = new ChromeDriver()
        {
            Url = GetUrl(url)
        };

        try
        {
            driver.Navigate();

            var wait = new WebDriverWait(new SystemClock(), driver, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(500));

            foreach (var task in tasks)
            {
                switch (task.Type)
                {
                    case "fill":
                        foreach (var field in task.Data)
                        {
                            try
                            {
                                wait.Until(d => d.FindElement(By.Id(field.Id)).Displayed);
                                var element = driver.FindElement(By.Id(field.Id));
                                element.Clear();
                                element.SendKeys(field.Value);
                            }
                            catch (Exception ex)
                            {
                                Log($"Field Error: ID '{field.Id}'", ex);
                                return;
                            }
                        }
                        break;

                    case "cloudflare-captcha":
                        Log("Please manually solve the Cloudflare CAPTCHA.");
                        break;

                    case "button-click":
                        foreach (var item in task.Data)
                        {
                            try
                            {
                                wait.Until(d => d.FindElement(By.Id(item.Id)).Displayed);
                                driver.FindElement(By.Id(item.Id)).Click();
                            }
                            catch (Exception ex)
                            {
                                Log($"Button Click Error: ID '{item.Id}'", ex);
                                return;
                            }
                        }
                        break;

                    default:
                        Log($"Unsupported task type: {task.Type}");
                        return;
                }
            }

            Log("Tasks completed successfully.");
        }
        catch (Exception ex)
        {
            Log("Unexpected Error", ex);
        }
        finally
        {
            driver.Quit();
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

    public void Log(string message)
    {
        MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public void Log(string message, Exception ex)
    {
        string fullMessage = message + Environment.NewLine + GetExceptionMessages(ex);
        MessageBox.Show(fullMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private string GetExceptionMessages(Exception ex)
    {
        List<string> messages = new();
        while (ex != null)
        {
            messages.Add(ex.Message + Environment.NewLine);
            ex = ex.InnerException;
        }
        return string.Join(Environment.NewLine, messages);
    }

    public class WebFormTask
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("data")]
        public List<WebFormData> Data { get; set; } = new();
    }

    public class WebFormData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
