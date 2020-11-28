using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Entities.Systems
{// Render
    public class WeatherSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly GraphicsDevice _graphicsDevice;
        private ComponentMapper<RaindropComponent> _raindropComponentMapper;

        public WeatherSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(RaindropComponent)))
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _graphicsDevice = graphicsDevice;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _raindropComponentMapper = mapperService.GetMapper<RaindropComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach(var entity in ActiveEntities)
            {
                RaindropComponent raindropComponent = _raindropComponentMapper.Get(entity);

                _spriteBatch.FillRectangle(
                    raindropComponent.Position,
                    new Size2(raindropComponent.Size,
                    raindropComponent.Size),
                    Color.LightBlue);
            }
            _spriteBatch.End();
        }        
    }
}