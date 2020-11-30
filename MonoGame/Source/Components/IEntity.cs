using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;

namespace MonoGame
{
    public interface IEntity : ICollisionActor
    {
        public void Draw(SpriteBatch spriteBatch);
        public void Update(GameTime gameTime);
    }
}