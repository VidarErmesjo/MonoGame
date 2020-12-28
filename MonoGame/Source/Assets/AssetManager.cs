using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MonoGame.Assets
{
    interface IAssetManager : IDisposable
    {
    }

    public class AssetManager : IAssetManager
    {
        private bool isDisposed = false;

        private readonly ContentManager _content;

        private bool _hasLoaded = false;

        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, SpriteFont> fonts;
        private Dictionary<string, AsepriteDocument> sprites;

        public AssetManager(ContentManager content)
        {
            _content = content;
            textures = new Dictionary<string, Texture2D>();
            fonts = new Dictionary<string, SpriteFont>();
            sprites = new Dictionary<string, AsepriteDocument>();
        }

        private string AssetsPath(string path, string filename, string rootDir)
        {
            return Path.Combine(path.Substring(path.IndexOf(rootDir) + rootDir.Length), filename).Replace('\\', '/').Substring(1);
        }

        public Texture2D Texture(string name) => textures[name];

        public SpriteFont Font(string name) => fonts[name];
     
        public AsepriteDocument Sprite(string name) => sprites[name];

        public void LoadContent()
        {
            if(_hasLoaded)
                return;

            textures = Load<Texture2D>("Textures");
            sprites = Load<AsepriteDocument>("Aseprite");
            fonts = Load<SpriteFont>("Fonts");

            _hasLoaded = true;
            System.Console.WriteLine("AssetsManager.LoadAllAssets() => OK");
        }

        public Dictionary<string, T> Load<T>(string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(_content.RootDirectory, directory));
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            Dictionary<string, T> tmp = new Dictionary<string, T>();
            FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string asset = AssetsPath(
                    file.DirectoryName,
                    Path.GetFileNameWithoutExtension(file.Name),
                    _content.RootDirectory);

                System.Console.WriteLine(
                    "{0}.Load<T>() => Loaded",
                    asset.Split('/').Last());
                    //asset.GetType().FullName);    
                            
                tmp.Add(asset.Split('/').Last(), _content.Load<T>(asset));
            }

            return tmp;
        }

        public void UnloadContent()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
            {
                _content.Dispose();
                
                foreach(var texture in textures)
                {
                    texture.Value.Dispose();
                    System.Console.WriteLine("{0}.Dispose() => OK", texture.Key);
                }

                foreach(var sprite in sprites)
                {
                    sprite.Value.Dispose();
                    System.Console.WriteLine("{0}.Dispose() => OK", sprite.Key);
                }

                System.Console.WriteLine("AssetsManager.Dispose() => OK");
            }

            isDisposed = true;
        }

        ~AssetManager()
        {
            Dispose(false);
        }
    }
}