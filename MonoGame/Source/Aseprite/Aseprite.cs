using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MonoGame.Aseprite
{
    public class AsepriteSprite : ICollisionActor, IDisposable
    {
        private bool isDisposing = false;

        private readonly Dictionary<string, List<Rectangle>> _animations;
        private readonly Dictionary<string, List<Rectangle>> _slices;
        private readonly AsepriteData _asepriteData;
        private Color[,] _pixelMap = null;
        private string _currentAnimation = "Idle";
        private int _currentFrame = 0;
        private bool _isAnimated = false;
        private bool _isColliding = false;
        private bool _hasSlices = false;

        public Texture2D Texture { get; private set; }
        public RenderTarget2D Outline { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public IShapeF Bounds { get; private set; }

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public Matrix Transform
        {
            get
            {
                return
                    Matrix.CreateTranslation(new Vector3(-this.Origin, 0)) *
                    Matrix.CreateRotationZ(this.Rotation) *
                    Matrix.CreateScale(this.Scale) *
                    Matrix.CreateTranslation(new Vector3(this.Position, 0));
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
               _asepriteData = JsonConvert.DeserializeObject<AsepriteData>(
                    File.ReadAllText(Path.Combine("Content/Animations/" + name + ".json")));
               
                if(_asepriteData.meta.frameTags != null)
                    _isAnimated = true;

                if(_asepriteData.meta.slizes != null)
                    _hasSlices = true;

                this.Rectangle = new Rectangle(
                    0,
                    0,
                    _asepriteData.frames[0].sourceSize.w,
                    _asepriteData.frames[0].sourceSize.h); 

                this.Origin = new Vector2(
                    _asepriteData.frames[0].sourceSize.w * 0.5f,
                    _asepriteData.frames[0].sourceSize.h * 0.5f);

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
                            
                        frames.Add(index, source);
                        index++;
                    }

                if(_isAnimated)
                {
                    _animations = new Dictionary<string, List<Rectangle>>();
                    foreach(var frameTag in _asepriteData.meta.frameTags.ToArray())
                    {
                        List<Rectangle> rectangles = new List<Rectangle>();
                        for(int i = frameTag.from; i <= frameTag.to; i++)
                        {
                            rectangles.Add(frames[i]);
                        }

                        _animations.Add(frameTag.name, rectangles);
                    }
                }

                if(_hasSlices)
                {
                    _slices = new Dictionary<string, List<Rectangle>>();
                    foreach(var slize in _asepriteData.meta.slizes.ToArray())
                    {

                    }
                }
            }
            catch(FileNotFoundException)
            {
                _isAnimated = false;

                this.Rectangle = new Rectangle(
                    0,
                    0,
                    Texture.Width,
                    Texture.Height);

                this.Origin = new Vector2(
                    Texture.Width * 0.5f,
                    Texture.Height * 0.5f);

                this.Scale = 1f;
            }

            /*Color[] color = new Color[Outline.Width * Outline.Height];
            for(int i = 0; i < color.Length; i++)
            {
                color[i].R = 255;s
                color[i].G = 255;
                color[i].B = 255;
                color[i].A = 255;
            }
            Outline.SetData<Color>(color);*/

            /*this.Bounds = new RectangleF(
                this.Rectangle.X,
                this.Rectangle.Y,
                this.Rectangle.Width + 2,
                this.Rectangle.Height + 2);
            this.Bounds.Position = Position;*/
            this.Bounds = new RectangleF(
                this.Position - this.Origin - Vector2.One,
                new Size2(
                    this.Rectangle.Width + 2,
                    this.Rectangle.Height + 2));

            /*this.Bounds = new CircleF(
                Point2.Zero,
                (float) System.Math.Sqrt(this.Origin.X * this.Origin.X + this.Origin.Y * this.Origin.Y));*/

            this.Outline = new RenderTarget2D(
                Core.GraphicsDeviceManager.GraphicsDevice,
                this.Rectangle.Width + 2,
                this.Rectangle.Height + 2);

            /*Core.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(this.Outline);
            SpriteBatch temp = new SpriteBatch(Core.GraphicsDeviceManager.GraphicsDevice);
            temp.Begin(samplerState: SamplerState.PointClamp);
            temp.DrawRectangle((RectangleF) this.Bounds, Color.White, 1);
            temp.End();
            Core.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);*/

            //Extended.Shapes.Polygon polygon;
            //polygon = new Extended.Shapes.Polygon();
        }

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

                this.Rectangle = _animations[_currentAnimation].ToArray().ElementAt(_currentFrame);
            }

            this.Color = Color.White;
            this.PenetrationVector = Vector2.Zero;
            _isColliding = false;
        }

        public void Render(SpriteBatch spriteBatch)
        {
           /*spriteBatch.Draw(
                texture: this.Outline,
                position: this.Position,
                sourceRectangle: this.Outline.Bounds,
                color: Color.Yellow,
                rotation: this.Rotation,
                origin: this.Origin + Vector2.One,
                scale: this.Scale,
                effects: this.SpriteEffect,
                layerDepth: 0
            );*/

            spriteBatch.DrawRectangle((RectangleF) this.Bounds, Color.Red, 1f, 0);
            //spriteBatch.DrawCircle((CircleF) this.Bounds, 32, Color.Red, 1, 0);

            spriteBatch.Draw(
                texture: this.Texture,
                position: this.Position,
                sourceRectangle: this.Rectangle,
                color: this.Color,
                rotation: this.Rotation,
                origin: this.Origin,
                scale: this.Scale,
                effects: this.SpriteEffect,
                layerDepth: 0
            );
        }

        public Vector2 PenetrationVector { get; private set; }
        public void OnCollision(CollisionEventArgs collisionEventArgs)
        {
            this.PenetrationVector = collisionEventArgs.PenetrationVector;
            this.Color = Color.Red;

            _isColliding = true;
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
                this.Outline.Dispose();
            }

            isDisposing = true;
        }

        ~AsepriteSprite()
        {
            Dispose(false);
        }
    }
}