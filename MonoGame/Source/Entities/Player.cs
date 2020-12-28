using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Weapons;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;

namespace MonoGame.Entities
{
    /// <summary>
    ///     Base player class
    /// </summary>
    public class Player : Entity
    {
        private Vector2 _direction = Vector2.Zero;
        //private Vector2 _angle = Vector2.Zero;
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
            //_angle = (Core.Camera.ScreenToWorld(new Vector2(_mouseState.X, _mouseState.Y)) - this.Position).NormalizedCopy();

            _keyboardState = Keyboard.GetState();
            _direction.X = _keyboardState.IsKeyDown(Keys.Left) ? -1.0f :
                _keyboardState.IsKeyDown(Keys.Right) ? 1.0f : 0.0f;  
            _direction.Y = _keyboardState.IsKeyDown(Keys.Up) ? -1.0f:
                _keyboardState.IsKeyDown(Keys.Down) ? 1.0f : 0.0f;  

            if(!_isColliding)
            {
                this.Velocity = !_direction.NormalizedCopy().IsNaN() ? _direction * this._sprite.Width * this.Scale : Vector2.Zero;
                this.Position += this.Velocity * deltaTime;            
                this.Origin = new Vector2(_sprite.Width * 0.5f, _sprite.Height * 0.5f);
                //this.Rotation = _angle.ToAngle() + MathF.PI;
                this.Color = Color.White;
            }

            _sprite.Play((this.Velocity == Vector2.Zero ? "Idle" : "Walk"));
            _sprite.Update(deltaTime);
            _damageManager.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Render(spriteBatch);

            foreach(var part in _damageManager.Parts)
                foreach(var polygon in this.Slices)
                    if(part.Key == polygon.Key && part.Value > 0f)
                        spriteBatch.DrawPolygon(
                            Vector2.Zero,
                            polygon.Value,
                            new Color { R = 255, G = 0, B = 0, A = (byte) part.Value },
                            part.Value);

            spriteBatch.DrawRectangle((RectangleF) this.Bounds, Color.LightGreen, 1f);
        }   
    }
}