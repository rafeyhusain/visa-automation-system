namespace Visa.Auomation.App
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            string url = "data/login.html"; // or a full URL like "http://example.com/login"
            string json = File.ReadAllText("data/login.json");

            var filler = new WebFormAutoFiller();
            filler.Fill(url, json);
        }
    }
}
