using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using MonoGame.Components;

namespace MonoGame.Extended.Entities.Systems
{
    public class ControllerSystem : EntityUpdateSystem
    {
        private ComponentMapper<AsepriteSprite> _spriteMapper;
        private ComponentMapper<PlayerComponent> _playerMapper;

        public ControllerSystem() : base(Aspect.All(typeof(AsepriteSprite), typeof(PlayerComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _spriteMapper = mapperService.GetMapper<AsepriteSprite>();
            _playerMapper = mapperService.GetMapper<PlayerComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Core.KeyboardState;
            float elapsedSeconds = gameTime.GetElapsedSeconds();

            Vector2 angle = Core.Camera.ScreenToWorld(
                new Vector2(Core.MouseState.X, Core.MouseState.Y)) - Core.Camera.Center;
            angle.Normalize();

            Vector2 direction = Vector2.Zero;
            direction.X = Core.KeyboardState.IsKeyDown(Keys.Left) ? -1.0f :
                Core.KeyboardState.IsKeyDown(Keys.Right) ? 1.0f : 0.0f;  
            direction.Y = Core.KeyboardState.IsKeyDown(Keys.Up) ? -1.0f:
                Core.KeyboardState.IsKeyDown(Keys.Down) ? 1.0f : 0.0f;  
            direction.Normalize();

            //if(!direction.IsNaN())
            //    Core.Camera.Move(direction * elapsedSeconds * Core.SpriteSize * Core.SpriteScale);

            // Move to PlayerSystem. Have GamePadSystem / Component instead?
            foreach(var entity in ActiveEntities)
            {
                AsepriteSprite sprite = _spriteMapper.Get(entity);
                PlayerComponent player = _playerMapper.Get(entity);

                //sprite.Velocity = !direction.IsNaN() ? direction * Core.SpriteSize * Core.SpriteScale : Vector2.Zero;
                player.Velocity = !direction.IsNaN() ?
                    direction * Core.SpriteSize * Core.SpriteScale * elapsedSeconds : 
                    Vector2.Zero; //-player.Velocity * elapsedSeconds; //Vector2.Zero;
                //Core.Camera.Move(player.Velocity * elapsedSeconds);
                sprite.SpriteEffect = SpriteEffects.FlipVertically;
                //sprite.Play((!direction.IsNaN() ? "Walk" : "Idle"));
                //sprite.Position = Core.Camera.Center;
                sprite.Rotation = angle.ToAngle();
            }
        }
    }
}