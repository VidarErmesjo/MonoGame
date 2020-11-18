namespace SpriteManager
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
#if NETFX_CORE
    using System.Threading.Tasks;
#endif
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class SpriteSheetLoader
    {
        private readonly ContentManager _contentManager;
        private readonly GraphicsDevice _graphicsDevice;

        public SpriteSheetLoader(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
        }

        public SpriteSheet MultiLoad(string imageResourceFormat, int numSheets)
        {
            SpriteSheet result = new SpriteSheet();
            for (int i = 0; i < numSheets; i++)
            {
                string spriteResource = string.Format(imageResourceFormat, i);

                SpriteSheet tmp = Load(spriteResource);
                result.Add(tmp);
            }
            return result;
        }

        public SpriteSheet Load(string spriteResource)
        {
            Texture2D spriteSheet = _contentManager.Load<Texture2D>(spriteResource);
            string spriteData = Path.ChangeExtension(spriteResource, "csv");

            // Kommer liksom fra fil "csv" fil
            SpriteInfo spriteInfo = new SpriteInfo(spriteSheet);

            var counter = new Tools.Incrementor();
            SpriteSheet sheet = new SpriteSheet();
            Texture2D texture = null;
            Color[] destination;
            float scale, pivot;
            bool isRotated;
            Rectangle source;
            SpriteFrame sprite;
            int offset, spriteSize = spriteInfo.frameSize * spriteInfo.frameSize;
            for(int frame = 0; frame < spriteInfo.numFrames; frame++)
            {
                offset = spriteInfo.frameSize * frame;                
                source = new Rectangle(
                    offset,
                    0,
                    spriteInfo.frameSize,
                    spriteInfo.frameSize);

                destination = new Color[spriteSize];
                spriteSheet.GetData<Color>(0, 0, source, destination, 0, spriteSize);

                texture = new Texture2D(
                    _graphicsDevice,
                    spriteInfo.frameSize,
                    spriteInfo.frameSize); 
                texture.SetData<Color>(destination);

                pivot = spriteInfo.frameSize / 2.0f;
                scale = 1.0f;
                isRotated = false;

                sprite = new SpriteFrame(
                    texture,
                    new Rectangle(0, 0, spriteInfo.frameSize, spriteInfo.frameSize),
                    new Vector2(scale, scale),
                    new Vector2(pivot, pivot),
                    isRotated);

                // shitguy/walk/ skal komme fra spriteInfo
                int currentCount = counter.Tick();
                string name = "shitguy/walk/000" + currentCount;

                sheet.Add(name, sprite);
            }
            //texture.Dispose() => Fjerner siste ??
            spriteSheet.Dispose();
            return sheet;
        }

#if NETFX_CORE
        private string[] ReadDataFile(string dataFile)
        {
            var dataFileLines = ReadDataFileLines(dataFile);

            return dataFileLines.Result.ToArray();
        }

        private async Task<string[]> ReadDataFileLines(string dataFile)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            var file = await folder.GetFileAsync(dataFile).AsTask().ConfigureAwait(false);
            var fileContents = await Windows.Storage.FileIO.ReadLinesAsync(file).AsTask().ConfigureAwait(false);

            return fileContents.ToArray();
        }
#elif __ANDROID__
		private string[] ReadDataFile(string dataFile) {
			using(var ms = new MemoryStream()) {
				using (var s = Game.Activity.Assets.Open (dataFile)) {
					s.CopyTo (ms);
					return System.Text.Encoding.Default.GetString (ms.ToArray()).Split (new char[] { '\n'});
				}
			}
		}
#else
        private string[] ReadDataFile(string dataFile) 
        {
            return File.ReadAllLines(dataFile);
        }
#endif
    }

    /*public class SpriteSheetLoader
    {
        private readonly ContentManager contentManager;
        private readonly GraphicsDevice graphicsDevice;

        public SpriteSheetLoader(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            this.contentManager = contentManager;
            this.graphicsDevice = graphicsDevice;
        }

        public SpriteSheet MultiLoad(string imageResourceFormat, int numSheets)
        {
            SpriteSheet result = new SpriteSheet();
            for (int i = 0; i < numSheets; i++)
            {
                string spriteResource = string.Format(imageResourceFormat, i);

                SpriteSheet tmp = Load(spriteResource);
                result.Add(tmp);
            }
            return result;
        }


        public SpriteSheet Load(string spriteResource)
        {
            var imageFile = Path.Combine(contentManager.RootDirectory, spriteResource);
            var dataFile = Path.ChangeExtension(imageFile, "txt");

            FileStream fileStream = new FileStream(imageFile, FileMode.Open);
            var texture = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();


            var dataFileLines = ReadDataFile(dataFile);

            var sheet = new SpriteSheet();

            foreach (
                var cols in
                    from row in dataFileLines
                    where !string.IsNullOrEmpty(row) && !row.StartsWith("#")
                    select row.Split(';'))
            {
                if (cols.Length != 10)
                {
                    throw new InvalidDataException("Incorrect format data in spritesheet data file");
                }

                var isRotated = int.Parse (cols [1]) == 1;
                var name = cols[0];
                var source = new Rectangle(
                    int.Parse(cols[2]),
                    int.Parse(cols[3]),
                    int.Parse(cols[4]),
                    int.Parse(cols[5]));
                var scale = new Vector2(
                    int.Parse(cols[6]),
                    int.Parse(cols[7]));
                var pivot = new Vector2(
                    float.Parse(cols[8], CultureInfo.InvariantCulture),
                    float.Parse(cols[9], CultureInfo.InvariantCulture));
                var sprite = new SpriteFrame(texture, source, scale, pivot, isRotated);

                sheet.Add(name, sprite);
            }

            return sheet;
        }

#if NETFX_CORE
        private string[] ReadDataFile(string dataFile)
        {
            var dataFileLines = ReadDataFileLines(dataFile);

            return dataFileLines.Result.ToArray();
        }

        private async Task<string[]> ReadDataFileLines(string dataFile)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            var file = await folder.GetFileAsync(dataFile).AsTask().ConfigureAwait(false);
            var fileContents = await Windows.Storage.FileIO.ReadLinesAsync(file).AsTask().ConfigureAwait(false);

            return fileContents.ToArray();
        }
#elif __ANDROID__
		private string[] ReadDataFile(string dataFile) {
			using(var ms = new MemoryStream()) {
				using (var s = Game.Activity.Assets.Open (dataFile)) {
					s.CopyTo (ms);
					return System.Text.Encoding.Default.GetString (ms.ToArray()).Split (new char[] { '\n'});
				}
			}
		}
#else
        private string[] ReadDataFile(string dataFile) 
        {
            return File.ReadAllLines(dataFile);
        }
#endif
    }*/
}