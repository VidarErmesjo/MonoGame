using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MonoGame.Aseprite
{   // Change name to SuperSprite, HyperSprite or just Sprite??
    public class AsepriteSprite : ICollisionActor, IDisposable
    {
        private bool isDisposing = false;

        private readonly Dictionary<string, List<Rectangle>> _animations;
        private readonly Dictionary<string, List<Color[]>> _data;
        private readonly Dictionary<string, List<bool[]>> _mask;
        private readonly List<Slice> _slices;
        private readonly AsepriteData _asepriteData;
        private Color[,] _pixelMap = null;
        private string _currentAnimation = "Idle";
        private int _currentFrame = 0;
        private bool _isAnimated = false;
        private bool _hasSlices = false;

        public Texture2D Texture { get; private set; }
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
        public Vector2 PenetrationVector { get; private set; }

        public Vector2 Position { get; set; }
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
        }

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

        public Color Color { get; private set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public IShapeF Bounds
        {
            get
            {
                double rotation = Math.Abs(this.Rotation % (Math.PI * 0.5f));
                double cos = Math.Cos(rotation);
                double sin = Math.Sin(rotation);

                Vector2 sacledBounds = new Vector2(
                    (float) Math.Abs(this.Rectangle.Width * cos + this.Rectangle.Height * sin),
                    (float) Math.Abs(this.Rectangle.Width * sin + this.Rectangle.Height * cos));
                Vector2 scaledOrigin = new Vector2(
                    (float) Math.Abs(this.Origin.X * cos + this.Origin.Y * sin),
                    (float) Math.Abs(this.Origin.X * sin + this.Origin.Y * cos));

                return new RectangleF(
                    this.Position - scaledOrigin * this.Scale,
                    new Size2(
                        sacledBounds.X * this.Scale,
                        sacledBounds.Y * this.Scale));
            }
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.Identity *
                    Matrix.CreateTranslation(-this.Origin.X, -this.Origin.Y, 0f) *
                    Matrix.CreateRotationZ(this.Rotation) *
                    Matrix.CreateScale(this.Scale) *
                    Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0f);
            }
        }

        public List<Polygon> Polygons
        {
            get
            {
                if(_hasSlices)
                {
                    List<Polygon> polygons = new List<Polygon>();
                    foreach(var slice in _slices)
                    {
                        List<Vector2> vectorList = new List<Vector2>(4);
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

                        polygons.Add(new Polygon(vectorList));
                    }

                    return polygons;
                }

                return null;
            }
        }

        public Color[] Data
        {
            get
            {
                return _data[_currentAnimation].ToArray().ElementAt(_currentFrame);
            }
        }

        public bool[] Mask
        {
            get
            {
                return _mask[_currentAnimation].ToArray().ElementAt(_currentFrame);
            }            
        }

        // Experimental
        public Components.Collision Collision { get; private set; }

        public AsepriteSprite(string name)
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
                        //tempSlice.Color = (Color) slice.color;
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

            //this.Scale = 4f; // Remove when done testing
        }

        // Same as Data?
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

        public void Play(string name)
        {
            if(!_isAnimated)
                return;

            _currentAnimation = string.IsNullOrWhiteSpace(name) ?
                _asepriteData.meta.frameTags[0].name : name;

            _currentFrame = _currentAnimation == name ? _currentFrame : 0;
        }

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

            this.Color = Color.White;
            this.PenetrationVector = Vector2.Zero;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Slice mesh
            /*if(_hasSlices)
                foreach(var polygon in this.Polygons)
                    spriteBatch.DrawPolygon(Vector2.Zero, polygon, Color.Red, 1f);*/

            // Inner bounds
            /*if(_hasSlices && this.Rotation != 0f)
                foreach(var polygon in this.Polygons)
                    spriteBatch.DrawRectangle((RectangleF) polygon.BoundingRectangle, Color.Green, 1f, 0);*/

            // SLice vertices
            /*if(_hasSlices)
                foreach(var polygon in this.Polygons)
                    foreach(var vertex in polygon.Vertices)
                        spriteBatch.DrawPoint(
                            vertex.X,
                            vertex.Y,
                            Color.Yellow,
                            1f);*/

            // Bounding box
            //spriteBatch.DrawRectangle((RectangleF) this.Bounds, Color.Yellow, 1f, 0);

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
        }

        public void OnCollision(CollisionEventArgs collisionEventArgs)
        {
            // Outer bounds
            if(_hasSlices && PolygonPerfectOnCollision(collisionEventArgs))
            {
                this.PenetrationVector = collisionEventArgs.PenetrationVector;
                this.Color = Color.Red;
                return;
            }
            else if(!_hasSlices && PixelPerfectOnCollision(collisionEventArgs))
            {
                this.PenetrationVector = collisionEventArgs.PenetrationVector;
                this.Color = Color.Red;
                return;
            }
            
            this.Color = Color.Yellow;
        }

        public bool PolygonPerfectOnCollision(CollisionEventArgs collisionEventArgs)
        {
            AsepriteSprite other = (AsepriteSprite) collisionEventArgs.Other;

            foreach(var A in this.Polygons)
                foreach(var B in other.Polygons)
                    if(A != B && A.BoundingRectangle.Intersects(B.BoundingRectangle))
                        return PolygonIntersects(A, B);

            return false;
        }

        /// <summary>
        /// Edge / diagonal intersection
        /// </summarY>
        public bool PolygonIntersects(Polygon A, Polygon B)
        {
            for(int shape = 0; shape < 2; shape++)
            {
                if(shape == 1)
                {
                    Polygon C = A;
                    A = B;
                    B = C;
                }

                for(int p = 0; p < A.Vertices.Length; p++)
                {
                    Vector2 beginA = A.BoundingRectangle.Center;
                    Vector2 endA = A.Vertices[p];

                    for(int q = 0; q < B.Vertices.Length; q++)
                    {
                        Vector2 beginB = B.Vertices[q];
                        Vector2 endB = B.Vertices[(q + 1) % B.Vertices.Length];

                        float h =   (endB.X - beginB.X) * (beginA.Y - endA.Y) -
                                    (beginA.X - endA.X) * (endB.Y - beginB.Y);

                        float t1 =  (beginB.Y - endB.Y) * (beginA.X - beginB.X) +
                                    (endB.X - beginB.X) * (beginA.Y - beginB.Y);

                        float t2 =  (beginA.Y - endA.Y) * (beginA.X - beginB.X) +
                                    (endA.X - beginA.X) * (beginA.Y - beginB.Y);

                        t1 /= h;
                        t2 /= h;

                        if(t1 >= 0f && t1 < 1f && t2 >= 0f && t2 < 1f)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool PixelPerfectOnCollision(CollisionEventArgs collisionEventArgs)
        {
            AsepriteSprite other = (AsepriteSprite) collisionEventArgs.Other;

            // New
            bool swap = this.Rectangle.Width * this.Rectangle.Height > other.Rectangle.Width * other.Rectangle.Height;

            Matrix A = !swap ? this.Transform : other.Transform;
            Matrix B = !swap ? other.Transform : this.Transform;
            int widthA = !swap ? this.Rectangle.Width : other.Rectangle.Width;
            int widthB = !swap ? other.Rectangle.Width : this.Rectangle.Width;
            int heightA = !swap ? this.Rectangle.Height : other.Rectangle.Height;
            int heightB = !swap ? other.Rectangle.Height : this.Rectangle.Height;
            bool[] dataA = !swap ? this.Mask : other.Mask;
            bool[] dataB = !swap ? other.Mask : this.Mask;

            Matrix inverseB = Matrix.Invert(B);
            Matrix AB = A * inverseB;

            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, AB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, AB);

            Vector2 startOfRow = Vector2.Transform(Vector2.Zero, AB);

            for(int yA = 0; yA < heightA; yA++)
            {
                Vector2 positionB = startOfRow;
                for(int xA = 0; xA < widthA; xA++)
                {
                    int xB = (int) Math.Round(positionB.X);
                    int yB = (int) Math.Round(positionB.Y);

                    if(0 <= xB && xB < widthB && 0 <= yB && yB < heightB)
                        if(dataA[xA + yA * widthA] && dataB[xB + yB * widthB])
                            return true;

                    positionB += stepX;
                }

                startOfRow += stepY;
            }

            // Old
            /*Rectangle rectA = new Rectangle(
                (int) this.Position.X,
                (int) this.Position.Y,
                (int) (this.Rectangle.Width * this.Scale),
                (int) (this.Rectangle.Height * this.Scale));

            Rectangle rectB = new Rectangle(
                (int) other.Position.X,
                (int) other.Position.Y,
                (int) (other.Rectangle.Width * other.Scale),
                (int) (other.Rectangle.Height * other.Scale));

            int top = Math.Max(rectA.Top, rectB.Top);
            int bottom = Math.Min(rectA.Bottom, rectB.Bottom);
            int left = Math.Max(rectA.Left, rectB.Left);
            int right = Math.Min(rectA.Right, rectB.Right);     

            // Not working perfect for scale other than 1 nor rotation
            for(int y = top; y < bottom; y++)
                for(int x = left; x < right; x++)
                {
                    int indexA = ((x - rectA.Left) + (y - rectA.Top) * rectA.Width) % this.Mask.Length;
                    int indexB = ((x - rectB.Left) + (y - rectB.Top) * rectB.Width) % other.Mask.Length;
                    
                    if(this.Mask[indexA] && other.Mask[indexB])
                        return true;
                }*/

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

        ~AsepriteSprite()
        {
            Dispose(false);
        }
    }
}