using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace MonoGame.Entities
{
    /// <summary>
    ///     Base entity class
    /// </summary>
    public class Entity : IEntity
    {
        protected bool isDisposed = false;

        protected readonly AnimatedSprite _sprite;
        protected readonly DamageManager _damageManager;
        protected bool _isColliding = false;

        protected Entity(AsepriteDocument asepriteDocument)
        {
            _sprite = new AnimatedSprite(asepriteDocument);
            _damageManager = new DamageManager(_sprite.Slices.Select(x => x.Value.Name).ToList());

            this.IsCollidable = false;
            this.IsDestroyed = false;
        }

        /// <summary>
        ///     Flag to enable collision check with other entities who are also marked for collision
        /// </summary>
        public bool IsCollidable { get; protected set; }

        /// <summary>
        ///     Flag to mark entity for removal from list
        /// </summary>
        public bool IsDestroyed { get; set; }

        /// <summary>
        ///     Position of entity
        /// </summary>
        public Vector2 Position
        {
            get => _sprite.Position;
            set => _sprite.Position = value;
        }

        /// <summary>
        ///     Transform of entity
        /// </summary>
        public Matrix Transform =>  Matrix.Identity *
                                    Matrix.CreateTranslation(-this.Origin.X, -this.Origin.Y, 0f) *
                                    Matrix.CreateScale(this.Scale.X) *
                                    Matrix.CreateRotationZ(this.Rotation) *
                                    Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0f);

        public RectangleF BoundingBox => (RectangleF) this.Bounds;

        /// <summary>
        ///     Bounding box of entity
        /// </summary>
        public IShapeF Bounds => new Polygon(
            new Vector2[]
                {
                    Vector2.Transform(new Vector2(0f, 0f), this.Transform),
                    Vector2.Transform(new Vector2(_sprite.Width, 0f), this.Transform),
                    Vector2.Transform(new Vector2(_sprite.Width, _sprite.Height), this.Transform),
                    Vector2.Transform(new Vector2(0f, _sprite.Height), this.Transform)
                }).BoundingRectangle;

        /// <summary>
        ///     Mesh of entity (defined by Aseprite slices)
        /// </summary>
        public Dictionary<string, Polygon> Slices
        {
            get
            {
                var slices = new Dictionary<string, Polygon>();
                foreach(var slice in _sprite.Slices)
                    slices.Add(
                        slice.Value.Name,
                        new Polygon(new Vector2[]
                        {
                            Vector2.Transform(new Vector2(
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Left,
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Top), this.Transform),
                            Vector2.Transform(new Vector2(
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Right,
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Top), this.Transform),
                            Vector2.Transform(new Vector2(
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Right,
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Bottom), this.Transform),
                            Vector2.Transform(new Vector2(
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Left,
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Bottom), this.Transform)
                        }));

                return slices;
            }
        }

        /// <summary>
        ///     Velocity of entity
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        ///     Origin of entity
        /// </summary>
        public Vector2 Origin
        {
            get => _sprite.Origin;
            protected set => _sprite.Origin = value;
        }

        /// <summary>
        ///     Rotation of entity
        /// </summary>
        public float Rotation
        {
            get => _sprite.Rotation;
            set => _sprite.Rotation = value;
        }

        /// <summary>
        ///     Scale of entity
        /// </summary>
        public Vector2 Scale
        {
            get => _sprite.Scale;
            set => _sprite.Scale = value;
        }

        /// <summary>
        ///     Color of entity
        /// </summary>
        public Color Color
        {
            get => _sprite.Color;
            set => _sprite.Color = value;
        }

        /// <summary>
        ///     Per frame entity update
        /// </summary>
        public virtual void Update(GameTime gameTime) {}

        /// <summary>
        ///     Per frame entity draw
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch) {}

        /// <summary>
        ///     Course collision check (ICollitionActor prerequisite)
        /// </summary>
        public void OnCollision(CollisionEventArgs collisionEventArgs)
        {
            OnCollisionInnerBounds(collisionEventArgs);

            if(_isColliding)
            {
                this.Position -= collisionEventArgs.PenetrationVector;
                this.Velocity *= -1f;
                _isColliding = false;
 
                return;
            }
        }

        /// <summary>
        ///     Fine collision check 
        /// </summary>
        private void OnCollisionInnerBounds(CollisionEventArgs collisionEventArgs)
        {
            if(_isColliding)
                return;

            var other = (Entity) collisionEventArgs.Other;
            foreach(var A in this.Slices)
                foreach(var B in other.Slices)
                    if(PolygonExtensions.Intersects(A.Value, B.Value))
                    {
                        this._damageManager.AddDamage(A.Key, this.Scale.Dot(other.Scale) * 2f);
                        other._damageManager.AddDamage(B.Key, other.Scale.Dot(this.Scale) * 2f);
                        this._isColliding = true;
                        other._isColliding = true;
                        return;
                    }
        }

        /// <summary>
        ///     Course collision check (ITargetActor prerequisite)
        /// </summary>
        public virtual void OnCollision(CollisionInfo collisionInfo)
        {   // CollisionWorld
        }

        /// <summary>
        ///     Destroy entity
        /// </summary>
        public virtual void Destroy()
        {
            IsDestroyed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
                _sprite.Texture.Dispose();

            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Entity()
        {
            Dispose(false);
        } 
    }
}