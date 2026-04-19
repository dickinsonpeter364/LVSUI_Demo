namespace ComMatcherTest
{
    [Serializable]
    public class DataMatrixDefinition : ElementSearchDefinition

    {
        public DataMatrixDefinition(int x, int y, int width, int height)
        : base(x, y, width, height)
        {
            
        }

        public DataMatrixDefinition()
        {
        }
    }

}


