using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Entities;

namespace MonoGame.Collisions
{
    public class CollisionManager : IDisposable
    {
        private bool isDisposed;

        private CollisionComponent _collisionComponent;
        private readonly MonoGame _game;
        //private Vector2 _position;

        public CollisionManager(MonoGame game)
        {
            _game = game;
        }
        
        public void Initialize()
        {
            _collisionComponent = new CollisionComponent(
                new RectangleF(
                    _game.GameManager.Camera.Position.X,
                    _game.GameManager.Camera.Position.Y,
                    _game.GameManager.VirtualResolution.Width,
                    _game.GameManager.VirtualResolution.Height));
                
            foreach(var entity in _game.EntityManager.Entities)
                _collisionComponent.Insert(entity);            
        }

        /// <summary>
        ///     Refresh boundary and reload entities
        /// </summary>
        /// <remarks>
        ///     Has penalty costs to CPU
        /// </remarks>
        private void Refresh()
        {
            _collisionComponent = new CollisionComponent(
                new RectangleF(
                    _game.GameManager.Camera.Position.X,
                    _game.GameManager.Camera.Position.Y,
                    _game.GameManager.VirtualResolution.Width,
                    _game.GameManager.VirtualResolution.Height));
                
            foreach(var entity in _game.EntityManager.Entities)
                _collisionComponent.Insert(entity);

            //_position = _entityManager.Players.First().Position;
        }

        public void Update(GameTime gameTime)
        {
            _collisionComponent.Update(gameTime);

            //var position = _entityManager.Players.First().Position;
            //if(position != _position)
                this.Refresh();
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
                _collisionComponent.Dispose();
                _game.Dispose();
            }

            isDisposed = true;

        }

        ~CollisionManager()
        {
            this.Dispose(false);
        }
    }
}