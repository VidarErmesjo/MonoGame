using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteManager;
using TexturePackerMonoGameDefinitions;

namespace MonoGame
{
    public class MonoGame: Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private SpriteRender _spriteRender;

        private readonly Vector2 screenResolution4K;

        // Eiendel av entitet?
        public SpriteSheet spriteSheet;

        public MonoGame(bool fullscreen)
        {
            _graphics = new GraphicsDeviceManager(this);

             screenResolution4K = new Vector2(3840, 2160);
            
            if(fullscreen)
            {
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                _graphics.IsFullScreen = true;

            }
            else
            {
                _graphics.PreferredBackBufferWidth = (int) screenResolution4K.X / 2;
                _graphics.PreferredBackBufferHeight = (int) screenResolution4K.Y / 2;
            }

            IsFixedTimeStep = true;
            _graphics.SynchronizeWithVerticalRetrace = true;

            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteRender = new SpriteRender(_spriteBatch);

            var spriteSheetLoader = new SpriteSheetLoader(Content, GraphicsDevice);
            spriteSheet = spriteSheetLoader.Load("Sprites/Shitsprite_idle");
            System.Console.WriteLine(spriteSheet);
            spriteSheet.SaveAsPngs();

            var color = new Color[256];
            spriteSheet.Sprite("shitguy/walk/0004").Texture.GetData<Color>(color);

            _spriteFont = this.Content.Load<SpriteFont>("Fonts/Consolas");
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            if(Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        float rotation = 0.0f;
        float scale = 1.0f;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                null);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "TotalGameTime: " + gameTime.TotalGameTime.Seconds.ToString(),
                    new Vector2(0, 0),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "FPS: " + (1 / (float) gameTime.ElapsedGameTime.TotalSeconds).ToString(),
                    new Vector2(0, _spriteFont.LineSpacing),
                    Color.White);

                int offset = 0;
                foreach(string key in spriteSheet.Keys())
                {
                    rotation = rotation > System.Math.PI * 2 ? rotation = 0.0f : rotation += 0.0025f;
                    scale = scale > 8.0f ? scale = 0.0f : scale += 0.0025f;

                    _spriteRender.Draw(
                        spriteSheet.Sprite(key),
                        new Vector2(
                            _graphics.PreferredBackBufferWidth / 2 + offset,
                            _graphics.PreferredBackBufferHeight / 2),
                        Color.White,
                        rotation,
                        spriteSheet.Sprite(key).Scale.X * scale,
                        SpriteEffects.None);
                    offset += (int) (16 * spriteSheet.Sprite(key).Scale.X * scale);    
 
                }

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Hello MonoGame!",
                    new Vector2(
                        _graphics.PreferredBackBufferWidth / 2,
                        _graphics.PreferredBackBufferHeight / 2),
                    Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
