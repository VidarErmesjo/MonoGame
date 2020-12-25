using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;

namespace MonoGame.Entities
{
    public class Actor : Entity //, ICollisionActor
    {
        protected readonly AnimatedSprite _sprite;
        protected readonly HitDetectionManager _hitDetectionManager;
        //private readonly List<AnimatedSprite> _sprites;
        //private Transform2 _transform; Use with multiple sprites?

        protected bool _isColliding = false;
        
        public override Vector2 Position
        {
            get => _sprite.Position;
            set => _sprite.Position = value;
        }

        public Matrix Transform
        {
            get =>  Matrix.Identity *
                    Matrix.CreateTranslation(-this.Origin.X, -this.Origin.Y, 0f) *
                    Matrix.CreateScale(this.Scale.X) *
                    Matrix.CreateRotationZ(this.Rotation) *
                    Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0f);
        }

        public override RectangleF BoundingBox
        {   // CollisionWorld?
            get => (RectangleF) this.Bounds;
        }

        public override IShapeF Bounds
        {
            get
            {
                var corners = new Vector2[]
                {
                    Vector2.Transform(new Vector2(0f, 0f), this.Transform),
                    Vector2.Transform(new Vector2(_sprite.Width, 0f), this.Transform),
                    Vector2.Transform(new Vector2(_sprite.Width, _sprite.Height), this.Transform),
                    Vector2.Transform(new Vector2(0f, _sprite.Height), this.Transform)
                };

                var min = new Vector2(corners.Min(i => i.X), corners.Min(i => i.Y));
                var max = new Vector2(corners.Max(i => i.X), corners.Max(i => i.Y));

                return new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
            }

            set {}
        }

        public List<Slice> Slices
        {
            get
            {

                return null;
            }
        }

        public Dictionary<string, Polygon> Polygons
        {
            get
            {
                var slices = new Dictionary<string, Polygon>();
                foreach(var slice in _sprite.Slices)
                {
                    var vectors = new List<Vector2>();

                    vectors.Add(
                        Vector2.Transform(
                            new Vector2(
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Left,
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Top),
                            this.Transform));

                    vectors.Add(
                        Vector2.Transform(
                            new Vector2(
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Right,
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Top),
                            this.Transform));

                    vectors.Add(
                        Vector2.Transform(
                            new Vector2(
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Right,
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Bottom),
                            this.Transform));

                    vectors.Add(
                        Vector2.Transform(
                            new Vector2(
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Left,
                                slice.Value.Keys[_sprite.CurrentFrameIndex].Bounds.Bottom),
                            this.Transform));

                    slices.Add(slice.Value.Name, new Polygon(vectors));                    
                }

                return slices;
            }
        }

        public override Vector2 Velocity { get; set; }

        public override Vector2 Origin
        {
            get => _sprite.Origin;
            protected set => _sprite.Origin = value;
        }

        public override float Rotation
        {
            get => _sprite.Rotation;
            set => _sprite.Rotation = value;
        }

        public override Vector2 Scale
        {
            get => _sprite.Scale;
            set => _sprite.Scale = value;
        }

        public override Color Color
        {
            get => _sprite.Color;
            set => _sprite.Color = value;
        }

        public Actor(AsepriteDocument asepriteDocument)
        {
            _sprite = new AnimatedSprite(asepriteDocument);
            _hitDetectionManager = new HitDetectionManager(_sprite.Slices.Select(x => x.Value.Name).ToList());
            this.IsCollidable = true;
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if(this.Position.X <= 0f || this.Position.X >= Core.VirtualResolution.Width - _sprite.Width)
                this.Velocity = new Vector2(-this.Velocity.X, this.Velocity.Y);
            if(this.Position.Y <= 0f || this.Position.Y >= Core.VirtualResolution.Height - _sprite.Height)
                this.Velocity = new Vector2(this.Velocity.X, -this.Velocity.Y);
            
            if(!_isColliding)
            {
                //this.Velocity *= 0.5f;
                this.Position += this.Velocity * deltaTime;
                this.Origin = new Vector2(_sprite.Width * 0.5f, _sprite.Height * 0.5f);
                this.Rotation += 0.01f;
                if(this.Rotation >= 2 * MathF.PI)
                    this.Rotation = 0f;

                this.Color = Color.White;
            }

            _sprite.Play((this.Velocity == Vector2.Zero ? "Idle" : "Walk"));
            _sprite.Update(deltaTime);
            _hitDetectionManager.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Render(spriteBatch);

            foreach(var part in _hitDetectionManager.Parts)
                foreach(var polygon in this.Polygons)
                    if(part.Key == polygon.Key && part.Value > 0f)
                        spriteBatch.DrawPolygon(Vector2.Zero, polygon.Value, new Color { R = 255, G = 0, B = 0, A = (byte) part.Value }, part.Value);
        }

        public override void OnCollision(CollisionEventArgs collisionEventArgs)
        {
            OnCollisionInnerBounds(collisionEventArgs);

            if(_isColliding)
            {
                this.Position -= collisionEventArgs.PenetrationVector;
                this.Velocity *= -1f;
                //this.Color = Color.Red;
                _isColliding = false;
 
                return;
            }

            base.OnCollision(collisionEventArgs);
        }

        private void OnCollisionInnerBounds(CollisionEventArgs collisionEventArgs)
        {
            if(_isColliding)
                return;

            var other = (Actor) collisionEventArgs.Other;
            foreach(var A in this.Polygons)
                foreach(var B in other.Polygons)
                    if(PolygonExtensions.Intersects(A.Value, B.Value))
                    {
                        this._hitDetectionManager.AddDamage(A.Key, this.Scale.Dot(other.Scale) * 2f);
                        other._hitDetectionManager.AddDamage(B.Key, other.Scale.Dot(this.Scale) * 2f);
                        this._isColliding = true;
                        other._isColliding = true;
                        return;
                    }
        }

        public override void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
            {
                //Console.WriteLine("{0} [{1}] Dispose()", this.GetType().Name, this.GetHashCode());
                _sprite.Texture.Dispose();
            }

            isDisposed = true;
        }
    }
}