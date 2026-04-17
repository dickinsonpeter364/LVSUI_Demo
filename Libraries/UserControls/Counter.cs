namespace LVS3
{
    public class CountDisplay : uscCounter
    {
        public int CounterType = 0;
        public string CounterText = "";
        public int CounterPLC = 0;
        public int CValue = -1;

        public CountDisplay(string countertext, int countervalue, int counterplc, int countertype) : base(countertext, counterplc, countertype)
        {
            base.CounterValue(countervalue);
            this.CounterType = countertype;
            this.CounterText = countertext;
            this.CounterPLC = counterplc;
            this.CValue = countervalue;
        }


        public void Value(int value)
        {
            base.CounterValue(value);
            CValue = value;
        }

        public void Increment(int value)
        {
            base.CounterIncrement(value);
            CValue += value;
        }
    }
}
