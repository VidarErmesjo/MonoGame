using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Entities;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame
{
    public class Core : IDisposable
    {
        private bool isDisposed = false;

        private static bool _setup = false;
        private static bool _initialized = false;

        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public static GameWindow Window { get; private set; }
        public static ContentManager Content { get; private set; }

        public static Size VirtualResolution { get; private set; }
        public static Size TargetResolution { get; private set; }
        public static RenderTarget2D MainRenderTarget { get; private set; }
        public static Rectangle TargetRectangle { get; private set; }
        public static float ScaleToDevice { get; private set; }
        public static bool IsFullScreen { get; private set; }
        public static bool LowResolution { get; private set; }

        public static OrthographicCamera Camera { get; private set; }
        public static BoxingViewportAdapter ViewportAdapter { get; private set; }

        public static CollisionComponent CollisionComponent { get; set; }

        public static MouseState MouseState { get; private set; }
        public static MouseState PreviousMouseState { get; private set; }
        public static KeyboardState KeyboardState { get; private set; }
        public static KeyboardState PreviousKeyboardState { get; private set; }
        public static GamePadState GamePadState { get; private set; }
        public static GamePadState PreviousGamePadState { get; private set; }

        private const float _meterToSpriteRatio = 1f;
        public static float SpriteSize { get; private set; }
        public static float SpriteScale { get; private set; }
        public static float MetersPerPixel { get; private set; }
        public static Size2 MetersPerScreen { get; private set; }

        public Core(MonoGame game)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(game);
            game.IsMouseVisible = true;
            game.IsFixedTimeStep = true;
            game.Content.RootDirectory = "Content";

            Content = game.Content;
            Window = game.Window;
         }

        public static void Setup(Size resolution = default(Size), bool isFullscreen = true)
        {
            if(resolution == default(Size))
                resolution = new Size(3840, 2160);

            VirtualResolution = resolution;
            IsFullScreen = isFullscreen;

            _setup = true;
        }

        public static void Initialize()
        {
            if(!_setup)
                throw new InvalidOperationException("Core.Setup() must preceed Core.Initialize()");

            if(_initialized)
                return;

            Window.AllowUserResizing = false;

            LowResolution = false;

            if(IsFullScreen)
                TargetResolution = new Size(
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            else
                TargetResolution = new Size(
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2,
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2);

            TargetRectangle = new Rectangle(0, 0, TargetResolution.Width, TargetResolution.Height);

            GraphicsDeviceManager.PreferredBackBufferWidth = TargetResolution.Width;
            GraphicsDeviceManager.PreferredBackBufferHeight = TargetResolution.Height;
            GraphicsDeviceManager.IsFullScreen = IsFullScreen;
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            GraphicsDeviceManager.HardwareModeSwitch = false;
            GraphicsDeviceManager.ApplyChanges();
            GraphicsDeviceManager.HardwareModeSwitch = true;

            MainRenderTarget = new RenderTarget2D(
                graphicsDevice: GraphicsDeviceManager.GraphicsDevice,
                width: Core.VirtualResolution.Width,
                height: Core.VirtualResolution.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                preferredMultiSampleCount: GraphicsDeviceManager.GraphicsDevice.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents);

            ViewportAdapter = new BoxingViewportAdapter(
                Window,
                GraphicsDeviceManager.GraphicsDevice,
                VirtualResolution.Width,
                VirtualResolution.Height);
            Camera = new OrthographicCamera(ViewportAdapter);

            var scaleX = VirtualResolution.Width / (float) TargetResolution.Width;
            var scaleY = VirtualResolution.Height / (float) TargetResolution.Height;
            ScaleToDevice = (float) Math.Sqrt(scaleX * scaleX + scaleY * scaleY);

            MouseState = new MouseState();
            PreviousMouseState = new MouseState();
            Mouse.SetCursor(MouseCursor.Crosshair);
            KeyboardState = new KeyboardState();
            PreviousKeyboardState = new KeyboardState();
            GamePadState = new GamePadState();
            PreviousGamePadState = new GamePadState();

            SpriteSize = 16 * _meterToSpriteRatio;
            SpriteScale = 1f;
            MetersPerPixel = 1 / (SpriteSize * SpriteScale);
            MetersPerScreen = new Size2(
                VirtualResolution.Width,
                VirtualResolution.Height) * MetersPerPixel;

            _initialized = true;

            System.Console.WriteLine("Core.Initialize() => OK");
            System.Console.WriteLine("VirtualResolution => {0}, TargetResolution => {1}, ScaleToDevice => {2}", VirtualResolution, TargetResolution, ScaleToDevice);
        }

        public void ToggleRenderQuality()
        {
            LowResolution = !LowResolution;
        }

        public void Update()
        {
            PreviousMouseState = MouseState;
            PreviousKeyboardState = KeyboardState;
            PreviousGamePadState = GamePadState;
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
            if(isDisposed)
                return;

            if(disposing)
            {
                CollisionComponent.Dispose();
                GraphicsDeviceManager.Dispose();
                MainRenderTarget.Dispose();
                ViewportAdapter.Dispose();
                System.Console.WriteLine("Core.Dispose() => OK");
            }

            isDisposed = true;
        }

        ~Core()
        {
            Dispose(false);
        }

    }
}