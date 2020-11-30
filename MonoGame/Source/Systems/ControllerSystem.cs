using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;

namespace MonoGame.Extended.Entities.Systems
{
    public class ControllerSystem : EntityUpdateSystem
    {
        private readonly OrthographicCamera _camera;
        private ComponentMapper<AsepriteSprite> _asepriteMapper;

        public ControllerSystem() : base(Aspect.All(typeof(AsepriteSprite)))
        {
            _camera = Core.Camera;
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
            direction.X = keyboardState.IsKeyDown(Keys.Left) ? -1.0f :
                keyboardState.IsKeyDown(Keys.Right) ? 1.0f : 0.0f;  
            direction.Y = keyboardState.IsKeyDown(Keys.Up) ? -1.0f:
                keyboardState.IsKeyDown(Keys.Down) ? 1.0f : 0.0f;  
            direction.Normalize();

            if(!direction.IsNaN())
                _camera.Move(direction * elapsedSeconds * Core.SpriteSize * Core.SpriteScale);

            foreach(var entity in ActiveEntities)
            {
                AsepriteSprite player = _asepriteMapper.Get(entity);
                player.SpriteEffect = SpriteEffects.FlipVertically;
                player.Play((!direction.IsNaN() ? "Walk" : "Idle"));
                player.Position = _camera.Center;
                player.Update(gameTime);
            }
        }
    }
}