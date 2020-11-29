using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame
{
    public class Core : IDisposable
    {
        private bool isDisposing = false;

        private static MonoGame _game;
        private static Size _virtualResolution;
        private static Size _deviceResolution;
        private static bool _fullscreen = false;
        private static bool _graphics = false;
        private static bool _viewport = false;
        private static bool _input = false;
        private static bool _units = false;
        private static bool _initialized = false;

        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public static ContentManager Content { get; private set; }

        public static Size VirtualResolution { get; private set; }
        public static Size DeviceResolution { get; private set; }
        public static RenderTarget2D MainRenderTarget { get; private set; }

        public static OrthographicCamera Camera { get; private set; }
        public static BoxingViewportAdapter ViewportAdapter { get; private set; }

        public static MouseState MouseState { get; private set; }
        public static KeyboardState KeyboardState { get; private set; }
        public static GamePadState GamePadState { get; private set; }

        private const float _meterToSpriteRatio = 1f;
        public static float SpriteSize { get; private set; }
        public static float SpriteScale { get; private set; }
        public static float MetersPerPixel { get; private set; }
        public static Size2 MetersPerScreen { get; private set; }

        public Core(MonoGame game)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(game);
            Content = game.Content;
            _game = game;
        }

        public static void Initialize(Size virtualResolution = default(Size), Size deviceResolution = default(Size), bool fullscreen = false)
        {
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

            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            GraphicsDeviceManager.ApplyChanges();

            _virtualResolution = virtualResolution;
            _deviceResolution = deviceResolution;
            _fullscreen = fullscreen;
            _game.Window.AllowUserResizing = true;
            _game.IsMouseVisible = true;
            _game.IsFixedTimeStep = true;
            _game.Content.RootDirectory = "Content";

            if(!_graphics)
                Graphics();
            
            if(!_viewport)
                Viewport();
            
            if(!_input)
                Input();

            if(!_units)
                Units();

            _initialized = true;

            System.Console.WriteLine("VirtualResolution => {0}, DeviceResolution => {1}", _virtualResolution, _deviceResolution);
        }

        private static void Graphics()
        {
            if(_graphics)
                return;

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

            MainRenderTarget = new RenderTarget2D(
                _game.GraphicsDevice,
                _virtualResolution.Width,
                _virtualResolution.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                preferredMultiSampleCount: _game.GraphicsDevice.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents); 

            _graphics = true;
        }

        private static void Viewport()
        {
            if(_viewport)
                return;

            ViewportAdapter = new BoxingViewportAdapter(
                _game.Window,
                _game.GraphicsDevice,
                _virtualResolution.Width,
                _virtualResolution.Height);
            Camera = new OrthographicCamera(ViewportAdapter);

            _viewport = true;
        }

        private static void Input()
        {
            if(_input)
                return;

            MouseState = new MouseState();
            KeyboardState = new KeyboardState();
            GamePadState = new GamePadState();

            _game.IsMouseVisible = true;
            Mouse.SetCursor(MouseCursor.Crosshair);

            _input = true;
        }

        private static void Units()
        {
            if(_units)
                return;

            SpriteScale = 4f;
            SpriteSize = 16 * _meterToSpriteRatio;
            MetersPerPixel = 1 / (SpriteSize * SpriteScale);
            MetersPerScreen = new Size2(
                _virtualResolution.Width,
                _virtualResolution.Height) * MetersPerPixel;

            _units = true;
        }

        public void Update()
        {
            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();
            GamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if(isDisposing)
                return;

            if(disposing)
            {
                GraphicsDeviceManager.Dispose();
                MainRenderTarget.Dispose();
                ViewportAdapter.Dispose();
                System.Console.WriteLine("Core.Dispose() => OK");
            }
        }

        ~Core()
        {
            Dispose(false);
        }

    }
}