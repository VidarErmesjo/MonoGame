using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Aseprite;
using MonoGame.Components;
using System.Collections.Generic;

namespace MonoGame.Extended.Entities.Systems
{
    public class CollisionSystem : EntityUpdateSystem
    {
        private Dictionary<int, IShapeF> _colliders;
        private CollisionComponent _collisionComponent;
        private ComponentMapper<AsepriteSprite> _asepriteComponentMapper;
        private ComponentMapper<Collision> _colliderComponentMapper;
        private ComponentMapper<CollisionComponent> _collisionComponentMapper;

        public CollisionSystem() : base(Aspect.All(typeof(AsepriteSprite), typeof(Collision)))
        {
            _colliders = new Dictionary<int, IShapeF>();
            _collisionComponent = new CollisionComponent(new RectangleF(
                0,
                0,
                Core.VirtualResolution.Width,
                Core.VirtualResolution.Height));        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _asepriteComponentMapper = mapperService.GetMapper<AsepriteSprite>();
            _colliderComponentMapper = mapperService.GetMapper<Collision>();
            _collisionComponentMapper = mapperService.GetMapper<CollisionComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            _colliders.Clear();
            foreach(var entity in ActiveEntities)
            {
                AsepriteSprite aseprite = _asepriteComponentMapper.Get(entity);
                Collision collider = _colliderComponentMapper.Get(entity);
                CollisionComponent collision = _collisionComponentMapper.Get(entity);

                aseprite.Bounds.Position = aseprite.Position;
                //_colliders.Add(entity, aseprite.Bounds);

                _collisionComponent.Insert(aseprite);
            }
            _collisionComponent.Update(gameTime);

            /*foreach(var record in _colliders)
            {
                foreach(var current in _colliders)
                {
                    if(record.Value.Intersects(current.Value) && record.Key != current.Key)
                        System.Console.WriteLine("Impact! {0}{1}, {2}{3}", record.Key, record.Value, current.Key, current.Value);
                }
            }*/
        }        
    }
}