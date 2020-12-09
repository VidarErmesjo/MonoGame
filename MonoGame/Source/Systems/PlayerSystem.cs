using System;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using MonoGame.Components;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame
{
    public class PlayerSystem : EntityUpdateSystem
    {
        private ComponentMapper<PlayerComponent> _playerMapper;
        private ComponentMapper<SuperSprite> _spriteMapper;

        public PlayerSystem() : base(Aspect.All(typeof(PlayerComponent), typeof(SuperSprite)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _playerMapper = mapperService.GetMapper<PlayerComponent>();
            _spriteMapper = mapperService.GetMapper<SuperSprite>();
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            foreach(int entity in ActiveEntities)
            {
                PlayerComponent player = _playerMapper.Get(entity);
                SuperSprite sprite = _spriteMapper.Get(player.Id);

                sprite.Position = player.Position;
                //sprite.Bounds.Position = sprite.Position - sprite.Origin * sprite.Scale;// - sprite.PenetrationVector;// - Vector2.One;

                player.Velocity = (sprite.PenetrationVector != Vector2.Zero) ?
                    player.Velocity -sprite.PenetrationVector : 
                    player.Velocity * 0.5f;

                //player.Position += (sprite.PenetrationVector != Vector2.Zero) ? -spr : player.Velocity;
                player.Position += player.Velocity;// - sprite.PenetrationVector;
                Core.Camera.Move(player.Velocity);

                sprite.Play((player.Velocity == Vector2.Zero ? "Idle" : "Walk"));
                sprite.Update(gameTime);
            }
        }
    }
}