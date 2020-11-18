namespace SpriteManager
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpriteFrame
    {
        public Texture2D Texture { get; private set; }
        public Rectangle Shape { get; private set; }
        public Vector2 Scale { get; private set; }
        public bool IsRotated { get; private set; }
        public Vector2 Origin { get; private set; }

        public SpriteFrame(Texture2D texture, Rectangle shape, Vector2 scale, Vector2 pivot, bool isRotated)
        {
            this.Texture = texture;
            this.Shape = shape;
            this.Scale = scale;
            this.Origin = isRotated ? new Vector2(shape.Width * (1 - pivot.Y), shape.Height * pivot.X)
                                    : new Vector2(shape.Width * pivot.X, shape.Height * pivot.Y);
            this.IsRotated = isRotated;
        }
    }
}