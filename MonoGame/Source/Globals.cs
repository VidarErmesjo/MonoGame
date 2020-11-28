using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame
{
    public class Globals
    {
        private static bool _hasInitialized = false;

        public class Rendering
        {
            public static Size VirtualResolution { get; private set; }
            public static Size DeviceResolution { get; private set; }

            public static void Initialize()
            {
                if(!_hasInitialized)
                {
                    Globals.Initialize(_game);
                    return;
                }

                if(_virtualResolution == default(Size))
                    _virtualResolution = new Size(3840, 2160);

                if(_deviceResolution == default(Size))
                    _deviceResolution = new Size(
                        _game.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
                        _game.GraphicsDevice.Adapter.CurrentDisplayMode.Height);
                else _deviceResolution = new Size(
                    _game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    _game.GraphicsDevice.PresentationParameters.BackBufferHeight);
                
                VirtualResolution = _virtualResolution;
                DeviceResolution = _deviceResolution;
            }
        }

        public class Units
        {
            private const float _meterToSpriteRatio = 1f;
            public static float SpriteSize { get; private set; }
            public static float MetersPerPixel { get; private set; }
            public static Size2 MetersPerScreen { get; private set; }

            public static void Initialize()
            {
                if(!_hasInitialized)
                {
                    Globals.Initialize(_game);
                    return;
                }

                SpriteSize = 32 * _meterToSpriteRatio;
                MetersPerPixel = 1 / SpriteSize;
                MetersPerScreen = new Size2(
                    Rendering.VirtualResolution.Width,
                    Rendering.VirtualResolution.Height) * MetersPerPixel;
            }
        }

        public class Viewport
        {
            public static OrthographicCamera Camera { get; private set; }
            public static ViewportAdapter ViewportAdapter { get; private set; }

            public static void Initialize()
            {
                if(!_hasInitialized)
                {
                    Globals.Initialize(_game);
                    return;
                }

                ViewportAdapter = new BoxingViewportAdapter(
                    _game.Window,
                    _game.GraphicsDevice,
                    Rendering.VirtualResolution.Width,
                    Rendering.VirtualResolution.Height);
                Camera = new OrthographicCamera(ViewportAdapter);
            }
        }

        private static MonoGame _game;
        private static Size _virtualResolution;
        private static Size _deviceResolution;
        public static void Initialize(MonoGame game, Size virtualResolution = default(Size), Size deviceResolution = default(Size))
        {
            if(_hasInitialized)
                return;

            _game = game;
            _virtualResolution = virtualResolution;
            _deviceResolution = deviceResolution;
            _hasInitialized = true;

            Rendering.Initialize();
            Viewport.Initialize();
            Units.Initialize();
        }

        public static ContentManager content;
        public static ContentManager GetNewContentManager()
        {
            ContentManager contentManager = new ContentManager(
                content.ServiceProvider,
                content.RootDirectory);
            contentManager.RootDirectory = "Content";
            return contentManager;
        }

    }
}