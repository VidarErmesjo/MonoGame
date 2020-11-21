using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Entities.Systems
{
    public class ControllerSystem : EntityUpdateSystem
    {
        public ControllerSystem()
            : base(Aspect.All())
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
        }

        public override void Update(GameTime gameTime)
        {
            var direction = new Vector2(0.0f, 0.0f);
            direction.X = MonoGame.keyboardState.IsKeyDown(Keys.Left) ? -1.0f :
                MonoGame.keyboardState.IsKeyDown(Keys.Right) ? 1.0f : 0.0f;  
            direction.Y = MonoGame.keyboardState.IsKeyDown(Keys.Up) ? -1.0f :
                MonoGame.keyboardState.IsKeyDown(Keys.Down) ? 1.0f : 0.0f;  
            direction.Normalize();

            if(!direction.IsNaN())
                MonoGame.camera.Move(direction * gameTime.ElapsedGameTime.Milliseconds);   
        }
    }
}