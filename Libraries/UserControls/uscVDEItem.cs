using System.ComponentModel;

namespace LVS3
{
    public partial class uscVDEItem : UserControl
    {
        public bool IsNumeric { get => lids.IsNumeric; }
        public bool FIXED { get => lblFT.Text.Trim().ToUpper() == "FIXED" ? true : false; }
        private LabelItemDataSetup lids = null;
        public string PlaceHolder { get => lblPH.Text; }
        public string FieldType { get => lblFT.Text; }
        public string VariableName { get => lblVDE.Text; }
        public int Repeat { get => lids.Repeat; }
        private int repeatIndex = 1;
        public List<string> VDEDataList = new List<string>();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VDEData { get => lids.VDE_DATA; set => setval(value); }

        private int counter = 0;

        private void setval(string val)
        {
            lblData.Text = val;
            lids.VDE_DATA = val;
        }

        public uscVDEItem(LabelItemDataSetup ldis)
        {
            InitializeComponent();
            lids = ldis;
            lblPH.Text = ldis.PLACE_HOLDER;
            lblVDE.Text = ldis.VARIABLE_NAME;
            lblFT.Text = ldis.FIELD_TYPE;
            VDEData = ldis.VDE_DATA;
            lblData.Text = ldis.VDE_DATA;
            if (ldis.VDE_DATA_LIST.Count > 0)
                VDEDataList = ldis.VDE_DATA_LIST;
        }

        public bool MoveToNextVDE()
        {
            bool retVal = true;
            try
            {
                if (VDEDataList.Count == 0)
                    return false;

                if (lblData.InvokeRequired)
                {
                    lblData.Invoke((MethodInvoker)delegate
                    {
                        if (counter < VDEDataList.Count - 1)
                        {
                            counter++;
                            VDEData = VDEDataList[counter];
                            if (repeatIndex < Repeat)
                                repeatIndex++;
                            else
                                repeatIndex = 1;
                        }
                        else
                            VDEData = VDEDataList[VDEDataList.Count - 1];
                        lblData.Text = VDEData;
                        lblData.Refresh();
                        Invalidate();
                    });
                }
                else
                {
                    if (counter < VDEDataList.Count - 1)
                    {
                        counter++;
                        VDEData = VDEDataList[counter];
                        if (repeatIndex < Repeat)
                            repeatIndex++;
                        else
                            repeatIndex = 1;
                    }
                    else
                        VDEData = VDEDataList[VDEDataList.Count - 1];
                    lblData.Text = VDEData;
                    lblData.Refresh();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "MoveToNextVDE() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }


        public bool MoveToIndex(int x)
        {
            bool retVal = true;
            try
            {
                if (VDEDataList.Count == 0)
                    return false;

                if (lblData.InvokeRequired)
                {
                    lblData.Invoke((MethodInvoker)delegate
                    {
                        counter = x - 1;
                        VDEData = VDEDataList[counter];
                        if (repeatIndex < Repeat)
                            repeatIndex++;
                        else
                            repeatIndex = 1;
                        lblData.Text = VDEData;
                        lblData.Refresh();
                        Invalidate();
                    });
                }
                else
                {
                    counter = 0;
                    while (counter < (x - 1))
                    {
                        counter++;
                        VDEData = VDEDataList[counter];
                        if (repeatIndex < Repeat)
                            repeatIndex++;
                        else
                            repeatIndex = 1;
                        lblData.Text = VDEData;
                    }
                    lblData.Refresh();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "MoveToIndex() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }


        public bool MoveToPreviousVDE()
        {
            bool retVal = true;
            try
            {
                if (VDEDataList.Count == 0)
                    return false;

                if (lblData.InvokeRequired)
                {
                    lblData.Invoke((MethodInvoker)delegate
                    {
                        if (counter > 0)
                        {
                            counter--;
                            VDEData = VDEDataList[counter];
                        }
                        else
                            VDEData = VDEDataList[0];
                        lblData.Text = VDEData;
                        lblData.Refresh();
                        Invalidate();
                    });
                }
                else
                {
                    if (counter > 0)
                    {
                        counter--;
                        VDEData = VDEDataList[counter];
                    }
                    else
                        VDEData = VDEDataList[0];
                    lblData.Text = VDEData;
                    lblData.Refresh();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "MoveToNextVDE() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        public bool Reset()
        {
            bool retVal = true;
            try
            {
                counter = 0;
                if (VDEDataList.Count == 0)
                    return false;

                if (lblData.InvokeRequired)
                {
                    lblData.Invoke((MethodInvoker)delegate
                    {
                        VDEData = VDEDataList[0];
                        lblData.Text = VDEData;
                        repeatIndex = 1;
                        lblData.Refresh();
                        Invalidate();
                    });
                }
                else
                {
                    VDEData = VDEDataList[0];
                    lblData.Text = VDEData;
                    lblData.Refresh();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "Reset() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        public bool RemoveFirstItem()
        {
            bool retVal = true;
            try
            {
                counter = 0;
                if (VDEDataList.Count == 0)
                    return false;

                if (lblData.InvokeRequired)
                {
                    lblData.Invoke((MethodInvoker)delegate
                    {
                        VDEData = VDEDataList[1];
                        VDEDataList.RemoveAt(0);
                        lblData.Text = VDEData;
                        repeatIndex = 1;
                        lblData.Refresh();
                        Invalidate();
                    });
                }
                else
                {
                    VDEData = VDEDataList[1];
                    VDEDataList.RemoveAt(0);
                    lblData.Text = VDEData;
                    repeatIndex = 1;
                    lblData.Refresh();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "Reset() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        public bool ResetCount(int numLabels)
        {
            bool retVal = true;
            try
            {
                string dataToUse = "";
                counter = 0;
                if (VDEDataList.Count == 0)
                    return false;
                if (lblData.InvokeRequired)
                {
                    lblData.Invoke((MethodInvoker)delegate
                    {
                        if (VDEDataList.Count != 0)
                        {
                            dataToUse = VDEDataList[0];
                            VDEDataList.Clear();
                            for (int x = 0; x < numLabels; x++)
                            {
                                VDEDataList.Add(dataToUse);
                            }
                        }
                        VDEData = VDEDataList[0];
                        lblData.Text = VDEData;
                        repeatIndex = numLabels;
                        lblData.Refresh();
                        Invalidate();
                    });
                }
                else
                {
                    if (VDEDataList.Count != 0)
                    {
                        dataToUse = VDEDataList[0];
                        VDEDataList.Clear();
                        for (int x = 0; x < numLabels; x++)
                        {
                            VDEDataList.Add(dataToUse);
                        }
                    }
                    VDEData = VDEDataList[0];
                    lblData.Text = VDEData;
                    repeatIndex = numLabels;
                    lblData.Refresh();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "ResetCount() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }
    }
}
