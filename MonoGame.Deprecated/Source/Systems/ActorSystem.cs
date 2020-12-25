using System;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using MonoGame.Components;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame
{
    public class ActorSystem : EntityUpdateSystem
    {
        private ComponentMapper<ActorComponent> _actorMapper;
        private ComponentMapper<SuperSprite> _spriteMapper;

        public ActorSystem() : base(Aspect.All(typeof(ActorComponent), typeof(SuperSprite)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _actorMapper = mapperService.GetMapper<ActorComponent>();
            _spriteMapper = mapperService.GetMapper<SuperSprite>();
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            foreach(int entity in ActiveEntities)
            {
                ActorComponent actor = _actorMapper.Get(entity);
                SuperSprite sprite = _spriteMapper.Get(actor.Id);

                sprite.Position = actor.Position;
                sprite.Bounds.Position = sprite.Position - sprite.Origin * sprite.Scale;

                /*actor.Velocity = (sprite.PenetrationVector != Vector2.Zero) ?
                    actor.Velocity - sprite.PenetrationVector :
                    actor.Velocity * 0.25f;*/

                //if(actor.Velocity.LengthSquared() < 0.1f)
                //    actor.Velocity = Vector2.Zero;
 
                actor.Position += actor.Velocity * Core.SpriteSize * Core.SpriteScale * elapsedSeconds;
                sprite.Rotation += sprite.Rotation < 0f || sprite.Rotation > 2 * Math.PI ? -sprite.Rotation : 0.01f;

                sprite.Play((actor.Velocity.LengthSquared() < 0.1f ? "Idle" : "Walk"));
                sprite.Update(gameTime);
            }
        }
    }
}