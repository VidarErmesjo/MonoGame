using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Triangulation;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MonoGame.Aseprite
{   // inherit from basic AsepriteSprite?
    public class SuperSprite : ICollisionActor, IDisposable
    {
        private bool isDisposing = false;

        private readonly Dictionary<string, List<Rectangle>> _animations;
        private readonly Dictionary<string, List<Color[]>> _data;   // Deprecate?
        private readonly Dictionary<string, List<bool[]>> _mask;    // Deprecate?
        private readonly List<Slice> _slices;
        private readonly AsepriteData _asepriteData;
        private Color[,] _pixelMap = null;  // Deprecate?
        private string _currentAnimation = "Idle";
        private string _currentSlice = "Unassigned"; // Blææ?
        private int _currentFrame = 0;

        private bool _isAnimated = false;
        private bool _hasSlices = false;

        /// <summary>
        /// Returns the full sprite sheet.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// <returns>The current source rectangle mapped to Texture.
        /// Rectangle defines sprite from the full sprite sheet. </returns>
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                return _isAnimated ?
                    _animations[_currentAnimation].ToArray().ElementAt(_currentFrame)
                    :
                    new Rectangle(0, 0, Texture.Width, Texture.Height);
            }
        }

        /// <summary>
        /// Vector returned from OnCollision event.
        /// </summary>
        public Vector2 PenetrationVector { get; private set; }

        /// <summary>
        /// Position of sprite.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Center point of sprite.
        /// </summary>
        public Vector2 Origin
        { 
            get
            {
                return _isAnimated ?
                    new Vector2(
                        _animations[_currentAnimation].ToArray().ElementAt(_currentFrame).Width * 0.5f,
                        _animations[_currentAnimation].ToArray().ElementAt(_currentFrame).Height * 0.5f)
                    :
                    new Vector2(
                        Texture.Width * 0.5f,
                        Texture.Height * 0.5f);
            }

            set {}
        }

        // Deprecate?
        /// <summary>
        /// Size of sprite. DEPRECATE?
        /// </summary>
        public Size Size
        {
             get
             {
                 return _isAnimated ?
                    new Size(
                        _animations[_currentAnimation].ToArray().ElementAt(_currentFrame).Width,
                        _animations[_currentAnimation].ToArray().ElementAt(_currentFrame).Height)
                    :
                    new Size(
                        this.Texture.Width,
                        this.Texture.Height);
             }
        }

        /// <summary>
        /// Color of sprite.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Scale of sprite.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Rotation of sprite.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Effects on sprite.
        /// </summary>
        public SpriteEffects SpriteEffect { get; set; }

        /// <summary>
        /// Return outer bounds of sprite.
        /// Grows in respect to Rotation and Scale.
        /// </summary>
        public IShapeF Bounds
        {
            get
            {
                // Only care about 45 degrees
                double rotation = Math.Abs(this.Rotation % (Math.PI * 0.5f));
                double cos = Math.Cos(rotation);
                double sin = Math.Sin(rotation);

                Vector2 scaledBounds = new Vector2(
                    (float) Math.Abs(this.Rectangle.Width * cos + this.Rectangle.Height * sin),
                    (float) Math.Abs(this.Rectangle.Width * sin + this.Rectangle.Height * cos));
                Vector2 scaledOrigin = new Vector2(
                    (float) Math.Abs(this.Origin.X * cos + this.Origin.Y * sin),
                    (float) Math.Abs(this.Origin.X * sin + this.Origin.Y * cos));

                return new RectangleF(
                    this.Position - scaledOrigin * this.Scale,
                    new Size2(
                        scaledBounds.X * this.Scale,
                        scaledBounds.Y * this.Scale));
            }
        }

        /// <summary>
        /// Returns the local transform of the sprite.
        /// </summary>
        public Matrix Transform
        {
            get
            {
                return Matrix.Identity *
                    Matrix.CreateTranslation(-this.Origin.X, -this.Origin.Y, 0f) *
                    Matrix.CreateScale(this.Scale) *
                    Matrix.CreateRotationZ(this.Rotation) *
                    Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0f);
            }
        }

        public HashSet<Vector2> Vertices
        {
            get
            {
                if(_hasSlices)
                {
                    HashSet<Vector2> vertices = new HashSet<Vector2>();
                    foreach(var slice in _slices)
                    {
                        vertices.Add(Vector2.Transform(
                            new Vector2(
                                slice.Keys[_currentFrame].Bounds.Left,
                                slice.Keys[_currentFrame].Bounds.Top),
                            this.Transform));

                        vertices.Add(Vector2.Transform(
                            new Vector2(
                                slice.Keys[_currentFrame].Bounds.Right,
                                slice.Keys[_currentFrame].Bounds.Top),
                            this.Transform));

                        vertices.Add(Vector2.Transform(
                            new Vector2(
                                slice.Keys[_currentFrame].Bounds.Right,
                                slice.Keys[_currentFrame].Bounds.Bottom),
                            this.Transform));

                        vertices.Add(Vector2.Transform(
                            new Vector2(
                                slice.Keys[_currentFrame].Bounds.Left,
                                slice.Keys[_currentFrame].Bounds.Bottom),
                            this.Transform));
                    }

                    //int[] indices;
                    //Vector2[] output;
                    //Triangulator.Triangulate(vertices.ToArray(), WindingOrder.CounterClockwise, out output, out indices);
                    /*vertices.Clear();
                    foreach(var index in indices)
                        vertices.Add(output[index]);*/
                        //vertices.Add(Vector2.Transform(output[index], this.Transform));
                    //foreach(int index in indices)
                    //System.Console.WriteLine(index);
                    //System.Console.WriteLine(vertices.Count + ", " + output.Length + ", " + indices.Length);

                    return vertices;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns point list of polygons defined by sprite slices.
        /// <param name="Polygons"> List of polygons from slices.
        /// </summary>
        public Bag<Polygon> Polygons
        {
            get
            {
                if(_hasSlices)
                {
                    Bag<Polygon> polygons = new Bag<Polygon>();
                    foreach(var slice in _slices)
                    {
                        Bag<Vector2> vectorList = new Bag<Vector2>(3);
                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Left,
                                    slice.Keys[_currentFrame].Bounds.Top),
                                this.Transform));

                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Right,
                                    slice.Keys[_currentFrame].Bounds.Top),
                                this.Transform));

                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Right,
                                    slice.Keys[_currentFrame].Bounds.Bottom),
                                this.Transform));

                        polygons.Add(new Polygon(vectorList));

                        vectorList.Clear();
                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Right,
                                    slice.Keys[_currentFrame].Bounds.Bottom),
                                this.Transform));

                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Left,
                                    slice.Keys[_currentFrame].Bounds.Bottom),
                                this.Transform));

                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Left,
                                    slice.Keys[_currentFrame].Bounds.Top),
                                this.Transform));

                        polygons.Add(new Polygon(vectorList));
                    }

                    return polygons;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns array of polygons defined by sprite slices.
        /// <param name="Slices">List of polygons from slices.
        /// </summary>
        public Bag<Polygon> Slices
        {
            get
            {
                if(_hasSlices)
                {
                    _currentSlice = _slices[_currentFrame].Name;
                    Bag<Polygon> slices = new Bag<Polygon>();
                    foreach(var slice in _slices)
                    {
                        Bag<Vector2> vectorList = new Bag<Vector2>(4);
                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Left,
                                    slice.Keys[_currentFrame].Bounds.Top),
                                this.Transform));

                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Right,
                                    slice.Keys[_currentFrame].Bounds.Top),
                                this.Transform));

                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Right,
                                    slice.Keys[_currentFrame].Bounds.Bottom),
                                this.Transform));

                        vectorList.Add(
                            Vector2.Transform(
                                new Vector2(
                                    slice.Keys[_currentFrame].Bounds.Left,
                                    slice.Keys[_currentFrame].Bounds.Bottom),
                                this.Transform));

                        slices.Add(new Polygon(vectorList));
                    }

                    return slices;
                }

                return null;
            }
        }

        // Deprecated?
        public Color[] Data
        {
            get
            {
                return _data[_currentAnimation].ToArray().ElementAt(_currentFrame);
            }
        }

        // Deprecate?
        public bool[] Mask
        {
            get
            {
                return _mask[_currentAnimation].ToArray().ElementAt(_currentFrame);
            }            
        }

        /// <summary>
        /// Sprite with animations and polygonal collision bounds. (Set up in Aseprite)
        /// <param name="name">Shared name of image and json files. </param>
        /// </summary>
        public SuperSprite(string name)
        {
            name = name.Replace('\\', '/').Split('/').Last().Split('.').First();
            this.Texture = Assets.Texture(name);

            Color[] colors = new Color[Texture.Width * Texture.Height];
            this.Texture.GetData<Color>(colors);

            _pixelMap = new Color[Texture.Width, Texture.Height];
            for(int y = 0; y < Texture.Height; y++)
                for(int x = 0; x < Texture.Width; x++)
                    _pixelMap[x, y] = colors[x + y * Texture.Width];

            this.Position = new Vector2(0f, 0f);
            this.Color = Color.White;
            this.Rotation = 0f;
            this.SpriteEffect = SpriteEffects.None;
    
            try
            {
                // Check for .json Asprite data
               _asepriteData = JsonConvert.DeserializeObject<AsepriteData>(
                    File.ReadAllText(Path.Combine("Content/Aseprite/" + name + ".json")));
               
                if(_asepriteData.meta.frameTags != null)
                    _isAnimated = true;

                if(_asepriteData.meta.slices != null)
                    _hasSlices = true;

                this.Scale = _asepriteData.meta.scale;

                int index = 0;
                Dictionary<int, Rectangle> frames = new Dictionary<int, Rectangle>();
                for(int y = 0; y < _asepriteData.meta.size.h; y += _asepriteData.frames[0].sourceSize.h)
                    for(int x = 0; x < _asepriteData.meta.size.w; x += _asepriteData.frames[0].sourceSize.w)
                    {
                        Rectangle source = new Rectangle(
                            x,
                            y,
                            _asepriteData.frames[0].sourceSize.w,
                            _asepriteData.frames[0].sourceSize.h);
                        
                        // All frames (used/unused) in sprite sheet
                        frames.Add(index, source);
                        index++;
                    }

                if(_isAnimated)
                {
                    _animations = new Dictionary<string, List<Rectangle>>();
                    _data = new Dictionary<string, List<Color[]>>();
                    _mask = new Dictionary<string, List<bool[]>>();
                    foreach(var frameTag in _asepriteData.meta.frameTags.ToArray())
                    {
                        List<Rectangle> rectangles = new List<Rectangle>();
                        List<Color[]> data = new List<Color[]>();
                        List<bool[]> mask = new List<bool[]>();
                        for(int i = frameTag.from; i <= frameTag.to; i++)
                        {
                            // Animation frames to use
                            rectangles.Add(frames[i]);

                            // Cache pixel data
                            Color[] tempData = new Color[frames[i].Width * frames[i].Height];
                            this.Texture.GetData<Color>(0, frames[i], tempData, 0, frames[i].Width * frames[i].Height);
                            data.Add(tempData);

                            // Cache collision mask
                            bool[] tempMask = new bool[frames[i].Width * frames[i].Height];
                            for(index = 0; index < tempData.Length; index++)
                                tempMask[index] = tempData[index].A != 0 ? true : false;
                            mask.Add(tempMask);
                        }

                        // Done
                        _animations.Add(frameTag.name, rectangles);
                        _data.Add(frameTag.name, data);
                        _mask.Add(frameTag.name, mask);
                    }
                }

                if(_hasSlices)
                {
                    _slices = new List<Slice>();
                    foreach(var slice in _asepriteData.meta.slices.ToArray())
                    {
                        Slice tempSlice = new Slice();

                        tempSlice.Name = slice.name;
                        tempSlice.Color = ColorHelper.FromHex(slice.color);

                        foreach(var key in slice.keys)
                        {
                            Key tempKey = new Key();
                            tempKey.Frame = key.frame;
                            tempKey.Bounds = new Rectangle(
                                key.bounds.x,
                                key.bounds.y,
                                key.bounds.w,
                                key.bounds.h);
                            tempKey.Pivot = new Point(key.pivot.x, key.pivot.y);

                            tempSlice.Keys.Add(tempKey);
                        }

                        _slices.Add(tempSlice);
                    }
                }
            }
            catch(FileNotFoundException)
            {
                // Revert to static sprite
                _isAnimated = false;

                // Pixel data
                Color[] tempData = new Color[this.Texture.Width * this.Texture.Height];
                this.Texture.GetData<Color>(0, this.Rectangle, tempData, 0, Texture.Width * Texture.Height);
                List<Color[]> data = new List<Color[]>();
                data.Add(tempData);

                // Collision mask
                bool[] tempMask = new bool[this.Texture.Width * this.Texture.Height];
                for(int index = 0; index < tempData.Length; index++)
                    tempMask[index] = tempData[index].A != 0 ? true : false;
                List<bool[]> mask = new List<bool[]>();
                mask.Add(tempMask);

                _data.Add(_currentAnimation, data);
                _mask.Add(_currentAnimation, mask);

                this.Scale = 1f;
            }

            this.Scale = 4f; // Remove when done testing
        }

        // Same as Data? Deprecate?
        public Color[] Frame()
        {
            Color[] colors = new Color[Rectangle.Width * Rectangle.Height];

            int index = 0;
            for(int y = Rectangle.Y; y < Rectangle.Y + Rectangle.Height; y++)
                for(int x = Rectangle.X; x < Rectangle.X + Rectangle.Width; x++)
                {
                    colors[index] = _pixelMap[x, y];
                    index++;
                }

            return colors;
        }

        /// <summary>
        /// Sets which sprite animation to play.
        /// Deault: "Idle".
        /// </summary>
        public void Play(string name)
        {
            if(!_isAnimated)
                return;

            _currentAnimation = string.IsNullOrWhiteSpace(name) ?
                _asepriteData.meta.frameTags[0].name : name;

            _currentFrame = _currentAnimation == name ? _currentFrame : 0;
        }

        /// <summary>
        /// Update the sprite on game time.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if(_isAnimated)
            {
                int frequency = (int) (gameTime.TotalGameTime.Milliseconds % _asepriteData.frames[_currentFrame].duration);
                if(frequency == 0)
                    _currentFrame++;

                if(_currentFrame > _animations[_currentAnimation].ToArray().Length - 1)
                    _currentFrame = 0;
            }

            //this.Color = Color.White;
            this.PenetrationVector = Vector2.Zero;
        }

        /// <summary>
        /// Renders the sprite.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Slice mesh
            if(_hasSlices)
                foreach(var slice in this.Slices)
                    ShapeExtensions.DrawPolygon(spriteBatch, Vector2.Zero, slice, Color.Blue, 1f);

            // Inner bounds
            /*if(_hasSlices && this.Rotation != 0f)
                foreach(var polygon in this.Polygons)
                    spriteBatch.DrawRectangle((RectangleF) polygon.BoundingRectangle, Color.Green, 0.5f, 0);*/

            // SLice vertices
            /*if(_hasSlices)
                foreach(var polygon in this.Triangles)
                    foreach(var vertex in polygon.Vertices)
                        spriteBatch.DrawPoint(
                            vertex.X,
                            vertex.Y,
                            Color.Yellow,
                            1f);*/

            /*if(_hasSlices)
                foreach(var vertex in this.Vertices)
                    spriteBatch.DrawPoint(
                        vertex.X,
                        vertex.Y,
                        Color.Yellow,
                        1f);*/

            // Bounding boxs
            //spriteBatch.DrawRectangle((RectangleF) this.Bounds, Color.Yellow, 0.25f, 0);

            // Sprite
            spriteBatch.Draw(
                texture: this.Texture,
                position: this.Position,
                sourceRectangle: this.Rectangle,
                color: this.Color,
                rotation: this.Rotation,
                origin: this.Origin,
                scale: this.Scale,
                effects: this.SpriteEffect,
                layerDepth: 0);

            /*Point2 point = Core.ViewportAdapter.PointToScreen(Core.MouseState.X, Core.MouseState.Y);
            foreach(var polygon in this.Polygons)
                if(Collisions.Check(point, polygon))
                    System.Console.WriteLine("Inside!");*/

           /* if(_hasSlices)
                foreach(var vertex in this.Vertices)
                    spriteBatch.DrawPoint(vertex.X, vertex.Y, Color.White, 2f);*/

            //System.Console.WriteLine(this.GetHashCode() + ": " + whatIsThis);

            //spriteBatch.DrawLine(this.Rectangle.Center.ToVector2(), whatIsThis, Color.Red, 1f);
        }

        /// <summary>
        /// Mandatory function that handles OnCollision events from CollisionComponent.
        /// Used to further decide weither point collision did occure.
        /// </summary>
        public void OnCollision(CollisionEventArgs collisionEventArgs)
        {
            if(_hasSlices && OnCollisionInnerBounds(collisionEventArgs))
            {
                this.PenetrationVector = collisionEventArgs.PenetrationVector;
                this.Color = Color.Red;
                System.Console.WriteLine("{0}: {1}", this.GetHashCode(), _slices[_currentFrame]);
                return;
            }

            this.Color = Color.Yellow;
        }

        public Vector2 whatIsThis = Vector2.Zero;
        private bool OnCollisionInnerBounds(CollisionEventArgs collisionEventArgs)
        {
            SuperSprite other = (SuperSprite) collisionEventArgs.Other;

            foreach(var A in this.Slices)
                foreach(var B in other.Slices)  // Must compare algorithms
                    if(Collisions.Intersects(A, B))
                    //if(A != B && Collisions.Intersects(A, B, out whatIsThis))
                        return true;

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(isDisposing)
                return;

            if(disposing)
            {
                this.Texture.Dispose();
            }

            isDisposing = true;
        }

        ~SuperSprite()
        {
            Dispose(false);
        }
    }
}