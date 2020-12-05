using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
//using MonoGame.Extended.Entities;
//using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities.Systems
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private ComponentMapper<WeaponComponent> _weaponMapper;
        private ComponentMapper<AsepriteSprite> _spriteMapper;

        public RenderSystem() : base(Aspect.All(typeof(AsepriteSprite), typeof(WeaponComponent)))
        {
            _spriteBatch = new SpriteBatch(Core.GraphicsDeviceManager.GraphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _weaponMapper = mapperService.GetMapper<WeaponComponent>();
            _spriteMapper = mapperService.GetMapper<AsepriteSprite>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.Additive,
                samplerState: SamplerState.PointClamp,
                transformMatrix: Core.Camera.GetViewMatrix());

            /*var direction = new Vector2(
                (float) System.Math.Sin(MonoGame.rotation),
                -(float) System.Math.Cos(MonoGame.rotation));
            direction.Normalize();*/

            foreach(var entity in ActiveEntities)
            {
                WeaponComponent weapon = _weaponMapper.Get(entity);
                AsepriteSprite sprite = _spriteMapper.Get(entity);

                sprite.Draw(_spriteBatch);

                if(weapon.isCharging)
                {
                    _spriteBatch.DrawLine(
                        weapon.origin.X,
                        weapon.origin.Y,
                        weapon.destination.X,
                        weapon.destination.Y,
                        new Color
                        {
                            R = 0,
                            G = 255,
                            B = 0,
                            A = (byte) weapon.charge
                        },
                        weapon.charge);
                }
                else if(weapon.charge > 0.0f)
                {
                    _spriteBatch.DrawLine(
                        weapon.origin.X,
                        weapon.origin.Y,
                        weapon.destination.X,
                        weapon.destination.Y,
                        new Color
                        {
                            R = 255,
                            G = 0,
                            B = 0,
                            A = (byte) weapon.charge
                        },
                        weapon.charge);
                }
            }

            _spriteBatch.End();
        }
    }
}