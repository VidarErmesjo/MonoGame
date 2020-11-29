using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Aseprite;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MonoGame
{
    public class Assets : IDisposable
    {
        private bool isDisposed = false;

        private bool _hasLoaded = false;

        private static Dictionary<string, Texture2D> textures;
        private static Dictionary<string, SpriteFont> fonts;

        public int Count { get; private set; }

        public Assets()
        {
            textures = new Dictionary<string, Texture2D>();
            fonts = new Dictionary<string, SpriteFont>();
        }

        private string AssetsPath(string path, string filename, string rootDir)
        {
            return Path.Combine(path.Substring(path.IndexOf(rootDir) + rootDir.Length), filename).Replace('\\', '/').Substring(1);
        }

        public static Texture2D Texture(string name)
        {
            return textures[name];
        }

        public static SpriteFont Font(string name)
        {
            return fonts[name];
        }

        public void LoadAllAssets(ContentManager content)
        {
            if(_hasLoaded)
                return;

            Count = 0;
            textures = Load<Texture2D>(content, "Sprites");
            fonts = Load<SpriteFont>(content, "Fonts");

            _hasLoaded = true;
        }

        public Dictionary<string, T> Load<T>(ContentManager content, string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(content.RootDirectory, directory));
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            Dictionary<string, T> tmp = new Dictionary<string, T>();
            FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string asset = AssetsPath(
                    file.DirectoryName,
                    Path.GetFileNameWithoutExtension(file.Name),
                    content.RootDirectory);

                Count++;
                System.Console.WriteLine(
                    "Asset loaded: {0} [Content/{1}]",
                    asset.Split('/').Last(),
                    asset.Split('/').First());    
                            
                tmp.Add(asset.Split('/').Last(), content.Load<T>(asset));
            }

            return tmp;
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
                foreach(var texture in textures)
                {
                    texture.Value.Dispose();
                    System.Console.WriteLine("Asset disposed: {0}", texture.Key);
                }

                System.Console.WriteLine("Assets.Dispose() => OK");
            }

            isDisposed = true;
        }

        ~Assets()
        {
            Dispose(false);
        }
    }
}