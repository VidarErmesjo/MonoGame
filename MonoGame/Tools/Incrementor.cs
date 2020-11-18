namespace Tools
{
    public class Incrementor
    {
        private static int _count;

        public Incrementor(int count = 0)
        {
            _count = count;
        }

        public int Tick()
        {
            _count++;
            return _count;
        }

        public void Reset(int count = 0)
        {
            _count = count;
        }
    }
}