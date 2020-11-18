namespace  MonoGame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class Camera
    {
        public Vector2 Position;
        public Rectangle Bounds;

        public Camera(float x = 0.0f, float y = 0.0f, int w = 1, int h = 0)
        {
            Bounds = new Rectangle(0, 0, w, h);
            Position.X = x;
            Position.Y = y;
        }
    }
}