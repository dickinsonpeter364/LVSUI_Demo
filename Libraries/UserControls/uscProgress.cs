using static LVS3.Delegates;

namespace UserControls
{
    public partial class uscProgress : UserControl
    {
        public static ProgressHandler AMD;

        public uscProgress()
        {
            InitializeComponent();
        }

        public void Max(int maximum)
        {
            this.Visible = true;
            label1.Visible = true;
            pb1.Visible = true;
            pb1.Maximum = maximum;
            pb1.Value = 0;
            label1.Text = "0";
            pb1.Invalidate();
            pb1.Refresh();
            label1.Invalidate();
            label1.Refresh();
            this.Refresh();
        }

        public void Val(int val)
        {
            if (val <= pb1.Maximum)
            {
                pb1.Value = val;
                label1.Text = val.ToString();
                pb1.Invalidate();
                pb1.Refresh();
                label1.Invalidate();
                label1.Refresh();
                this.Visible = true;
                this.Refresh();
            }
            else if (val > pb1.Maximum)
            {
                this.Visible = false;
                //pb1.Value = pb1.Maximum;
                //label1.Text = pb1.Maximum.ToString();
                //pb1.Invalidate();
                //pb1.Refresh();
                //label1.Invalidate();
                //label1.Refresh();
                this.Refresh();
            }
            else
            {
                this.Visible = true;
                this.Refresh();
            }
        }
    }
}
