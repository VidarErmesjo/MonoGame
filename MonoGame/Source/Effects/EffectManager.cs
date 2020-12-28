using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGame.Effects
{
    public class EffectManager : IDisposable
    {
        private bool isDisposed = false;

        private readonly MonoGame _game;
        private GaussianBlur _gaussianBlur;

        public GaussianBlur GaussianBlur => _gaussianBlur;

        public EffectManager(MonoGame game)
        {
            _game = game;
        }

        public void LoadContent()
        {
            _gaussianBlur = new GaussianBlur(_game);
        }

        public void Initialize()
        {
            _gaussianBlur.Initialize(); //?
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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
                _gaussianBlur.Dispose();
            }

            isDisposed = true;
        }

        ~EffectManager()
        {
            this.Dispose(false);
        }
    }
}