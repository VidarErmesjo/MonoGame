using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace MonoGame.Weapons
{
    interface IWeaponManager
    {
        T AddWeapon<T>(T weapon) where T : IWeapon;
    }

    public class WeaponManager // ArmsManager?
    {
        private bool isDisposed;

        private readonly MonoGame _game;
        private readonly Bag<IWeapon> _weapons;

        public IEnumerable<IWeapon> Weapons => _weapons;

        public WeaponManager(MonoGame game)
        {
            _game = game;
            _weapons = new Bag<IWeapon>();
        }

        public T AddWeapon<T>(T weapon) where T: Weapon
        {
            _weapons.Add(weapon);
            return weapon;
        }

        public void Update(GameTime gameTime)
        {
            foreach(var weapon in _weapons)
            {
                weapon.Update(gameTime);
                /*if(entity.IsDestroyed)
                    _entities.Remove(entity);*/
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.Additive,
                samplerState: SamplerState.PointClamp,
                transformMatrix: _game.GameManager.Camera.GetViewMatrix());

            foreach(var weapon in _weapons)
                weapon.Draw(spriteBatch);

            spriteBatch.End();
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

        protected virtual void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
            {
                _game.Dispose();
            }

            isDisposed = true;
        }

        ~WeaponManager()
        {
            this.Dispose(false);
        }
    }
}