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
        private ComponentMapper<SuperSprite> _spriteMapper;

        public CollisionSystem() : base(Aspect.All(typeof(SuperSprite), typeof(Collision)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _spriteMapper = mapperService.GetMapper<SuperSprite>();
        }

        public override void Update(GameTime gameTime)
        {
            Core.CollisionComponent = new CollisionComponent(new RectangleF(
                            Core.Camera.Position.X,
                            Core.Camera.Position.Y,
                            Core.VirtualResolution.Width,
                            Core.VirtualResolution.Height));

            foreach(int entity in ActiveEntities)
            {
                SuperSprite sprite = _spriteMapper.Get(entity);
                Core.CollisionComponent.Insert(sprite);
            }
            Core.CollisionComponent.Update(gameTime);
        } 
    }
}