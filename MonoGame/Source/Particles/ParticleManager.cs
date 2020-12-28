using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

using MonoGame.Effects;

namespace MonoGame.Particles
{
    interface IParticleManager : IDisposable
    {
        T AddParticle<T>(T particle) where T : IParticle;
    }

    public class ParticleManager : IParticleManager
    {
        private bool isDisposed = false;

        private readonly Bag<IParticle> _particles;
        private readonly MonoGame _game;
        private RenderTarget2D screenTexture;

        public IEnumerable<Particle> Particles => _particles.OfType<Particle>();

        public ParticleManager(MonoGame game)
        {
            _game = game;
            _particles = new Bag<IParticle>();
        }

        public T AddParticle<T>(T particle) where T : IParticle
        {   // Return id?
            _particles.Add(particle);
            return particle;
        }

        public void Update(GameTime gameTime)
        {
            foreach(var particle in _particles)
                particle.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Effect effect = null, Matrix? transformMatrix = null)
        {
            // GaussianBlur too slow and bugged?
            screenTexture = new RenderTarget2D(
                _game.GraphicsDevice,
                _game.GameManager.VirtualResolution.Width,
                _game.GameManager.VirtualResolution.Height);
            /*_graphicsDevice.SetRenderTarget(screenTexture);
            _graphicsDevice.Clear(Color.Transparent);*/
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: BlendState.Additive,
                samplerState: null,
                effect: effect,
                transformMatrix: transformMatrix);

            foreach(var particle in _particles)
                particle.Draw(spriteBatch);

            spriteBatch.End();
            
            /*_graphicsDevice.SetRenderTarget(null);

            var result = _effectManager.GaussianBlur.PerformGaussianBlur(screenTexture, spriteBatch);

            _graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            spriteBatch.Draw(result, result.Bounds, Color.White);
            spriteBatch.End();*/
        }

        public void UnloadContent()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
            {
                _game.Dispose();

                foreach(var particle in _particles)
                    particle.Dispose();

                screenTexture?.Dispose();       
            }

            isDisposed = true;
        }

        ~ParticleManager()
        {
            this.Dispose(false);
        }
    }
}