namespace SpriteManager
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Graphics;

    public class SpriteSheet
    {
        private readonly IDictionary<string, SpriteFrame> _spriteList;

        public int Count { get; private set; }
        
        public SpriteSheet()
        {
            _spriteList = new Dictionary<string, SpriteFrame>();
            Count = 0;
        }

        public void Add(string name, SpriteFrame sprite)
        {
            _spriteList.Add(name, sprite);
            Count = _spriteList.Count;
        }

        public void Add(SpriteSheet otherSheet)
        {
            foreach (var sprite in otherSheet._spriteList)
            {
                _spriteList.Add(sprite);
            }
            Count = _spriteList.Count;
        }

        public SpriteFrame Sprite(string sprite)
        {
            return _spriteList[sprite];
        }

        public void SaveAsPngs()
        {
            var keyList = this.Keys();
            foreach(string key in keyList)
            {
                try
                {
                    string fileName = key.Replace('/','_') + ".png";
                    System.IO.Stream stream = null;
                    stream = System.IO.File.Create(fileName);
                    this.Sprite(key).Texture.SaveAsPng(stream, this.Sprite(key).Texture.Width, this.Sprite(key).Texture.Height);
                    stream.Dispose();
                }
                catch(System.Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }
            }
        }

        public ICollection<string> Keys()
        {
            return _spriteList.Keys;
        }

        public override string ToString()
        {
            var keyList = _spriteList.Keys;
            string stringList = "";

            foreach(string key in keyList)
            {
                stringList += key + "\n";
            }
            return stringList;
        }
    }
}