using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Particles
{
    /// <summary>
    ///     Interface for particle class
    /// </summary>
    /// <remarks>
    ///     Represents a single ParticleEffect (not a single particle)
    /// </remarks>
    public interface IParticle :  IDisposable
    {
        void Draw(SpriteBatch spriteBatch) {}
        void Update(GameTime gameTime) {}
        void Dispose(bool disposinig) {}
    }
}