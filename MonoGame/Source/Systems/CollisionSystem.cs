using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Aseprite;
using MonoGame.Components;
using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Entities.Systems
{
    public class CollisionSystem : EntityUpdateSystem
    {
        private ComponentMapper<AsepriteSprite> _spriteMapper;
        private ComponentMapper<Collision> _collisionMapper;
        private ComponentMapper<ActorComponent> _actorMapper;

        public CollisionSystem() : base(Aspect.All(typeof(AsepriteSprite), typeof(Collision)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _spriteMapper = mapperService.GetMapper<AsepriteSprite>();
            _collisionMapper = mapperService.GetMapper<Collision>();
            _actorMapper = mapperService.GetMapper<ActorComponent>();

            Core.CollisionComponent.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(int entity in ActiveEntities)
            {
                AsepriteSprite sprite = _spriteMapper.Get(entity);
                ActorComponent actor = _actorMapper.Get(entity);
                //Collision collision = _collisionMapper.Get(entity);

                //collision.Torso.Position = actor.Position - sprite.Origin - Vector2.One;
                //sprite.Bounds.Position = actor.Position - sprite.Origin - Vector2.One;

                /// !!!! Do multiple RectangleF's from Aseprite slices?? (Head, Body etc.)
                //sprite.Update(gameTime);
                //if(!Core.CollisionComponent.Contains(sprite))
                //sprite.Position = actor.Position;
                //sprite.Update(gameTime); // ?
                Core.CollisionComponent.Insert(sprite);
            }
            Core.CollisionComponent.Update(gameTime);
        }       
    }
}