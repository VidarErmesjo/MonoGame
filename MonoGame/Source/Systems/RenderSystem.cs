using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;


namespace MonoGame.Extended.Entities.Systems
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private ComponentMapper<WeaponComponent> _weaponComponentMapper;

        public RenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(Sprite), typeof(Transform2), typeof(WeaponComponent)))
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _graphicsDevice = graphicsDevice;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
            _weaponComponentMapper = mapperService.GetMapper<WeaponComponent>();
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
                WeaponComponent weaponComponent = _weaponComponentMapper.Get(entity);
                Sprite sprite = _spriteMapper.Get(entity);
                Transform2 transform = _transformMapper.Get(entity);
                transform.Scale = Vector2.One * 4;
                _spriteBatch.Draw(sprite, transform);

                // LAZER
                if(weaponComponent.isCharging)
                {
                    _spriteBatch.DrawLine(
                        MonoGame.camera.Center.X,
                        MonoGame.camera.Center.Y,
                        weaponComponent.destination.X + MonoGame.camera.Position.X,
                        weaponComponent.destination.Y + MonoGame.camera.Position.Y,
                        new Color
                        {
                            R = 0,
                            G = 255,
                            B = 0,
                            A = (byte) weaponComponent.charge
                        },
                        weaponComponent.charge);
                }
                else if(weaponComponent.charge > 0.0f)
                {
                    _spriteBatch.DrawLine(
                        MonoGame.camera.Center.X,
                        MonoGame.camera.Center.Y,
                        weaponComponent.destination.X + MonoGame.camera.Position.X,
                        weaponComponent.destination.Y + MonoGame.camera.Position.Y,
                        new Color
                        {
                            R = 255,
                            G = 0,
                            B = 0,
                            A = (byte) weaponComponent.charge
                        },
                        weaponComponent.charge);
                }
            }

            _spriteBatch.End();
        }
    }
}