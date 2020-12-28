using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace MonoGame.Entities
{
    interface IEntityManager
    {
        T AddEntity<T>(T entity) where T : IEntity;
    }

    public class EntityManager : IEntityManager
    {
        private readonly MonoGame _game;
        private readonly Bag<IEntity> _entities;

        public IEnumerable<Entity> Entities => _entities.OfType<Entity>();
        public IEnumerable<Actor> Actors => _entities.OfType<Actor>();
        public IEnumerable<Player> Players => _entities.OfType<Player>();

        public int Count => _entities.Count;

        public EntityManager(MonoGame game = null)
        {
            _game = game;
            _entities = new Bag<IEntity>();
        }

        public T AddEntity<T>(T entity) where T : IEntity
        {
            _entities.Add(entity);
            return entity;
        }

        public void Update(GameTime gameTime)
        {
            foreach(var entity in _entities)
            {
                entity.Update(gameTime);
                if(entity.IsDestroyed)
                    _entities.Remove(entity);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Effect effect = null, Matrix? transformMatrix = null)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: BlendState.Additive,
                samplerState: SamplerState.PointClamp,
                effect: effect,
                transformMatrix: transformMatrix);

            foreach(var entity in _entities)
                if(!entity.IsDestroyed)
                    entity.Draw(spriteBatch);

            spriteBatch.End();
        }

        public void UnloadContent()
        {
            _game?.Dispose();
            
            foreach(var entity in _entities)
                entity.Dispose();
        }
    }
}