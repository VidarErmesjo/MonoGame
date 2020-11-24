using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Aseprite;

namespace MonoGame
{
    public class MonoGame: Game
    {
        // Render
        private GraphicsDeviceManager _graphics;
        private RenderTarget2D _internalRenderTarget { get; set; }
        private readonly Vector2 _internalRenderBounds;
        private SpriteBatch _spriteBatch { get; set; }

        // Why??
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
        public static AnimatedSprite animatedSprite { get; private set; }

        public static Sprite arrowSprite;

        // ESC
        public static World world { get; private set; }
        public static OrthographicCamera camera { get; private set; }

        // Make Array of players, npcs, etc.
        public static Entity[] player { get; set; }
        //private List<Entity> _entities { get; set; }

        private float _scale = 1.0f;
        public static float rotation { get; private set; }

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

        public MonoGame(int w, int h, bool fullscreen)
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
                _graphics.PreferredBackBufferWidth = (int) w;
                _graphics.PreferredBackBufferHeight = (int) h;
            }

            _internalRenderBounds = new Vector2(w, h);
            
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
                _graphics.Dispose();
                _graphics = null;
                viewportAdapter.Dispose();
                viewportAdapter = null;
                world.Dispose();
                world = null;
            }

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            // HUD components
            Texture2D texture = Content.Load<Texture2D>("Sprites/Compass");
            arrowSprite = new Sprite(texture, new Vector2(
                GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f,
                GraphicsDevice.PresentationParameters.BackBufferHeight * 0.5f));
            arrowSprite.RenderDefinition.Scale = Vector2.One * 0.032f;
            arrowSprite.RenderDefinition.Origin = new Vector2(
                arrowSprite.Texture.Width * 0.5f,
                arrowSprite.Texture.Height * 0.5f);
            arrowSprite.RenderDefinition.SpriteEffect = SpriteEffects.FlipVertically;

            // Animation test
            // Make functional loader? animatedSprite = AnimatedSpriteloader(string file)
            AnimationDefinition animationDefinition = Content.Load<AnimationDefinition>("Sprites/ShitspriteAnimation");
            Texture2D spriteSheet = Content.Load<Texture2D>("Sprites/Shitsprite");
                animatedSprite = new AnimatedSprite(spriteSheet,
                animationDefinition,
                Vector2.One * GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f);
            /*animatedSprite = Tools.AnimatedSpriteLoader(
                Content,
                Vector2.One * GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f,
                4);*/
            animatedSprite.RenderDefinition.SpriteEffect = SpriteEffects.FlipVertically;
            animatedSprite.RenderDefinition.Scale = Vector2.One * 4;
            animatedSprite.RenderDefinition.Origin = new Vector2(
                animatedSprite.CurrentFrame.frame.Width * 0.5f,
                animatedSprite.CurrentFrame.frame.Height * 0.5f);

            // Fonts
            spriteFonts.Add("Consolas", Content.Load<SpriteFont>("Fonts/Consolas"));
            spriteFonts.Add("Arial", Content.Load<SpriteFont>("Fonts/Arial"));
        }

        protected override void Initialize()
        {
            base.Initialize();
            System.Console.WriteLine("CurrentDisplayMode: " + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.ToString());
            System.Console.WriteLine("Render: " + _internalRenderBounds.ToString());
            System.Console.WriteLine("Window: " + Window.ClientBounds.ToString());

            // Camera
            viewportAdapter = new BoxingViewportAdapter(
                Window,
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);
            camera = new OrthographicCamera(viewportAdapter);

            // Internal resolution = 4K
            _internalRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                (int) _internalRenderBounds.X,
                (int) _internalRenderBounds.Y,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                preferredMultiSampleCount: _graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents);

            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

            // World
            world = new WorldBuilder()
                .AddSystem(new RenderSystem(_graphics.GraphicsDevice))
                .AddSystem(new HUDSystem(_graphics.GraphicsDevice))
                .AddSystem(new ControllerSystem())
                .AddSystem(new WeaponSystem())
                .Build();

            // Entities
            player[(int) Player.One] = world.CreateEntity();
            // texture from atlas?
            Texture2D texture = Content.Load<Texture2D>("Sprites/Shitsprite");
            player[(int) Player.One].Attach(new Extended.Sprites.Sprite(new TextureRegion2D(
                texture,
                0,
                0,
                texture.Width,
                texture.Height)));
            player[(int) Player.One].Attach(new Transform2(new Vector2(
                _graphics.GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f,
                _graphics.GraphicsDevice.PresentationParameters.BackBufferHeight * 0.5f)));
            player[(int) Player.One].Attach(new WeaponComponent(Weapon.None, 0));

            world.Initialize();
            //base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            // Expand for more gamepads
            gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            animatedSprite.Play("Walk");
            animatedSprite.Update(gameTime);

            /*if(camera.Zoom < 10.0f)
                camera.ZoomIn(0.001f);
            else camera.Zoom = 0.0f;*/

            var worldPosition = MonoGame.camera.ScreenToWorld(new Vector2(MonoGame.mouseState.X, MonoGame.mouseState.Y));
            var direction = worldPosition - MonoGame.camera.Center;
            direction.Normalize();
            rotation = direction.ToAngle();

            //camera.LookAt(new Vector2(0.0f, 0.0f));
            System.Console.WriteLine("Window: " + Window.ClientBounds.ToString());

            world.Update(gameTime);

            base.Update(gameTime);
            previousMouseState = mouseState;
            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
        }

        private Rectangle FitToScreen()
        {
            Rectangle rectangle;
            float preferredAspectRatio = _internalRenderBounds.X / (float) _internalRenderBounds.Y;
            float outputAspectRatio = Window.ClientBounds.Width / (float) Window.ClientBounds.Height;

            // Letter boxing
            if(outputAspectRatio <= preferredAspectRatio)
            {
                int presentHeight = (int) ((Window.ClientBounds.Width / preferredAspectRatio) + 0.5f);
                int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;
                rectangle = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
            }
            else
            {
                int presentWidth = (int) ((Window.ClientBounds.Height * preferredAspectRatio) + 0.5f);
                int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;
                rectangle = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
            }

            return rectangle;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);
            GraphicsDevice.SetRenderTarget(_internalRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            world.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);

            /*if(rotation < System.Math.PI * 2)
                rotation += 0.0f;
            else
                rotation = 0.0f;*/

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
