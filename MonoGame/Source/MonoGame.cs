using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Aseprite;
using MonoGame.Tools;

namespace MonoGame
{
    public class MonoGame: Game
    {
        // Graphics
        private readonly GraphicsDeviceManager _graphics;
        private readonly Size _virtualResolution;
        private RenderTarget2D _primaryRenderTarget { get; set; }
        private SpriteBatch _spriteBatch { get; set; }

        public static ViewportAdapter viewportAdapter { get; private set; }

        // Input
        public static MouseState mouseState { get; private set; }
        public static MouseState previousMouseState { get; private set; }
        public static KeyboardState keyboardState { get; private set; }
        public static KeyboardState previousKeyboardState { get; private set; }
        public static GamePadState gamePadState { get; private set; }
        public static GamePadState previousGamePadState { get; private set; }

        // Weather
        public static float WindSpeed = -0.1f;

        // Animation test
        public static AsepriteSprite arrowSprite;

        // ESC
        public static World world { get; private set; }
        public static OrthographicCamera camera { get; private set; }

        // Make Array of players, npcs, etc.
        public static Entity[] player { get; set; }
        public static Entity entity { get; set;}
        //private List<Entity> _entities { get; set; }

        //private float _scale = 1.0f;
        public static float rotation { get; private set; }

        public static AsepriteSprite aseprite;

        // Experimental
        public enum Weapon {
            None = 0,
            Something
        };

        public enum Player {
            One = 0,
            Two,
            Tree,
            Four
        }

        public MonoGame(Size virtualResolution = default(Size), Size deviceResolution = default(Size), bool fullscreen = true)
        {
            _graphics = new GraphicsDeviceManager(this);
            if(fullscreen)
            {
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                _graphics.IsFullScreen = true;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = deviceResolution.Width;
                _graphics.PreferredBackBufferHeight = deviceResolution.Height;
            }

            _virtualResolution = virtualResolution;
            
            Window.AllowUserResizing = true;

            IsFixedTimeStep = true;            
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            // Input
            mouseState = new MouseState();
            previousMouseState = mouseState;
            IsMouseVisible = true;
            Mouse.SetCursor(MouseCursor.Crosshair);
            keyboardState = new KeyboardState();
            previousKeyboardState = keyboardState;
            gamePadState = new GamePadState();
            previousGamePadState = gamePadState;

            // Stuffz
            player = new Entity[4];

            Globals.Initialize(this, virtualResolution, deviceResolution);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _primaryRenderTarget.Dispose();
                _spriteBatch.Dispose();
                _graphics.Dispose();
                viewportAdapter.Dispose();
                world.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            Assets.LoadAllAssets(Content);

            // HUD components
            arrowSprite = new AsepriteSprite("Compass");
            arrowSprite.Position = new Vector2(_virtualResolution.Width, _virtualResolution.Height) * 0.5f;
            arrowSprite.Scale = 0.032f;
            arrowSprite.Origin = new Vector2(
                arrowSprite.Rectangle.Width * 0.5f,
                arrowSprite.Rectangle.Height * 0.5f);
            arrowSprite.SpriteEffect = SpriteEffects.FlipVertically;

            aseprite = new AsepriteSprite("Shitsprite");
            aseprite.SpriteEffect = SpriteEffects.FlipVertically;

            base.LoadContent();
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Camera
            viewportAdapter = new BoxingViewportAdapter(
                Window,
                _graphics.GraphicsDevice,
                _virtualResolution.Width,
                _virtualResolution.Height);
            camera = new OrthographicCamera(viewportAdapter);

            _primaryRenderTarget = new RenderTarget2D(
                _graphics.GraphicsDevice,
                _virtualResolution.Width,
                _virtualResolution.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                preferredMultiSampleCount: _graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents);

            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

            // World
            world = new WorldBuilder()
                .AddSystem(new RenderSystem(_graphics.GraphicsDevice))
                .AddSystem(new WeatherSystem(_graphics.GraphicsDevice))
                .AddSystem(new HUDSystem(_graphics.GraphicsDevice))
                .AddSystem(new ControllerSystem())
                .AddSystem(new WeaponSystem())
                .AddSystem(new ExpirySystem())
                .AddSystem(new RainfallSystem())
                .Build();
            Components.Add(world);

            // Entities
            player[(int) Player.One] = world.CreateEntity();
            player[(int) Player.One].Attach(new AsepriteSprite("Shitsprite"));
            player[(int) Player.One].Attach(new WeaponComponent(Weapon.None, 0));

            world.Initialize();
            //base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);
            previousMouseState = mouseState;
            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            // Expand for more gamepads?
            gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            /*if(camera.Zoom < 10.0f)
                camera.ZoomIn(0.001f);
            else camera.Zoom = 0.0f;*/

            var direction = MonoGame.camera.ScreenToWorld(
                new Vector2(MonoGame.mouseState.X, MonoGame.mouseState.Y)) - MonoGame.camera.Center;
            direction.Normalize();
            rotation = direction.ToAngle();

            world.Update(gameTime);
            base.Update(gameTime);
        }

        private Rectangle FitToScreen()
        {
            Rectangle destinationRectangle;
            float preferredAspectRatio = _virtualResolution.Width / (float) _virtualResolution.Height;
            float outputAspectRatio = Window.ClientBounds.Width / (float) Window.ClientBounds.Height;

            if(outputAspectRatio == preferredAspectRatio)
                destinationRectangle = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            else if(outputAspectRatio <= preferredAspectRatio)
            {
                int presentHeight = (int) ((Window.ClientBounds.Width / preferredAspectRatio) + 0.5f);
                int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;
                destinationRectangle = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
                if(barHeight > 0)
                    System.Console.WriteLine(barHeight);
            }
            else
            {
                int presentWidth = (int) ((Window.ClientBounds.Height * preferredAspectRatio) + 0.5f);
                int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;
                destinationRectangle = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
                if(barWidth > 0)
                    System.Console.WriteLine(barWidth);
            }

            return destinationRectangle;
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.DarkSlateGray);

            _graphics.GraphicsDevice.SetRenderTarget(_primaryRenderTarget);
            _graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            world.Draw(gameTime);
            _graphics.GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: BlendState.Opaque,
                samplerState: SamplerState.PointClamp);

               _spriteBatch.Draw(_primaryRenderTarget, FitToScreen(), Color.White);
;
            _spriteBatch.End(); 
            base.Draw(gameTime);    
        }
    }
}
