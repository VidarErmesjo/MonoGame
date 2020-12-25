using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoGame.Aseprite
{
    public class Slice
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public List<Key> Keys { get; set; }

        public Slice()
        {
            Keys = new List<Key>();
        }
    }
}