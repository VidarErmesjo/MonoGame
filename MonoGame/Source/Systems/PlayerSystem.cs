using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Entities.Systems
{
    public class PlayerSystem : EntityUpdateSystem
    {
        private ComponentMapper<OrthographicCamera> _cameraMapper;

        public PlayerSystem()
            : base(Aspect.One(typeof(OrthographicCamera)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _cameraMapper = mapperService.GetMapper<OrthographicCamera>();
        }

        public override void Update(GameTime gameTime)
        {
            var direction = new Vector2(0.0f, 0.0f);
            var _keyboardState = Keyboard.GetState();
            direction.X = _keyboardState.IsKeyDown(Keys.Left) ? -1.0f :
                _keyboardState.IsKeyDown(Keys.Right) ? 1.0f : 0.0f;  
            direction.Y = _keyboardState.IsKeyDown(Keys.Up) ? -1.0f :
                _keyboardState.IsKeyDown(Keys.Down) ? 1.0f : 0.0f;  
            direction.Normalize();

            OrthographicCamera camera = _cameraMapper.Get(0);
            if(!direction.IsNaN())
                camera.Move(direction * gameTime.ElapsedGameTime.Milliseconds);
        }
    }
}