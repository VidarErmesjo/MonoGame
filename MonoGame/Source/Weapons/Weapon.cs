using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Weapons
{
    public class Weapon : IWeapon
    {
        private bool isDisposed;

        public Weapon()
        {
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
            {

            }

            isDisposed = true;
        }

        ~Weapon()
        {
            this.Dispose(false);
        }
    }
}