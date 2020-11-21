using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;


namespace MonoGame.Extended.Entities.Systems
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private ComponentMapper<TestComponent> _testComponentMapper;

        public RenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(Sprite), typeof(Transform2), typeof(TestComponent)))
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
            _testComponentMapper = mapperService.GetMapper<TestComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            var transformMatrix = MonoGame.camera.GetViewMatrix();

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.NonPremultiplied,
                samplerState: SamplerState.PointClamp,
                transformMatrix: transformMatrix);

            foreach(var entity in ActiveEntities)
            {
                TestComponent testComponent = _testComponentMapper.Get(entity);

                // LAZER
                if(testComponent.isCharging)
                {
                    _spriteBatch.DrawLine(
                        MonoGame.camera.Center.X,
                        MonoGame.camera.Center.Y,
                        testComponent.destination.X + MonoGame.camera.Position.X,
                        testComponent.destination.Y + MonoGame.camera.Position.Y,
                        Color.Green,
                        testComponent.charge);
                }
                else if(testComponent.charge > 0.0f)
                {
                    _spriteBatch.DrawLine(
                        MonoGame.camera.Center.X,
                        MonoGame.camera.Center.Y,
                        testComponent.destination.X + MonoGame.camera.Position.X,
                        testComponent.destination.Y + MonoGame.camera.Position.Y,
                        new Color
                        {
                            R = 255,
                            G = 0,
                            B = 0,
                            A = (byte) testComponent.charge
                        },
                        testComponent.charge);
                }

                Sprite sprite = _spriteMapper.Get(entity);
                Transform2 transform = _transformMapper.Get(entity);
                _spriteBatch.Draw(sprite, transform);
            }

            _spriteBatch.End();
        }
    }
}