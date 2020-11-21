namespace MonoGame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

// Depricated
    public class Player
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        public float charge { get; set; }
        public bool isCharging { get; private set; }
        public Vector2 dischargePoint { get; set; }
        //public Extended.Shapes.Polygon dechargePoint { get; set; }

        public Player()
        {
            Position = new Vector2(0.0f, 0.0f);
            Velocity = new Vector2(0.0f, 0.0f);
            charge = 0.0f;
            isCharging = false;
            dischargePoint = new Vector2(0.0f, 0.0f);
        }

        public void toggleIsCharging()
        {
            isCharging = !isCharging;
        }
    }
}