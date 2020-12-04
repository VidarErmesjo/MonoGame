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
        private ComponentMapper<AsepriteSprite> _spriteMapper;

        public PlayerSystem() : base(Aspect.All(typeof(PlayerComponent), typeof(AsepriteSprite)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _playerMapper = mapperService.GetMapper<PlayerComponent>();
            _spriteMapper = mapperService.GetMapper<AsepriteSprite>();
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            foreach(int entity in ActiveEntities)
            {
                PlayerComponent player = _playerMapper.Get(entity);
                AsepriteSprite sprite = _spriteMapper.Get(player.Id);

                sprite.Position = player.Position;
                sprite.Bounds.Position = sprite.Position;// - sprite.Origin;// - Vector2.One;

                player.Velocity = (sprite.PenetrationVector != Vector2.Zero) ?
                    Vector2.Zero - sprite.PenetrationVector :
                    player.Velocity;

                player.Position += player.Velocity;
                Core.Camera.Move(player.Velocity);

                sprite.Play((player.Velocity == Vector2.Zero ? "Idle" : "Walk"));
                sprite.Update(gameTime);
            }
        }
    }
}