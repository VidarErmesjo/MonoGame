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
            bool isFullscreen = false;
            Size virtualResolution = new Size(3840, 2160);
            Size deviceResolution = new Size(1920, 1080);

            if(args.Length > 0)
            {
                foreach(string argument in args)
                {
                    switch(argument)
                    {
                        case "--fullscreen":
                                isFullscreen = true;
                            break;
                        case "--windowed":
                                isFullscreen = false;
                            break;
                        default:
                            break;
                    }
                }
            }

            using (var game = new MonoGame(
                virtualResolution,
                deviceResolution,
                isFullscreen))
                game.Run();
        }
    }
}
