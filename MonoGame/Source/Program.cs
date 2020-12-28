using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonoGame
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            bool fullscreen = true;
            Size resolution = new Size(3840, 2160);

            if(args.Length > 0)
            {
                foreach(string argument in args)
                {
                    switch(argument.ToLower())
                    {
                        case "--fullscreen":
                                fullscreen = true;
                            break;
                        case "--windowed":
                                fullscreen = false;
                            break;
                        case "--nes":
                            resolution = new Size(266, 240);
                            break;
                        case "--snes":
                            resolution = new Size(256,224);
                            break;
                        case "--genesis":
                            resolution = new Size(320, 244);
                            break;
                        case "--360p":
                            resolution = new Size(640, 360);
                            break;
                        case "--450p":
                            resolution = new Size(800, 450);
                            break;
                        case "--720p":
                            resolution = new Size(1280, 720);
                            break;
                        case "--1080p":
                            resolution = new Size(1920, 1080);
                            break;
                        default:
                            resolution = new Size(3840, 2160);
                            break;
                    }
                }
            }

            using (var game = new MonoGame(resolution, fullscreen))
                game.Run();
        }
    }
}
