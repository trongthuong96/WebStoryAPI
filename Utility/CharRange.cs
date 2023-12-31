namespace Utility
{
    public class CharRange
    {
        private int startIndex;
        private int length;

        public int StartIndex
        {
            get => startIndex;
            set => startIndex = value;
        }

        public int Length
        {
            get => length;
            set => length = value;
        }

        public CharRange(int startIndex, int length)
        {
            this.startIndex = startIndex;
            this.length = length;
        }

        public bool IsInRange(int index)
        {
            return startIndex <= index && index <= startIndex + length - 1;
        }

        public int GetEndIndex()
        {
            return startIndex + length - 1;
        }
    }
}

