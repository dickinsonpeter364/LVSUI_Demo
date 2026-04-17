using static LVS3.Enums;

namespace LVS3
{
    public partial class uscCounterGroup : UserControl
    {
        private IDataManager DataManager;
        private IUtilityFunctions UtilityFunctions;

        public uscCounterGroup(IDataManager dataManager, IUtilityFunctions utilityFunctions)
        {
            InitializeComponent();
            DataManager = dataManager;
            UtilityFunctions = utilityFunctions;
        }
        /// <summary>
        /// Zeroes all counters in list
        /// </summary>
        public void Reset()
        {
            foreach (Control c in tlpCounters.Controls)
                if ((CountDisplay)c is CountDisplay)
                {
                    CountDisplay cntr = (CountDisplay)c;
                    cntr.CounterClear();
                }
        }


        public void AddCounter(CountDisplay counter)
        {
            tlpCounters.Controls.Add(counter, 0, tlpCounters.RowStyles.Count - 1);

            if (tlpCounters.Controls.Count == 1)
            {
                tlpCounters.RowStyles.RemoveAt(0);
                tlpCounters.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tlpCounters.SetCellPosition(counter, new TableLayoutPanelCellPosition(0, 0));
                counter.Dock = DockStyle.Fill;
            }
            else
            {
                tlpCounters.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tlpCounters.SetCellPosition(counter, new TableLayoutPanelCellPosition(0, tlpCounters.RowStyles.Count - 1));
                counter.Dock = DockStyle.Fill;
            }
            //for (int x = 0; x < tlpCounters.RowCount; x++)
            //    tlpCounters.RowStyles[x] = new RowStyle(SizeType.Percent, 50F);
        }

        public void Update(int countertype, int value)
        {
            var values = UtilityFunctions.GetValues<CounterTypes>();

            foreach (Control c in tlpCounters.Controls)
                if ((CountDisplay)c is CountDisplay)
                {
                    CountDisplay cd = (CountDisplay)c;
                    if (cd.CounterType == countertype)
                        cd.Value(value);
                }
        }
        public void Increment(int countertype, int value)
        {
            var values = UtilityFunctions.GetValues<CounterTypes>();

            foreach (Control c in tlpCounters.Controls)
            {
                if ((CountDisplay)c is CountDisplay)
                {
                    CountDisplay cd = (CountDisplay)c;
                    if (cd.CounterType == countertype)
                    {
                        cd.Increment(value);
                        break;
                    }
                }
            }
        }

        public List<CounterValue> GetValues()
        {
            List<CounterValue> retVal = new List<CounterValue>();
            //var values = UtilityFunctions.GetValues<CounterTypes>();

            foreach (Control c in tlpCounters.Controls)
                if ((CountDisplay)c is CountDisplay)
                {
                    CountDisplay cd = (CountDisplay)c;
                    CounterValue cv = new CounterValue(cd.CounterText, cd.CValue);
                    retVal.Add(cv);
                }
            return retVal;
        }
    }

    public class CounterValue
    {
        public string Description = "";
        public int Value = -1;

        public CounterValue(string description, int value)
        {
            this.Description = description;
            this.Value = value;
        }
    }
}
