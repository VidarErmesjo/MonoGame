using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Weapons
{
    public interface IWeapon : IDisposable
    {
        void Update(GameTime gameTime) {}
        void Draw(SpriteBatch spriteBatch) {}
    }
}