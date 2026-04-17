namespace LVS3
{
    public partial class uscCounter : UserControl
    {
        private int m_counterValue = 0;
        private int m_counterType = 0;


        //public int CounterType
        //{
        //    get { return m_counterType; }
        //}

        //public int Count
        //{
        //    get { return m_counterValue; }
        //}

        public uscCounter(string text, int plcid, int countertype)
        {
            InitializeComponent();
            this.lblCount.Text = "0";
            this.m_counterValue = 0;
            this.lblTxt.Text = text;
            this.m_counterType = countertype;
        }

        internal void CounterValue(int value)
        {
            if (lblCount.InvokeRequired)
            {
                lblCount.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    this.lblCount.Text = value.ToString();
                    this.m_counterValue = value;
                });
            }
            else
            {
                this.lblCount.Text = value.ToString();
                this.m_counterValue = value;
            }
        }

        internal void CounterIncrement(int value)
        {
            if (lblCount.InvokeRequired)
            {
                lblCount.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    this.m_counterValue += value;
                    this.lblCount.Text = m_counterValue.ToString();
                });
            }
            else
            {
                this.m_counterValue += value;
                this.lblCount.Text = m_counterValue.ToString();
            }
        }

        internal void CounterClear()
        {
            if (lblCount.InvokeRequired)
            {
                lblCount.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    this.lblCount.Text = 0.ToString();
                    this.m_counterValue = 0;
                });
            }
            else
            {
                this.lblCount.Text = 0.ToString();
                this.m_counterValue = 0;
            }
        }
    }
}
