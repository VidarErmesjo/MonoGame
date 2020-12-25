using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Extended;
using System;

namespace MonoGame.Entities
{
    /*public interface IEntity : ICollisionActor, IDisposable
    {

    }*/

    public abstract class Entity : IActorTarget, ICollisionActor, IDisposable
    {
        protected bool isDisposed = false;

        public bool IsCollidable { get; protected set; }
        public bool IsDestroyed { get; private set; }

        protected Entity()
        {
            IsCollidable = false;
            IsDestroyed = false;
        }

        public abstract Vector2 Position { get; set; }
        public abstract Vector2 Velocity { get; set; }
        public abstract Vector2 Origin { get; protected set; }
        public abstract float Rotation { get; set; }
        public abstract Vector2 Scale { get; set; }
        public abstract Color Color { get; set; }
        public abstract IShapeF Bounds { get; set; }
        public abstract RectangleF BoundingBox { get; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Dispose(bool disposing);

        public virtual void OnCollision(CollisionEventArgs collisionEventArgs)
        {   // CollisionComponent
        }

        public virtual void OnCollision(CollisionInfo collisionInfo)
        {   // CollisionWorld
        }

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Entity()
        {
            Dispose(false);
        } 
    }
}