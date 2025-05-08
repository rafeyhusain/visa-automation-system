
public class Logger
{
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
}