using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Entities.Systems
{
    public class WeaponSystem : EntityUpdateSystem
    {
        private ComponentMapper<WeaponComponent> _componentMapper;

        public WeaponSystem()
            : base(Aspect.All(typeof(WeaponComponent)))
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
                var component = _componentMapper.Get(entity);
                if(MonoGame.mouseState.LeftButton == ButtonState.Pressed)
                {
                    if(!component.isCharging)
                    {
                        component.toggleIsCharging();
                        component.charge = 0.0f;
                    }
                    else
                    {
                        component.charge += 1.0f;
                        if(component.charge > 255.0f)
                            component.charge = 255.0f;  

                        component.origin = MonoGame.camera.Center;
                        component.destination = MonoGame.camera.ScreenToWorld(new Vector2(MonoGame.mouseState.X, MonoGame.mouseState.Y));
                    }
                }
                else
                {
                    if(component.isCharging)
                        component.toggleIsCharging();

                    if(!component.isCharging)
                    {
                        if(component.charge > 0.0f)
                            component.charge -= 1.0f;
                        else
                            component.charge = 0.0f;
                    }
                }
            }
        }
    }
}