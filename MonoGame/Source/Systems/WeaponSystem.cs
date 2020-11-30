using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Extended.Entities.Systems
{
    public class WeaponSystem : EntityUpdateSystem
    {
        private readonly OrthographicCamera _camera;
        private ComponentMapper<WeaponComponent> _componentMapper;

        public WeaponSystem() : base(Aspect.All(typeof(WeaponComponent)))
        {
            _camera = Core.Camera;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _componentMapper = mapperService.GetMapper<WeaponComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Core.MouseState;

            foreach(var entity in ActiveEntities)
            {
                var component = _componentMapper.Get(entity);
                if(mouseState.LeftButton == ButtonState.Pressed)
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

                        component.origin = _camera.Center;
                        component.destination = _camera.ScreenToWorld(
                            new Vector2(
                                mouseState.X,
                                mouseState.Y));
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