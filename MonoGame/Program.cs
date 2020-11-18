using System;

namespace MonoGame
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            bool fullscreen = false;

            if(args.Length > 0)
            {
                foreach(string argument in args)
                {
                    switch(argument)
                    {
                        case "--fullscreen":
                                fullscreen = true;
                            break;
                        case "--windowed":
                                fullscreen = false;
                            break;
                        default:
                            break;
                    }
                }
            }

            using (var game = new MonoGame(3840, 2160, fullscreen))
                game.Run();
        }
    }
}
