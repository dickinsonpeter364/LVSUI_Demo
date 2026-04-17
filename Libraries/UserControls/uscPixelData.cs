namespace LVS3
{
    public partial class uscPixelData : UserControl
    {

        public uscPixelData()
        {
            InitializeComponent();
        }

        public void pointData(int x, int y, int v)
        {
            try
            {
                this.lblX.Text = x.ToString();
                this.lblY.Text = y.ToString();
                if (v >= 0)
                    this.lblScale.Text = v.ToString();
                else
                    this.lblScale.Text = "-";
            }
            catch
            {
                this.lblX.Text = "-";
                this.lblY.Text = "-";
                this.lblScale.Text = "-";
            }
        }

        public void uscPixelData_Load(object sender, EventArgs e)
        {
            this.lblX.Text = "-";
            this.lblY.Text = "-";
            this.lblScale.Text = "-";
        }

        public void SetToNull()
        {
            this.lblX.Text = "-";
            this.lblY.Text = "-";
            this.lblScale.Text = "-";
        }
    }
}
