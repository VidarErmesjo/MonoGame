using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using MonoGame.Components;

namespace MonoGame.Extended.Entities.Systems
{
    public class ControllerSystem : EntityUpdateSystem
    {
        private ComponentMapper<AsepriteSprite> _asepriteMapper;

        public ControllerSystem() : base(Aspect.All(typeof(AsepriteSprite), typeof(Player)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _asepriteMapper = mapperService.GetMapper<AsepriteSprite>();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Core.KeyboardState;
            float elapsedSeconds = gameTime.GetElapsedSeconds();


            Vector2 direction = new Vector2(0.0f, 0.0f);
            direction.X = Core.KeyboardState.IsKeyDown(Keys.A) ? -1.0f :
                Core.KeyboardState.IsKeyDown(Keys.D) ? 1.0f : 0.0f;  
            direction.Y = Core.KeyboardState.IsKeyDown(Keys.W) ? -1.0f:
                Core.KeyboardState.IsKeyDown(Keys.S) ? 1.0f : 0.0f;  
            direction.Normalize();

            if(!direction.IsNaN())
                Core.Camera.Move(direction * elapsedSeconds * Core.SpriteSize * Core.SpriteScale);

            foreach(var entity in ActiveEntities)
            {
                AsepriteSprite player = _asepriteMapper.Get(entity);
                player.SpriteEffect = SpriteEffects.FlipVertically;
                player.Play((!direction.IsNaN() ? "Walk" : "Idle"));
                player.Position = Core.Camera.Center;
                player.Update(gameTime);
            }
        }
    }
}