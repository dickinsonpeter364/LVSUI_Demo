using System.ComponentModel;

namespace LVS3
{

    public partial class uscImageRotater : UserControl
    {

        public int ValueRotated = 0;

        public uscImageRotater()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Selectable, false);
        }

        public delegate void UpDownChangedHandler(int rotatedvalue);
        public event UpDownChangedHandler Rotater_ValueChanged;

        private void button_value_changed(object sender, EventArgs e)
        {
            ValueRotated += 1;
            if (this.Rotater_ValueChanged != null)
                Rotater_ValueChanged(ValueRotated);
            if (ValueRotated > 3)
                ValueRotated = 0;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.Enabled)
                cmdRotate.BackColor = Color.Teal;
            else
                cmdRotate.BackColor = Color.LightGray;
            base.OnEnabledChanged(e);
        }


        public void SetValue(int value)
        {
            button_value_changed(cmdRotate, null);
        }

        public void Reset()
        {
            ValueRotated = 0;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SetColor
        { get => cmdRotate.BackColor;
            set { cmdRotate.BackColor = value; } 
        }
        
    }
}
