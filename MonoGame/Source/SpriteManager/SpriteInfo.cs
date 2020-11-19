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
    public class SpriteInfo
    {
        // public readonly int frameWidth; osv.. versjon to, ikke-kvadratiske sprites. Æsj!?
        public readonly int frameSize;
        public readonly int numFrames;
        public readonly int framesPerRow;

        public readonly string[] name;

        public SpriteInfo(int frameSize = 16, int numFrames = 1, int framesPerRow = 1)
        {
            if(frameSize < 16 || numFrames < 16)
                return;
            this.frameSize = frameSize;
            this.numFrames = numFrames;
            this.framesPerRow = framesPerRow;
        }

        public SpriteInfo(Texture2D spriteSheet)
        {
            if(spriteSheet.Width < 16 || spriteSheet.Height < 16)
                return;
            frameSize = spriteSheet.Height;
            numFrames = spriteSheet.Width / spriteSheet.Height;
            framesPerRow = 1;

        }
    }
}