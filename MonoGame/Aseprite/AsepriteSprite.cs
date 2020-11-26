using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MonoGame.Aseprite
{
    public class AsepriteSprite
    {
        private readonly Dictionary<string, List<Rectangle>> _animations;
        private readonly AsepriteData _asepriteData;
        private Texture2D _currentTexture { get; set; }
        private Rectangle _currentSource { get; set; }
        private string _currentAnimation { get; set; }
        private int _currentFrame { get; set; }
        private bool _isAnimated = false;

        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

        public AsepriteSprite(string name)
        {
            if(name.Split('/').Length > 1)
                throw new System.ArgumentException("AsepriteSprite.Length > 1");

            _currentTexture = Assets.Texture(name);//name.Split('/').Last());
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

                _currentSource = new Rectangle(
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

                _currentSource = new Rectangle(
                    0,
                    0,
                    _currentTexture.Width,
                    _currentTexture.Height);

                Origin = new Vector2(
                    _currentTexture.Width * 0.5f,
                    _currentTexture.Height * 0.5f);

                Scale = 1.0f;
                return;
                //throw new FileLoadException(e.Message);
            }

            var frames = new Dictionary<int, Rectangle>();
            int index = 0;
            for(int y = 0; y < _asepriteData.meta.size.h; y += _asepriteData.frames[0].sourceSize.h)
            {
                for(int x = 0; x < _asepriteData.meta.size.w; x += _asepriteData.frames[0].sourceSize.w)
                {
                    Rectangle source = new Rectangle(
                        x,
                        y,
                        _asepriteData.frames[0].sourceSize.w,
                        _asepriteData.frames[0].sourceSize.h);
                    frames.Add(index, source);

                    /*Color[] pixels = new Color[source.Width * source.Height];
                    _currentTexture.GetData<Color>(0, source, pixels, 0, pixels.Length);

                    // Maybe store all frames as RGBA arrays?
                    Texture2D texture = new Texture2D(graphics, source.Width, source.Height);
                    texture.SetData<Color>(pixels);
                    frames.Add(index, texture);*/
                    index++;
                }
            }

            _animations = new Dictionary<string, List<Rectangle>>();
            foreach(var frameTag in _asepriteData.meta.frameTags.ToArray())
            {
                //System.Console.WriteLine("name: {0}, from: {1}, to: {2}", frameTag.name, frameTag.from, frameTag.to);
                List<Rectangle> rectangles = new List<Rectangle>();
                for(int i = frameTag.from; i <= frameTag.to; i++)
                {
                    rectangles.Add(frames[i]);
                }
                _animations.Add(frameTag.name, rectangles);
           }
        }

        public Texture2D Texture()
        {
            return _currentTexture;
        }

        public void Play(string name)
        {
            if(!_isAnimated)
                return;
            if(string.IsNullOrEmpty(name))
                throw new System.NullReferenceException("Can not be null!");

            _currentAnimation = name;
            _currentFrame = _currentAnimation == name ? _currentFrame : 0;
        }

        public void Update(GameTime gameTime)
        {
            int frequency = (int) (gameTime.TotalGameTime.TotalMilliseconds % _asepriteData.frames[_currentFrame].duration);
            if(frequency == 0)
                _currentFrame++;

            if(_currentFrame > _animations[_currentAnimation].ToArray().Length - 1)
                _currentFrame = 0;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            _currentSource = _animations[_currentAnimation].ToArray().ElementAt(_currentFrame);
            spriteBatch.Draw(
                texture: _currentTexture,
                position: Position,
                sourceRectangle: _currentSource,
                color: Color.White,
                rotation: Rotation,
                origin: Origin,
                scale: Scale,
                effects: SpriteEffect,
                layerDepth: 0
            );
        }
    }
}