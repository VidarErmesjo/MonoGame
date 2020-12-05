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
        private ComponentMapper<AsepriteSprite> _spriteMapper;

        public ActorSystem() : base(Aspect.All(typeof(ActorComponent), typeof(AsepriteSprite)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _actorMapper = mapperService.GetMapper<ActorComponent>();
            _spriteMapper = mapperService.GetMapper<AsepriteSprite>();
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            foreach(int entity in ActiveEntities)
            {
                ActorComponent actor = _actorMapper.Get(entity);
                AsepriteSprite sprite = _spriteMapper.Get(actor.Id);

                sprite.Position = actor.Position;
                sprite.Bounds.Position = sprite.Position - sprite.Origin * sprite.Scale;// - sprite.PenetrationVector;// - Vector2.One;

                /*actor.Velocity = (sprite.PenetrationVector != Vector2.Zero) ?
                    Vector2.Zero - sprite.PenetrationVector :
                    actor.Velocity;*/

                actor.Position += actor.Velocity * Core.SpriteSize * Core.SpriteScale * elapsedSeconds;

                sprite.Play((actor.Velocity == Vector2.Zero ? "Idle" : "Walk"));
                sprite.Update(gameTime);
            }
        }
    }
}