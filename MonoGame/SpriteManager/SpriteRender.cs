namespace SpriteManager
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Tools;

    public class SpriteRender
    {
        private const float ClockwiseNinetyDegreeRotation = (float)(Math.PI / 2.0f);

        private SpriteBatch _spriteBatch;

        public SpriteRender(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        // <param name="position">This should be where you want the pivot point of the sprite image to be rendered.</param>
        public void Draw(SpriteFrame sprite, Vector2 position, Color color, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            Vector2 origin = sprite.Origin;
            if(sprite.IsRotated)
            {
                rotation -= ClockwiseNinetyDegreeRotation;
                switch(spriteEffects)
                {
                    case SpriteEffects.FlipHorizontally:
                        spriteEffects = SpriteEffects.FlipVertically;
                        break;
                    case SpriteEffects.FlipVertically:
                        spriteEffects = SpriteEffects.FlipHorizontally;
                        break;
                    default:
                        break;
                }
            }
            switch(spriteEffects)
            {
                case SpriteEffects.FlipHorizontally:
                    origin.X = sprite.Shape.Width - origin.X;
                    break;
                case SpriteEffects.FlipVertically:
                    origin.Y = sprite.Shape.Height - origin.Y;
                    break;
                default:
                    break;
            }

            _spriteBatch.Draw(
                texture: sprite.Texture,
                position: position,
                sourceRectangle: sprite.Shape,
                color: color,
                rotation: rotation,
                origin: origin,
                scale: new Vector2(scale, scale),
                effects: spriteEffects,
                layerDepth: 0.0f
            );
        }
    }
}