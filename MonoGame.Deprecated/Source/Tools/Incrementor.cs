namespace MonoGame.Tools
{
    public class Incrementor
    {
        private int _count;

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