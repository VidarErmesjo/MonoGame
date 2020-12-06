using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonoGame.Aseprite
{
    public class Key
    {
        public int Frame { get; set; }
        public Rectangle Bounds { get; set; }
        public Point Pivot { get; set; }
    }
}