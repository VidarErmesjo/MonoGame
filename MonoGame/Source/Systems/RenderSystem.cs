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
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.NonPremultiplied,
                samplerState: SamplerState.PointClamp,
                transformMatrix: MonoGame.camera.GetViewMatrix());

            var direction = new Vector2(
                (float) System.Math.Sin(MonoGame.rotation),
                (float) -System.Math.Cos(MonoGame.rotation));
            direction.Normalize();

            MonoGame.animatedSprite.RenderDefinition.Rotation = direction.ToAngle();
            MonoGame.animatedSprite.Render(_spriteBatch);

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
                        weaponComponent.origin.X,
                        weaponComponent.origin.Y,
                        weaponComponent.destination.X,
                        weaponComponent.destination.Y,
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
                        weaponComponent.origin.X,
                        weaponComponent.origin.Y,
                        weaponComponent.destination.X,
                        weaponComponent.destination.Y,
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