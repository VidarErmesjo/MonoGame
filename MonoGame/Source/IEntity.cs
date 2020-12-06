using MonoGame.Aseprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

using System;

namespace MonoGame
{
    public abstract class IEntity : ICollisionActor, IDisposable
    {
        private bool isDisposed = false;

        public int Id;
        public string Type;
        public string Name;

        public AsepriteSprite sprite;

        public Vector2 Position;
        public Vector2 Velocity;
        public IShapeF Bounds { get; set; } // Collision
        // If I want to go from ECS to Good Old Inheritance (GOI)
        // Must have EntityManager

        abstract public void Draw(SpriteBatch spriteBatch);
        // { sprite.Draw(spriteBatch); }
        abstract public void Update(GameTime gameTime);
        // { sprite.Update(gameTime); }

        abstract public void OnCollision(CollisionEventArgs collisionEventArgs);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
            {
                sprite.Dispose();
            }

            isDisposed = true;
        }

        ~IEntity()
        {
            Dispose(false);
        }
    }
}