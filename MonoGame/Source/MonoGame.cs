using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.ViewportAdapters;
using SpriteManager;

namespace MonoGame
{
    public class MonoGame: Game
    {
        // Render
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private SpriteRender _spriteRender;

        //private Camera _camera
        private OrthographicCamera _camera;
        private Vector2 _worldPosition;

        // Input
        private KeyboardState _keyboardState, _previousKeyboardState;
        private MouseState _mouseState, _previousMouseState;

        // Eiendel av entitet?
        public SpriteManager.SpriteSheet spriteSheet;

        private Player _player;

        // ESC
        private World _world;
        private Entity player;
        //private List<Entity> _entities;

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
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // View
            var viewportAdapter = new BoxingViewportAdapter(
                Window,
                GraphicsDevice,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            _camera = new OrthographicCamera(viewportAdapter);
            _camera.Move(new Vector2(0.0f, 0.0f));
            _worldPosition = new Vector2(0.0f, 0.0f);

            // World
            _world = new WorldBuilder()
                .AddSystem(new PlayerSystem())
                .AddSystem(new RenderSystem(GraphicsDevice))
                .Build();


            // Entities
            var texture = Content.Load<Texture2D>("Sprites/Shitsprite_idle");

            _player = new Player();
            player = _world.CreateEntity();
            player.Attach(new Transform2(new Vector2(100, 100)));
            player.Attach(new Sprite(new Extended.TextureAtlases.TextureRegion2D(
                texture,
                0,
                0,
                texture.Width,
                texture.Height)));
            //player.Attach(new Sprite)

            // Input
            _keyboardState = new KeyboardState();
            _previousKeyboardState = new KeyboardState();
            _mouseState = new MouseState();
            _previousMouseState = new MouseState();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteRender = new SpriteRender(_spriteBatch);

            // Mouse cursor
            var cursor = new Texture2D(GraphicsDevice, 16, 16);
            Color[] colors = Enumerable.Range(0, cursor.Width * cursor.Height)
                .Select(i => Color.Red).ToArray();
            cursor.SetData<Color>(colors);
            Mouse.SetCursor(MouseCursor.FromTexture2D(cursor, 0, 0));

            var spriteSheetLoader = new SpriteSheetLoader(Content, GraphicsDevice);
            spriteSheet = spriteSheetLoader.Load("Sprites/Shitsprite_idle");

            // Test
            //var testSheet = Content.Load<Extended.Sprites.SpriteSheet>("Sprites/Shitsprite_idle");
            //var sprite = new AnimatedSprite(testSheet);
            
            _spriteFont = this.Content.Load<SpriteFont>("Fonts/Consolas");
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            _previousKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();
            _previousMouseState = _mouseState;
            _mouseState = Mouse.GetState();
            _worldPosition = _camera.ScreenToWorld(new Vector2(_mouseState.X, _mouseState.Y));

            // Keyboard
            if(_keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            var direction = new Vector2(0.0f, 0.0f);
            direction.X = _keyboardState.IsKeyDown(Keys.Left) ? -1.0f :
                _keyboardState.IsKeyDown(Keys.Right) ? 1.0f : 0.0f;  
            direction.Y = _keyboardState.IsKeyDown(Keys.Up) ? -1.0f :
                _keyboardState.IsKeyDown(Keys.Down) ? 1.0f : 0.0f;  
            direction.Normalize();

            if(!direction.IsNaN())
                _camera.Move(direction * gameTime.ElapsedGameTime.Milliseconds);

            // Mouse
            if(_mouseState.LeftButton == ButtonState.Pressed)
            {
                if(!_player.isCharging)
                {
                    _player.toggleIsCharging();
                }
                else
                {
                    _player.charge += 0.6666f;
                    if(_player.charge > 255.0f)
                        _player.charge = 255.0f;  

                    _player.chargePoint = new Vector2(_mouseState.X, _mouseState.Y);
                }
            }
            else
            {
                if(_player.isCharging)
                    _player.toggleIsCharging();

                if(!_player.isCharging)
                {
                    if(_player.charge > 0.0f)
                        _player.charge -= 0.3333f;
                    else
                        _player.charge = 0.0f;
                }
            }

            _world.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(
                blendState: BlendState.NonPremultiplied,
                samplerState: SamplerState.PointClamp);
                // <HUD>
                _spriteBatch.DrawString(
                    _spriteFont,
                    "TotalGameTime: " + gameTime.TotalGameTime.Seconds.ToString(),
                    new Vector2(0.0f, 0.0f),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "FPS: " + (1 / (float) gameTime.ElapsedGameTime.TotalSeconds).ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Camera: " + _camera.Position.X.ToString("0") + ", " + _camera.Position.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 2),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Cursor: " + _worldPosition.X.ToString("0") + ", " + _worldPosition.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 3),
                    Color.White);

                var direction = _worldPosition - _camera.Center;
                direction.Normalize();
                _spriteBatch.DrawString(
                    _spriteFont,
                    "Direction: " + direction.X.ToString("0.0000") + ", " + direction.Y.ToString("0.0000"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 4),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Angle: " + (direction.ToAngle() * 180 / System.Math.PI + 180).ToString("0.00"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 5),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Charge: " + _player.charge.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 6),
                    Color.White);
                // </HUD>
                if(_player.isCharging)
                {
                    _spriteBatch.DrawLine(
                        _camera.Center.X,
                        _camera.Center.Y,
                        _mouseState.Position.X,
                        _mouseState.Position.Y,
                        Color.Green,
                        _player.charge);
                }
                else if(_player.charge > 0.0f)
                {
                    _spriteBatch.DrawLine(
                        _camera.Center.X,
                        _camera.Center.Y,
                        _player.chargePoint.X,
                        _player.chargePoint.Y,
                        new Color
                        {
                            R = 255,
                            G = 0,
                            B = 0,
                            A = (byte) _player.charge
                        },
                        _player.charge);
                }
            _spriteBatch.End();

            var transformMatrix = _camera.GetViewMatrix();
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                transformMatrix: transformMatrix
            );

                int offset = 0;
                foreach(string key in spriteSheet.Keys())
                {
                    _spriteRender.Draw(
                        spriteSheet.Sprite(key),
                        new Vector2(
                            _graphics.PreferredBackBufferWidth / 2 + offset,
                            _graphics.PreferredBackBufferHeight / 2) - _camera.Position,
                        Color.White,
                        0,
                        spriteSheet.Sprite(key).Scale.X,
                        SpriteEffects.None);
                    offset += (int) (16 * spriteSheet.Sprite(key).Scale.X);    
 
                }

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Hello MonoGame!",
                    new Vector2(
                        _graphics.PreferredBackBufferWidth / 2,
                        _graphics.PreferredBackBufferHeight / 2) - _camera.Position,
                    Color.White);

            _spriteBatch.End();
            _world.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
