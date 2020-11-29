using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Entities.Systems
{// Render
    public class WeatherSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly OrthographicCamera _camera;
        private ComponentMapper<RaindropComponent> _raindropComponentMapper;

        public WeatherSystem()
            : base(Aspect.All(typeof(RaindropComponent)))
        {
            _spriteBatch = new SpriteBatch(Core.GraphicsDeviceManager.GraphicsDevice);
            _camera = Core.Camera;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _raindropComponentMapper = mapperService.GetMapper<RaindropComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                transformMatrix: _camera.GetViewMatrix());

            foreach(var entity in ActiveEntities)
            {
                RaindropComponent raindropComponent = _raindropComponentMapper.Get(entity);

                _spriteBatch.FillRectangle(
                    raindropComponent.Position,
                    new Size2(
                        raindropComponent.Size,
                        raindropComponent.Size),
                    Color.LightBlue);
            }

            _spriteBatch.End();
        }        
    }
}