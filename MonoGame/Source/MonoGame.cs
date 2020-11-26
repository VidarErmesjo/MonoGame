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
        private GraphicsDeviceManager _graphicsDeviceManager;
        private readonly Point _internalRenderBounds;
        private RenderTarget2D _internalRenderTarget { get; set; }
        private SpriteBatch _spriteBatch { get; set; }

        public static ContentManager content { get; private set; }
        public static ViewportAdapter viewportAdapter { get; private set; }

        // Input
        public static MouseState mouseState { get; private set; }
        public static MouseState previousMouseState { get; private set; }
        public static KeyboardState keyboardState { get; private set; }
        public static KeyboardState previousKeyboardState { get; private set; }
        public static GamePadState gamePadState { get; private set; }
        public static GamePadState previousGamePadState { get; private set; }

        // Fonts
        public static IDictionary<string, SpriteFont> spriteFonts { get; private set; }
 
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

        public MonoGame(Point source, Point destination, bool fullscreen)
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            if(fullscreen)
            {
                _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                _graphicsDeviceManager.IsFullScreen = true;
            }
            else
            {
                _graphicsDeviceManager.PreferredBackBufferWidth = destination.X;
                _graphicsDeviceManager.PreferredBackBufferHeight = destination.Y;
            }

            _internalRenderBounds = source;
            
            Window.AllowUserResizing = true;

            IsFixedTimeStep = true;            
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            _graphicsDeviceManager.ApplyChanges();

            Content.RootDirectory = "Content";
            content = Content;

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

            spriteFonts = new Dictionary<string, SpriteFont>();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _internalRenderTarget.Dispose();
                _internalRenderTarget = null;
                _spriteBatch.Dispose();
                _spriteBatch = null;
                _graphicsDeviceManager.Dispose();
                _graphicsDeviceManager = null;
                viewportAdapter.Dispose();
                viewportAdapter = null;
                world.Dispose();
                world = null;
            }

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            Assets.LoadAllAssets(Content);

            // HUD components
            arrowSprite = new AsepriteSprite("Compass");
            arrowSprite.Position = _internalRenderBounds.ToVector2() * 0.5f;
            arrowSprite.Scale = 0.032f;
            arrowSprite.Origin = new Vector2(
                arrowSprite.Texture().Width * 0.5f,
                arrowSprite.Texture().Height * 0.5f);
            arrowSprite.SpriteEffect = SpriteEffects.FlipVertically;

            aseprite = new AsepriteSprite("Shitsprite");
            aseprite.Play("Walk");
            aseprite.Position = Vector2.One * 100;
            aseprite.Scale = 4.0f;
            aseprite.SpriteEffect = SpriteEffects.FlipVertically;

            base.LoadContent();
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Camera
            viewportAdapter = new BoxingViewportAdapter(
                Window,
                _graphicsDeviceManager.GraphicsDevice,
                _internalRenderBounds.X,
                _internalRenderBounds.Y);
            camera = new OrthographicCamera(viewportAdapter);

            _internalRenderTarget = new RenderTarget2D(
                _graphicsDeviceManager.GraphicsDevice,
                _internalRenderBounds.X,
                _internalRenderBounds.Y,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                preferredMultiSampleCount: _graphicsDeviceManager.GraphicsDevice.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents);

            _spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);

            // World
            world = new WorldBuilder()
                .AddSystem(new RenderSystem(_graphicsDeviceManager.GraphicsDevice))
                .AddSystem(new HUDSystem(_graphicsDeviceManager.GraphicsDevice))
                .AddSystem(new ControllerSystem())
                .AddSystem(new WeaponSystem())
                .Build();

            // Entities
            player[(int) Player.One] = world.CreateEntity();
            player[(int) Player.One].Attach(aseprite);
            player[(int) Player.One].Attach(new Transform2(new Vector2(
                camera.Center.X,
                camera.Center.Y)));
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
            float preferredAspectRatio = _internalRenderBounds.X / (float) _internalRenderBounds.Y;
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
            _graphicsDeviceManager.GraphicsDevice.Clear(Color.DarkSlateGray);

            _graphicsDeviceManager.GraphicsDevice.SetRenderTarget(_internalRenderTarget);
            _graphicsDeviceManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            world.Draw(gameTime);
            _graphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: BlendState.Opaque,
                samplerState: SamplerState.PointClamp);

               _spriteBatch.Draw(_internalRenderTarget, FitToScreen(), Color.White);
;
            _spriteBatch.End(); 
            base.Draw(gameTime);    
        }
    }
}
