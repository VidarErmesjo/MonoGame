using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Aseprite;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MonoGame
{
    class Assets
    {
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();

        public static int Count { get; private set; }

        private static string AssetsPath(string path, string filename, string rootDir)
        {
            return Path.Combine(path.Substring(path.IndexOf(rootDir) + rootDir.Length), filename).Replace('\\', '/').Substring(1);
        }

        public static Texture2D Texture(string name)
        {
            return Assets.textures[name];
        }

        public static SpriteFont Font(string name)
        {
            return Assets.fonts[name];
        }

        public static void LoadAllAssets(ContentManager content)
        {
            Count = 0;
            Assets.textures = LoadAssets<Texture2D>(content, "Sprites");
            Assets.fonts = LoadAssets<SpriteFont>(content, "Fonts");
        }

        public static Dictionary<string, T> LoadAssets<T>(ContentManager content, string directory)
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
                    "Asset #{0} loaded: {1} ({2})",
                    Count,
                    asset.Split('/').Last(),
                    asset.Split('/').First());    
                            
                tmp.Add(asset.Split('/').Last(), content.Load<T>(asset));
            }

            return tmp;
        }
    }
}