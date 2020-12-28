using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;

namespace MonoGame.Entities
{
    /// <summary>
    ///     Interface of entity class
    /// </summary>
    public interface IEntity : ICollisionActor, IActorTarget, IDisposable
    {
        bool IsDestroyed { get; set; }
        void Update(GameTime gameTime) {}
        void Draw(SpriteBatch spriteBatch) {}   
    }
}