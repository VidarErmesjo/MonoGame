using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Entities.Systems
{
    public class WeatherSystem : EntityDrawSystem
    {
        private readonly FastRandom _random;
        private readonly SpriteBatch _spriteBatch;
        private ComponentMapper<RaindropComponent> _raindropComponent;

        public WeatherSystem() : base(Aspect.All(typeof(RaindropComponent)))
        {
            _random = new FastRandom();
            _spriteBatch = new SpriteBatch(Core.GraphicsDeviceManager.GraphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _raindropComponent = mapperService.GetMapper<RaindropComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                transformMatrix: Core.Camera.GetViewMatrix());

            foreach(var entity in ActiveEntities)
            {
                RaindropComponent raindropComponent = _raindropComponent.Get(entity);

                //_spriteBatch.DrawLine(raindropComponent.Position, raindropComponent.ImpactPoint, Color.Red);

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