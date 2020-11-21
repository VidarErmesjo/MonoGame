using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Animations;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame
{
    public class MonoGame: Game
    {
        // Render
        private GraphicsDeviceManager _graphics;
        public static SpriteBatch spriteBatch { get; private set; }

        // Input
        public static MouseState mouseState { get; private set; }
        public static MouseState previousMouseState { get; private set; }
        public static KeyboardState keyboardState { get; private set; }
        public static KeyboardState previousKeyboardState { get; private set; }
        public static GamePadState gamePadState { get; private set; }
        public static GamePadState previousGamePadState { get; private set; }

        // Fonts
        public static SpriteFont spriteFontConsolas { get; private set; }

        // Temp
        public SpriteSheet _spriteSheet;

        // ESC
        public static World world { get; private set; }
        public static OrthographicCamera camera { get; private set; }

        public static Entity player { get; set; }
        //private List<Entity> _entities { get; set; }

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
                _graphics.PreferredBackBufferWidth = (int) w / 2;
                _graphics.PreferredBackBufferHeight = (int) h / 2;
            }

            IsFixedTimeStep = true;            
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            // Input
            mouseState = new MouseState();
            previousMouseState = mouseState;
            IsMouseVisible = true;
            keyboardState = new KeyboardState();
            previousKeyboardState = keyboardState;
            gamePadState = new GamePadState();
            previousGamePadState = gamePadState;

            // Camera
            camera = new OrthographicCamera(GraphicsDevice);

            // Sprites
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Initialize()
        {
            // World
            world = new WorldBuilder()
                .AddSystem(new PlayerSystem())
                .AddSystem(new RenderSystem(GraphicsDevice))
                .AddSystem(new TestSystem(GraphicsDevice))
                .AddSystem(new HUDSystem(GraphicsDevice))
                .Build();

            // Entities
            player = world.CreateEntity();
            player.Attach(new Transform2(new Vector2(
                _graphics.PreferredBackBufferWidth * 0.5f,
                _graphics.PreferredBackBufferHeight * 0.5f)));
            player.Attach(new TestComponent(null, 0));
            player.Attach(new OrthographicCamera(GraphicsDevice));

            world.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Mouse cursor
            var cursor = new Texture2D(GraphicsDevice, 16, 16);
            Color[] colors = Enumerable.Range(0, cursor.Width * cursor.Height)
                .Select(i => Color.Red).ToArray();
            cursor.SetData<Color>(colors);
            Mouse.SetCursor(MouseCursor.FromTexture2D(cursor, 0, 0));

            // Fonts
            // Put in dictionary
            spriteFontConsolas = Content.Load<SpriteFont>("Fonts/Consolas");

            // Entities
            var texture = Content.Load<Texture2D>("Sprites/Shitsprite");
            player.Attach(new Sprite(new TextureRegion2D(
                texture,
                0,
                0,
                texture.Width,
                texture.Height)));
                
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            // Expand for more gamepads
            gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // <Camera>
            var direction = new Vector2(0.0f, 0.0f);
            direction.X = keyboardState.IsKeyDown(Keys.Left) ? -1.0f :
                keyboardState.IsKeyDown(Keys.Right) ? 1.0f : 0.0f;  
            direction.Y = keyboardState.IsKeyDown(Keys.Up) ? -1.0f :
                keyboardState.IsKeyDown(Keys.Down) ? 1.0f : 0.0f;  
            direction.Normalize();

            if(!direction.IsNaN())
                camera.Move(direction * gameTime.ElapsedGameTime.Milliseconds);
            // </Camera>


            world.Update(gameTime);
            base.Update(gameTime);

            previousMouseState = mouseState;
            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            world.Draw(gameTime);

            /*spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.End();*/          
            base.Draw(gameTime);
        }
    }
}
