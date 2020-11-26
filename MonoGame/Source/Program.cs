using System;
using Microsoft.Xna.Framework;

namespace MonoGame
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            bool isFullscreen = false;
            Point source = new Point(3840, 2160);
            Point destination = new Point(1920, 1080);

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
                source,
                destination,
                isFullscreen))
                game.Run();
        }
    }
}
