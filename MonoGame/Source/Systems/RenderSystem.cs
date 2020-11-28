using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;


namespace MonoGame.Extended.Entities.Systems
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly GraphicsDevice _graphicsDevice;
        private ComponentMapper<WeaponComponent> _weaponComponentMapper;
        private ComponentMapper<AsepriteSprite> _asepriteComponentMapper;

        public RenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(AsepriteSprite), typeof(WeaponComponent)))
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _graphicsDevice = graphicsDevice;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _weaponComponentMapper = mapperService.GetMapper<WeaponComponent>();
            _asepriteComponentMapper = mapperService.GetMapper<AsepriteSprite>();
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

            foreach(var entity in ActiveEntities)
            {
                WeaponComponent weaponComponent = _weaponComponentMapper.Get(entity);
                AsepriteSprite aseprite = _asepriteComponentMapper.Get(entity);

                aseprite.Rotation = direction.ToAngle();
                aseprite.Render(_spriteBatch);

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