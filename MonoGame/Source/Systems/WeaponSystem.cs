using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Entities.Systems
{
    public class WeaponSystem : EntityUpdateSystem
    {
        private ComponentMapper<WeaponComponent> _componentMapper;

        public WeaponSystem() : base(Aspect.All(typeof(WeaponComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _componentMapper = mapperService.GetMapper<WeaponComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                var weapon = _componentMapper.Get(entity);
                if(Core.MouseState.LeftButton == ButtonState.Pressed)
                {
                    if(!weapon.isCharging)
                    {
                        weapon.toggleIsCharging();
                        weapon.charge = 0.0f;
                    }
                    else
                    {
                        weapon.charge += 1.0f;
                        if(weapon.charge > 255.0f)
                            weapon.charge = 255.0f;  

                        weapon.origin = Core.Camera.Center;
                        weapon.destination = Core.Camera.ScreenToWorld(
                            new Vector2(
                                Core.MouseState.X,
                                Core.MouseState.Y));
                    }
                }
                else
                {
                    if(weapon.isCharging)
                        weapon.toggleIsCharging();

                    if(!weapon.isCharging)
                    {
                        if(weapon.charge > 0.0f)
                            weapon.charge -= 1.0f;
                        else
                            weapon.charge = 0.0f;
                    }
                }
            }
        }
    }
}