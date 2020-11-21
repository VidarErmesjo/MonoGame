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
        private RenderTarget2D _renderTarget { get; set; }
        private SpriteBatch _spriteBatch { get; set; }

        // Input
        public static MouseState mouseState { get; private set; }
        public static MouseState previousMouseState { get; private set; }
        public static KeyboardState keyboardState { get; private set; }
        public static KeyboardState previousKeyboardState { get; private set; }
        public static GamePadState gamePadState { get; private set; }
        public static GamePadState previousGamePadState { get; private set; }

        // Fonts => make Array of SpriteFonts
        public static SpriteFont spriteFontConsolas { get; private set; }

        // Temp =>  Sprite/Animation Atlas
        public SpriteSheet _spriteSheet;

        public static Sprite arrowSprite;

        // ESC
        public static World world { get; private set; }
        public static OrthographicCamera camera { get; private set; }

        // Make Array of players, npcs, etc.
        public static Entity player { get; set; }
        //private List<Entity> _entities { get; set; }

        private float _scale = 1.0f;
        private float _rotation = 0.0f;

        // Experimental
        public enum Weapon {
            None = 0,
            Something
        };

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
            Mouse.SetCursor(MouseCursor.Crosshair);
            keyboardState = new KeyboardState();
            previousKeyboardState = keyboardState;
            gamePadState = new GamePadState();
            previousGamePadState = gamePadState;

            // Camera
            camera = new OrthographicCamera(GraphicsDevice);
        }

        protected override void LoadContent()
        {
            Texture2D texture = Content.Load<Texture2D>("Sprites/Compass");
            TextureRegion2D textureRegion = new TextureRegion2D(texture, 0, 0, texture.Width, texture.Height);
            arrowSprite = new Sprite(textureRegion);

            // Fonts
            // Put in dictionary
            spriteFontConsolas = Content.Load<SpriteFont>("Fonts/Consolas");

            // Entities
            texture = Content.Load<Texture2D>("Sprites/Shitsprite");
            player.Attach(new Sprite(new TextureRegion2D(
                texture,
                0,
                0,
                texture.Width,
                texture.Height)));
                
            base.LoadContent();
        }

        protected override void Initialize()
        {
            // Internal resolution = 4K
            _renderTarget = new RenderTarget2D(
                GraphicsDevice,
                3840,
                2160);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // World
            world = new WorldBuilder()
                .AddSystem(new RenderSystem(GraphicsDevice))
                .AddSystem(new HUDSystem(GraphicsDevice))
                .AddSystem(new ControllerSystem())
                .AddSystem(new WeaponSystem())
                .Build();

            // Entities
            player = world.CreateEntity();
            player.Attach(new Transform2(new Vector2(
                GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f,
                GraphicsDevice.PresentationParameters.BackBufferHeight * 0.5f)));
            player.Attach(new WeaponComponent(Weapon.None, 0));

            world.Initialize();
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            // Expand for more gamepads
            gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            /*if(camera.Zoom < camera.MaximumZoom)
                camera.ZoomIn(0.01f);
            else camera.Zoom = 0.0f;*/

            var worldPosition = MonoGame.camera.ScreenToWorld(new Vector2(MonoGame.mouseState.X, MonoGame.mouseState.Y));
            var direction = worldPosition - MonoGame.camera.Center;
            _rotation = direction.ToAngle();

            world.Update(gameTime);
            base.Update(gameTime);

            previousMouseState = mouseState;
            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SetRenderTarget(_renderTarget);
            world.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);

            /*if(_rotation < System.Math.PI * 2)
                _rotation += 0.0f;
            else
                _rotation = 0.0f;*/

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                samplerState: SamplerState.PointClamp,
                transformMatrix: null);

            _spriteBatch.Draw(
                texture: _renderTarget,
                position: camera.Center,
                sourceRectangle: GraphicsDevice.PresentationParameters.Bounds,
                color: Color.White,
                rotation: 0,
                origin: camera.Center,
                scale: _scale,
                effects: SpriteEffects.None,
                layerDepth: 0);
            _spriteBatch.End();     
            base.Draw(gameTime);
        }
    }
}
