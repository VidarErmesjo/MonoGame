using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame
{
    public class Globals
    {
        private static MonoGame _game;
        private static Size _virtualResolution;
        private static Size _deviceResolution;
        private static bool _fullscreen = false;
        private static bool _setup = false;
        private static bool _initialized = false;

        public static GraphicsDeviceManager GraphicsDeviceManager;

        public class Rendering
        {
            public static Size VirtualResolution { get; private set; }
            public static Size DeviceResolution { get; private set; }
            public static RenderTarget2D RenderTarget { get; private set; }

            public static void Initialize()
            {
                if(!_initialized)
                {
                    Globals.Initialize(_virtualResolution, _deviceResolution);
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

                // Seems to letter box fine without RenderTarget (which is not even running now)?
                RenderTarget = new RenderTarget2D(
                    _game.GraphicsDevice,
                    _virtualResolution.Width,
                    _virtualResolution.Height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.None,
                    preferredMultiSampleCount: _game.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.DiscardContents);
            }

            public static void Dispose()
            {
                RenderTarget.Dispose();
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
                if(!_initialized)
                {
                    Globals.Initialize(_virtualResolution, _deviceResolution);
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
                if(!_initialized)
                {
                    Globals.Initialize(_virtualResolution, _deviceResolution);
                    return;
                }

                ViewportAdapter = new BoxingViewportAdapter(
                    _game.Window,
                    _game.GraphicsDevice,
                    Rendering.VirtualResolution.Width,
                    Rendering.VirtualResolution.Height);
                Camera = new OrthographicCamera(ViewportAdapter);
            }

            public static void Dispose()
            {
                ViewportAdapter.Dispose();
            }
        }

        public class Input
        {
            public static MouseState MouseState { get; private set; }
            public static KeyboardState KeyboardState { get; private set; }
            public static GamePadState GamePadState { get; private set; }

            public static void Initialize()
            {
                if(!_initialized)
                {
                    Globals.Initialize(_virtualResolution, _deviceResolution);
                    return;
                }

                MouseState = new MouseState();
                KeyboardState = new KeyboardState();
                GamePadState = new GamePadState();

                _game.IsMouseVisible = true;
                Mouse.SetCursor(MouseCursor.Crosshair);
            }

            public static void Update()
            {
                MouseState = Mouse.GetState();
                KeyboardState = Keyboard.GetState();
                GamePadState = GamePad.GetState(PlayerIndex.One);
            }
        }

        public static void Initialize(Size virtualResolution = default(Size), Size deviceResolution = default(Size), bool fullscreen = false)
        {
            if(!_setup)
                throw new NoSuitableGraphicsDeviceException("Did you forget to add 'Globals.Setup(this)' to MonoGame constructor?");

            if(_initialized)
                return;

            if(fullscreen)
            {
                GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                GraphicsDeviceManager.IsFullScreen = true;
            }
            else
            {
                GraphicsDeviceManager.PreferredBackBufferWidth = deviceResolution.Width;
                GraphicsDeviceManager.PreferredBackBufferHeight = deviceResolution.Height;
            }

            _virtualResolution = virtualResolution;
            _deviceResolution = deviceResolution;
            _fullscreen = fullscreen;
            _game.Window.AllowUserResizing = true;
            _game.IsMouseVisible = true;
            _game.IsFixedTimeStep = true;
            _game.Content.RootDirectory = "Content";
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            GraphicsDeviceManager.ApplyChanges();
            _initialized = true;

            Rendering.Initialize();
            Viewport.Initialize();
            Units.Initialize();
            Input.Initialize();
        }

        public static void Setup(MonoGame game)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(game);
            _game = game;
            _setup = true;
        }

        public static void Dispose()
        {
            GraphicsDeviceManager.Dispose();
            Rendering.Dispose();
            Viewport.Dispose();
        }

        /*public static ContentManager GetNewContentManager()
        {
            ContentManager contentManager = new ContentManager(
                _game.Content.ServiceProvider,
                _game.Content.RootDirectory);
            contentManager.RootDirectory = "Content";
            return contentManager;
        }*/
    }
}