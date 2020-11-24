using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;

namespace MonoGame.Tools
{
    public class AnimatedSpriteLoader
    {
        private readonly AnimatedSprite _animatedSprite;
        public AnimatedSpriteLoader(ContentManager content, Vector2 position, Vector2 scale)
        {
            AnimationDefinition animationDefinition = content.Load<AnimationDefinition>("Sprites/ShitspriteAnimation");
            Texture2D spriteSheet = content.Load<Texture2D>("Sprites/Shitsprite");
                _animatedSprite = new AnimatedSprite(
                    spriteSheet,
                    animationDefinition,
                    position);
            _animatedSprite.RenderDefinition.SpriteEffect = SpriteEffects.FlipVertically;
            _animatedSprite.RenderDefinition.Scale = scale;
            _animatedSprite.RenderDefinition.Origin = new Vector2(
                _animatedSprite.CurrentFrame.frame.Width * 0.5f,
                _animatedSprite.CurrentFrame.frame.Height * 0.5f);

            //return _animatedSprite;
        }
    }
}