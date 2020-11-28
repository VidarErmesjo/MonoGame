using System.IO;
using MonoGame.Extended.Collections;

namespace MonoGame.Tools
{
    public class AtmosphericRandom
    {
        /// <sumary>
        /// Based on atmospheric measurements.
        /// </sumary>
        private readonly Bag<float> _numbers = new Bag<float>();
        private readonly long _seed;
        private int _index { get; set; }

        public AtmosphericRandom(long seed = long.MaxValue)
        {
            string[] numbers = File.ReadAllText("Content/Sequences/Unique.txt").Split('\n');
            _seed = seed == long.MaxValue ? System.DateTime.Now.Ticks : long.MaxValue;

            _numbers.Add(0f);
            foreach(string number in numbers)
            {
                _numbers.Add(2 * float.Parse(number) / numbers.Length);
            }

            _index = (int) System.Math.Abs(_seed % (_numbers.Count - 1));
        }

        public float Next()
        {
            return _numbers[_index < _numbers.Count - 1 ? ++_index : 0];
        }
    }
}