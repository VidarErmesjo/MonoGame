using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;

namespace MonoGame.Entities
{
    public class Player : Actor
    {
        private Vector2 _direction = Vector2.Zero;
        private Vector2 _angle = Vector2.Zero;
        private MouseState _mouseState;
        private KeyboardState _keyboardState;

        public Player(AsepriteDocument asepriteDocument)
        : base(asepriteDocument)
        {
            this.IsCollidable = true;
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            _mouseState = Mouse.GetState();
            _angle = Core.Camera.ScreenToWorld(
                new Vector2(_mouseState.X, _mouseState.Y)) - this.Position;
            _angle.Normalize();

            _keyboardState = Keyboard.GetState();
            _direction.X = _keyboardState.IsKeyDown(Keys.Left) ? -1.0f :
                _keyboardState.IsKeyDown(Keys.Right) ? 1.0f : 0.0f;  
            _direction.Y = _keyboardState.IsKeyDown(Keys.Up) ? -1.0f:
                _keyboardState.IsKeyDown(Keys.Down) ? 1.0f : 0.0f;  

            if(!_isColliding)
            {
                this.Velocity = !_direction.NormalizedCopy().IsNaN() ? _direction * Core.SpriteSize * this.Scale : Vector2.Zero;
                this.Position += this.Velocity * deltaTime;            
                this.Origin = new Vector2(_sprite.Width * 0.5f, _sprite.Height * 0.5f);
                this.Rotation = _angle.ToAngle() + MathF.PI;
                this.Color = Color.White;
            }

            _sprite.Play((this.Velocity == Vector2.Zero ? "Idle" : "Walk"));
            _sprite.Update(deltaTime);
            _hitDetectionManager.Update(gameTime);
        }

        /*public override void OnCollision(CollisionInfo collisionInfo)
        {
            this.Color = Color.DarkRed;
            Console.WriteLine("OU");
            base.OnCollision(collisionInfo);
        }*/        
    }
}