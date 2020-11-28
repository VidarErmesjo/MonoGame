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
        private readonly Size _virtualResolution;
        private readonly Size _deviceResolution;
        private readonly bool _fullscreen;

        // Weather
        public static float WindSpeed = -0.1f;

        // Animation test
        public static AsepriteSprite arrowSprite;
        public static AsepriteSprite aseprite;

        private World _world { get; set; }

        // Make Array of players, npcs, etc.
        public static Entity[] player { get; set; }
        public static Entity entity { get; set;}
        //private List<Entity> _entities { get; set; }

        //private float _scale = 1.0f;
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

        public MonoGame(Size virtualResolution = default(Size), Size deviceResolution = default(Size), bool fullscreen = true)
        {
            Globals.Setup(this);
            
            _virtualResolution = virtualResolution;
            _deviceResolution = deviceResolution;
            _fullscreen = fullscreen;
            
            player = new Entity[4];
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _world.Dispose();
                Globals.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            Assets.LoadAllAssets(Content);

            // HUD components
            arrowSprite = new AsepriteSprite("Compass");
            arrowSprite.Position = new Vector2(
                _virtualResolution.Width,
                _virtualResolution.Height) * 0.5f;
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
            Globals.Initialize(_virtualResolution, _deviceResolution, _fullscreen);
            base.Initialize();

            _world = new WorldBuilder()
                .AddSystem(new RenderSystem())
                .AddSystem(new WeatherSystem())
                .AddSystem(new HUDSystem())
                .AddSystem(new ControllerSystem())
                .AddSystem(new WeaponSystem())
                .AddSystem(new ExpirySystem())
                .AddSystem(new RainfallSystem())
                .Build();
            Components.Add(_world);

            // Entities
            player[(int) Player.One] = _world.CreateEntity();
            player[(int) Player.One].Attach(new AsepriteSprite("Shitsprite"));
            player[(int) Player.One].Attach(new WeaponComponent(Weapon.None, 0));
        }

        protected override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);
            
            Globals.Input.Update();

            KeyboardState keyboardState = Globals.Input.KeyboardState;
            GamePadState gamePadState = Globals.Input.GamePadState;
            MouseState mouseState = Globals.Input.MouseState;

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            var direction = Globals.Viewport.Camera.ScreenToWorld(
                new Vector2(
                    mouseState.X,
                    mouseState.Y)) - Globals.Viewport.Camera.Center;
            direction.Normalize();
            rotation = direction.ToAngle();

            //_world.Update(gameTime);
            base.Update(gameTime);
        }

        /*private Rectangle FitToScreen()
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
        }*/

        protected override void Draw(GameTime gameTime)
        {
            //_graphics.GraphicsDevice.Clear(Color.DarkSlateGray);

            //_graphics.GraphicsDevice.SetRenderTarget(_primaryRenderTarget);
            //_graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            Globals.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);
            //_world.Draw(gameTime);
            //_graphics.GraphicsDevice.SetRenderTarget(null);

            /*_spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                blendState: BlendState.Opaque,
                samplerState: SamplerState.PointClamp);

               _spriteBatch.Draw(_primaryRenderTarget, FitToScreen(), Color.White);
;
            _spriteBatch.End(); */
            base.Draw(gameTime);    
        }
    }
}
