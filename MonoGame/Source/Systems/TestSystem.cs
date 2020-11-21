using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;


namespace MonoGame.Extended.Entities.Systems
{
    public class TestSystem : EntityUpdateSystem //, IUpdateSystem, IDrawSystem, ISystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private MouseState _mouseState;
        private ComponentMapper<TestComponent> _componentMapper;

        public TestSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(TestComponent)))
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _mouseState = new MouseState();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _componentMapper = mapperService.GetMapper<TestComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            _mouseState = Mouse.GetState();
            foreach(var entity in ActiveEntities)
            {
                var component = _componentMapper.Get(entity);
                if(_mouseState.LeftButton == ButtonState.Pressed)
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

                        //component.origin = new Vector2(_camera.Center.X - _camera.Position.X, _camera.Center.Y - _camera.Position.Y);
                        component.destination = new Vector2(_mouseState.X, _mouseState.Y);
                    }
                }
                else
                {
                    if(component.isCharging)
                        component.toggleIsCharging();

                    if(!component.isCharging)
                    {
                        if(component.charge > 0.0f)
                            component.charge -= 0.25f;
                        else
                            component.charge = 0.0f;
                    }
                }
            }
        }
    }
}