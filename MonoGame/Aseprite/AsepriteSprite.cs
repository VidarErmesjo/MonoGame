using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MonoGame.Aseprite
{
    public class AsepriteSprite : IDisposable
    {
        private bool isDisposing = false;

        private readonly Dictionary<string, List<Rectangle>> _animations;
        private readonly AsepriteData _asepriteData;
        private Color[,] _pixelMap { get; set; }
        private string _currentAnimation { get; set; }
        private int _currentFrame { get; set; }
        private bool _isAnimated = false;

        public Texture2D Texture { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
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

            Position = new Vector2(0.0f, 0.0f);
            Rotation = 0.0f;
            SpriteEffect = SpriteEffects.None;
    
            _asepriteData = new AsepriteData();
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
            }
            catch(System.Exception e)
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

                Scale = 1.0f;

                return;
            }

            var frames = new Dictionary<int, Rectangle>();
            int index = 0;
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
            int frequency = (int) (gameTime.TotalGameTime.TotalMilliseconds % _asepriteData.frames[_currentFrame].duration);
            if(frequency == 0)
                _currentFrame++;

            if(_currentFrame > _animations[_currentAnimation].ToArray().Length - 1)
                _currentFrame = 0;

            Rectangle = _animations[_currentAnimation].ToArray().ElementAt(_currentFrame);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: Texture,
                position: Position,
                sourceRectangle: Rectangle,
                color: Color.White,
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
                System.Console.WriteLine(Texture.Name + " disposed");
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