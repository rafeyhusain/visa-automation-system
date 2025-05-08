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
            var workflow = new Workflow();
            workflow.Run();
            this.Close();
        }
    }
}
