using System.ComponentModel;
using System.Drawing.Drawing2D;


namespace LVS3
{

    public class SystemModeLabel : RoundBottomLabel
    {
        public string StatusDescription = "MANUAL MODE";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Mode
        {
            get { return StatusDescription; }
            set
            {
                StatusDescription = value; // (value == Enums.SystemStatus.MANUAL ? "MANUAL MODE" : value == Enums.SystemStatus.TRAINING ? "TRAINING MODE" : "INSPECTION MODE");
                if (this.InvokeRequired)
                {
                    this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        this.Text = StatusDescription;
                        this.Refresh();
                    });
                }
                else
                {
                    this.Text = StatusDescription;
                    this.Refresh();
                }
            }
        }

        public SystemModeLabel()
        {
            this.Text = StatusDescription;
        }
    }

    public class RoundRadioButton : RadioButton
    {
        public RoundRadioButton()
        {
            this.DoubleBuffered = true;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            base.MouseMove -= this.Ctl_MouseMove;
            base.BackColorChanged -= this.BackColor_Changed;
            base.MouseMove += this.Ctl_MouseMove;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            base.FlatAppearance.BorderColor = this.BackColor;
        }
        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundButton : Button
    {
        public RoundButton()
        {
            base.BackColor = Color.LightGray;
            this.DoubleBuffered = true;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            base.MouseMove -= this.Ctl_MouseMove;
            base.MouseMove += this.Ctl_MouseMove;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            if (this.BackColor != Color.Transparent)
                base.FlatAppearance.BorderColor = this.BackColor;
        }

        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.Enabled)
                        this.BackColor = Color.Teal;
                    else
                        this.BackColor = Color.LightGray;
                    base.OnEnabledChanged(e);
                });
            }
            else
            {
                if (this.Enabled)
                    this.BackColor = Color.Teal;
                else
                    this.BackColor = Color.LightGray;
                base.OnEnabledChanged(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius,
                                 Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundBottomButton : Button
    {
        public RoundBottomButton()
        {
            base.BackColor = Color.LightGray;
            this.DoubleBuffered = true;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            base.MouseMove -= this.Ctl_MouseMove;
            base.MouseMove += this.Ctl_MouseMove;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            if (this.BackColor != Color.Transparent)
                base.FlatAppearance.BorderColor = this.BackColor;
        }

        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.Enabled)
                        this.BackColor = Color.Teal;
                    else
                        this.BackColor = Color.LightGray;
                    base.OnEnabledChanged(e);
                });
            }
            else
            {
                if (this.Enabled)
                    this.BackColor = Color.Teal;
                else
                    this.BackColor = Color.LightGray;
                base.OnEnabledChanged(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y);
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundLeftButton : Button
    {

        public RoundLeftButton()
        {
            this.DoubleBuffered = true;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            base.MouseMove -= this.Ctl_MouseMove;
            base.BackColorChanged -= this.BackColor_Changed;
            base.MouseMove += this.Ctl_MouseMove;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            base.FlatAppearance.BorderColor = this.BackColor;
        }

        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.Enabled)
                        this.BackColor = Color.Teal;
                    else
                        this.BackColor = Color.LightGray;
                    base.OnEnabledChanged(e);
                });
            }
            else
            {
                if (this.Enabled)
                    this.BackColor = Color.Teal;
                else
                    this.BackColor = Color.LightGray;
                base.OnEnabledChanged(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height);
                GraphPath.AddLine(Rect.Width, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundRightButton : Button
    {

        public RoundRightButton()
        {
            this.DoubleBuffered = true;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            base.MouseMove -= this.Ctl_MouseMove;
            base.BackColorChanged -= this.BackColor_Changed;
            base.MouseMove += this.Ctl_MouseMove;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            base.FlatAppearance.BorderColor = this.BackColor;
        }

        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.Enabled)
                        this.BackColor = Color.Teal;
                    else
                        this.BackColor = Color.LightGray;
                    base.OnEnabledChanged(e);
                });
            }
            else
            {
                if (this.Enabled)
                    this.BackColor = Color.Teal;
                else
                    this.BackColor = Color.LightGray;
                base.OnEnabledChanged(e);
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.X, Rect.Height);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }


    public class RoundRightTextBox : TextBox
    {

        public RoundRightTextBox()
        {
            this.DoubleBuffered = true;
            base.MouseMove -= this.Ctl_MouseMove;
            base.BackColorChanged -= this.BackColor_Changed;
            base.MouseMove += this.Ctl_MouseMove;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            base.BackColor = this.BackColor;
        }

        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.Enabled)
                        this.BackColor = Color.Teal;
                    else
                        this.BackColor = Color.LightGray;
                    base.OnEnabledChanged(e);
                });
            }
            else
            {
                if (this.Enabled)
                    this.BackColor = Color.Teal;
                else
                    this.BackColor = Color.LightGray;
                base.OnEnabledChanged(e);
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.X, Rect.Height);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }


    public class RoundNoneButton : Button
    {

        public RoundNoneButton()
        {
            this.DoubleBuffered = true;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            base.MouseMove -= this.Ctl_MouseMove;
            base.BackColorChanged -= this.BackColor_Changed;
            base.MouseMove += this.Ctl_MouseMove;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            base.FlatAppearance.BorderColor = this.BackColor;
        }

        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.Enabled)
                        this.BackColor = Color.Teal;
                    else
                        this.BackColor = Color.LightGray;
                    base.OnEnabledChanged(e);
                });
            }
            else
            {
                if (this.Enabled)
                    this.BackColor = Color.Teal;
                else
                    this.BackColor = Color.LightGray;
                base.OnEnabledChanged(e);
            }
        }

    }


    public class RoundLeftBottomButton : Button
    {
        public RoundLeftBottomButton()
        {
            this.DoubleBuffered = true;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            base.MouseMove -= this.Ctl_MouseMove;
            base.BackColorChanged -= this.BackColor_Changed;
            base.MouseMove += this.Ctl_MouseMove;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            base.FlatAppearance.BorderColor = this.BackColor;
        }
        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.Enabled)
                        this.BackColor = Color.Teal;
                    else
                        this.BackColor = Color.LightGray;
                    base.OnEnabledChanged(e);
                });
            }
            else
            {
                if (this.Enabled)
                    this.BackColor = Color.Teal;
                else
                    this.BackColor = Color.LightGray;
                base.OnEnabledChanged(e);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }
        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height);
                GraphPath.AddLine(Rect.Width, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundNoneLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLink { get; set; }

        public RoundNoneLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }

    }

    public class RoundLeftTopLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLink { get; set; }

        public RoundLeftTopLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height);
                GraphPath.AddLine(Rect.Width, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Height, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundRightTopLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLink { get; set; }

        public RoundRightTopLabel()
        {
            this.DoubleBuffered = true;
        }


        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height);
                GraphPath.AddLine(Rect.Width, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.X, Rect.Height);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundRightTopButton : Button
    {

        public RoundRightTopButton()
        {
            this.DoubleBuffered = true;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            base.MouseMove -= this.Ctl_MouseMove;
            base.BackColorChanged -= this.BackColor_Changed;
            base.MouseMove += this.Ctl_MouseMove;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            base.FlatAppearance.BorderColor = this.BackColor;
        }

        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.Enabled)
                        this.BackColor = Color.Teal;
                    else
                        this.BackColor = Color.LightGray;
                    base.OnEnabledChanged(e);
                });
            }
            else
            {
                if (this.Enabled)
                    this.BackColor = Color.Teal;
                else
                    this.BackColor = Color.LightGray;
                base.OnEnabledChanged(e);
            }
        }
        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height);
                GraphPath.AddLine(Rect.Width, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.X, Rect.Height);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundRightBottomButton : Button
    {

        public RoundRightBottomButton()
        {
            this.DoubleBuffered = true;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            base.MouseMove -= this.Ctl_MouseMove;
            base.BackColorChanged -= this.BackColor_Changed;
            base.MouseMove += this.Ctl_MouseMove;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            base.FlatAppearance.BorderColor = this.BackColor;
        }
        protected void Ctl_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.Enabled)
                        this.BackColor = Color.Teal;
                    else
                        this.BackColor = Color.LightGray;
                    base.OnEnabledChanged(e);
                });
            }
            else
            {
                if (this.Enabled)
                    this.BackColor = Color.Teal;
                else
                    this.BackColor = Color.LightGray;
                base.OnEnabledChanged(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }
        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Height, Rect.X, Rect.Y);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class CircularLabel : Label
    {
        private Color old_color;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLink { get; set; }

        public CircularLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath())
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath()
        {
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            try
            {
                int diameter = this.Width > this.Height ? this.Height : this.Width;
                int left = (this.Width / 2) - (diameter / 2);
                RectangleF Rect = new RectangleF(0, 0, diameter, diameter);
                path.AddEllipse(left, 0, diameter, diameter);
                this.Region = new Region(path);
            }
            catch { }
            return path;
        }


        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }
    }

    public class RoundLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLink { get; set; }

        public RoundLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundRightLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLink { get; set; }

        public RoundRightLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.X, Rect.Height);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundLeftLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLink { get; set; }

        public RoundLeftLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height);
                GraphPath.AddLine(Rect.Width, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundTopLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLink { get; set; }

        public RoundTopLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddLine(Rect.Width, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Height, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundLeftBottomLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible )]
        public bool IsLink { get; set; }

        public RoundLeftBottomLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height);
                GraphPath.AddLine(Rect.Width, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundRightBottomLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLink { get; set; }

        public RoundRightBottomLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }
        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Height, Rect.X, Rect.Y);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundBottomLabel : Label
    {
        private Color old_color;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLink { get; set; }

        public RoundBottomLabel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs mevent)
        {
            if (IsLink == true)
            {
                old_color = base.ForeColor;
                base.ForeColor = Color.Cyan;
                Cursor.Current = Cursors.Hand;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsLink == true)
            {
                base.ForeColor = old_color;
                Cursor.Current = Cursors.Default;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsLink == true)
            {
                Cursor.Current = Cursors.Hand;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
                using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    this.Region = new Region(GraphPath);
                    using (Pen pen = new Pen(BackColor, 1.75f))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        e.Graphics.DrawPath(pen, GraphPath);
                    }
                }
            }
            catch { }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y);
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundPanel : Panel
    {
        public RoundPanel()
        {
            this.DoubleBuffered = true;
            base.BackColorChanged -= this.BackColor_Changed;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            if (this.BackColor != Color.Transparent)
                base.BackColor = this.BackColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }
    public class RoundRightPanel : Panel
    {

        public RoundRightPanel()
        {
            this.DoubleBuffered = true;
            base.BackColorChanged -= this.BackColor_Changed;
            base.BackColorChanged += this.BackColor_Changed;
        }
        protected void BackColor_Changed(object sender, EventArgs e)
        {
            if (this.BackColor != Color.Transparent)
                base.BackColor = this.BackColor;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X, Rect.Height);
                GraphPath.AddLine(Rect.X, Rect.Y, Rect.X, Rect.Height);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

    public class RoundLeftPanel : Panel
    {
        public RoundLeftPanel()
        {
            this.DoubleBuffered = true;
            base.BackColorChanged -= this.BackColor_Changed;
            base.BackColorChanged += this.BackColor_Changed;
        }

        protected void BackColor_Changed(object sender, EventArgs e)
        {
            if (this.BackColor != Color.Transparent)
                base.BackColor = this.BackColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, 10))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(base.BackColor, 1.75f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            try
            {
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width, Rect.Y);
                GraphPath.AddLine(Rect.Width, Rect.Y, Rect.Width, Rect.Height);
                GraphPath.AddLine(Rect.Width, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
            }
            catch { }
            return GraphPath;
        }
    }

}
