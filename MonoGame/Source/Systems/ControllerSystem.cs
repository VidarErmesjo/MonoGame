using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;

namespace MonoGame.Extended.Entities.Systems
{
    public class ControllerSystem : EntityUpdateSystem
    {
        private ComponentMapper<AsepriteSprite> _asepriteMapper;
        public ControllerSystem()
            : base(Aspect.All(typeof(AsepriteSprite)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _asepriteMapper = mapperService.GetMapper<AsepriteSprite>();
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

            foreach(var entity in ActiveEntities)
            {
                AsepriteSprite player = _asepriteMapper.Get(entity);
                player.SpriteEffect = SpriteEffects.FlipVertically;
                player.Play((!direction.IsNaN() ? "Walk" : "Idle"));
                player.Position = MonoGame.camera.Center;
                player.Update(gameTime);
            }
        }
    }
}