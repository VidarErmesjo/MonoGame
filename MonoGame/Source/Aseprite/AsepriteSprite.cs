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
        private readonly AsepriteData _asepriteData;
        private Color[,] _pixelMap { get; set; }
        private string _currentAnimation { get; set; }
        private int _currentFrame { get; set; }
        private bool _isAnimated = false;

        public Texture2D Texture { get; private set; }
        public RenderTarget2D Outline { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public IShapeF Bounds { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

        public AsepriteSprite(string name)
        {
            name = name.Replace('\\', '/').Split('/').Last().Split('.').First();
            Texture = Assets.Texture(name);

            Color[] colors = new Color[Texture.Width * Texture.Height];
            Texture.GetData<Color>(colors);

            _pixelMap = new Color[Texture.Width, Texture.Height];
            for(int y = 0; y < Texture.Height; y++)
                for(int x = 0; x < Texture.Width; x++)
                    _pixelMap[x, y] = colors[x + y * Texture.Width];

            Position = new Vector2(0f, 0f);
            Color = Color.White;
            Rotation = 0f;
            SpriteEffect = SpriteEffects.None;
    
            try
            {
               _asepriteData = JsonConvert.DeserializeObject<AsepriteData>(
                    File.ReadAllText(Path.Combine("Content/Animations/" + name + ".json")));
               
                if(_asepriteData.meta.frameTags.Count > 0)
                    _isAnimated = true;

                Rectangle = new Rectangle(
                    0,
                    0,
                    _asepriteData.frames[0].sourceSize.w,
                    _asepriteData.frames[0].sourceSize.h); 

                Origin = new Vector2(
                    _asepriteData.frames[0].sourceSize.w * 0.5f,
                    _asepriteData.frames[0].sourceSize.h * 0.5f);

                Scale = _asepriteData.meta.scale;

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
            catch(FileNotFoundException)
            {
                _isAnimated = false;

                Rectangle = new Rectangle(
                    0,
                    0,
                    Texture.Width,
                    Texture.Height);

                Origin = new Vector2(
                    Texture.Width * 0.5f,
                    Texture.Height * 0.5f);

                Scale = 1f;
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

            Bounds = new RectangleF(
                Rectangle.X,
                Rectangle.Y,
                Rectangle.Width + 2,
                Rectangle.Height + 2);
            Bounds.Position = Position;

            Outline = new RenderTarget2D(
                Core.GraphicsDeviceManager.GraphicsDevice,
                Rectangle.Width + 2,
                Rectangle.Height + 2);

            Core.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(Outline);
            SpriteBatch temp = new SpriteBatch(Core.GraphicsDeviceManager.GraphicsDevice);
            temp.Begin(samplerState: SamplerState.PointClamp);
            temp.DrawRectangle((RectangleF) Bounds, Color.White, 1);
            temp.End();
            Core.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);
        }

        public void OnCollision(CollisionEventArgs collisionEventArgs)
        {
            Bounds.Position -= collisionEventArgs.PenetrationVector;
            System.Console.WriteLine("Making LOve! {0}, {1}", Bounds.Position, collisionEventArgs.PenetrationVector);
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
            int frequency = (int) (gameTime.TotalGameTime.Milliseconds % _asepriteData.frames[_currentFrame].duration);
            if(frequency == 0)
                _currentFrame++;

            if(_currentFrame > _animations[_currentAnimation].ToArray().Length - 1)
                _currentFrame = 0;

            Rectangle = _animations[_currentAnimation].ToArray().ElementAt(_currentFrame);

            //Bounds.Position = Position;
        }

        public void Render(SpriteBatch spriteBatch)
        {
           spriteBatch.Draw(
                texture: Outline,
                position: Position,
                sourceRectangle: Outline.Bounds,
                color: Color.Red,
                rotation: Rotation,
                origin: Origin + Vector2.One,
                scale: Scale,
                effects: SpriteEffect,
                layerDepth: 0
            );

            spriteBatch.Draw(
                texture: Texture,
                position: Position,
                sourceRectangle: Rectangle,
                color: Color,
                rotation: Rotation,
                origin: Origin,
                scale: Scale,
                effects: SpriteEffect,
                layerDepth: 0
            );
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
                Texture.Dispose();
            }

            isDisposing = true;
        }

        ~AsepriteSprite()
        {
            Dispose(false);
        }
    }
}