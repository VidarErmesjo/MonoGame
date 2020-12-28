using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;

namespace MonoGame.Entities
{
    /// <summary>
    ///     Base NPC (Non Playable Character) class
    /// </summary>
    public class Actor : Entity
    {
        public Actor(AsepriteDocument asepriteDocument)
        :   base(asepriteDocument)
        {
            this.IsCollidable = true;
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Borders
            if(this.Position.X <= 0f || this.Position.X >= 3840 - _sprite.Width)
                this.Velocity = new Vector2(-this.Velocity.X, this.Velocity.Y);
            if(this.Position.Y <= 0f || this.Position.Y >= 2160 - _sprite.Height)
                this.Velocity = new Vector2(this.Velocity.X, -this.Velocity.Y);
            
            if(!_isColliding)
            {
                //this.Velocity *= 0.5f;
                this.Position += this.Velocity * deltaTime;
                this.Origin = new Vector2(_sprite.Width * 0.5f, _sprite.Height * 0.5f);
                this.Rotation += 0.01f;
                if(this.Rotation >= 2 * MathF.PI)
                    this.Rotation = 0f;

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
        }
    }
}