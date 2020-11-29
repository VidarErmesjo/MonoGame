using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Aseprite;

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

        public static Core Core { get; private set; }
        public static Assets Assets { get; private set; }

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
            Core = new Core(this);
            _virtualResolution = virtualResolution;
            _deviceResolution = deviceResolution;
            _fullscreen = fullscreen;
        }

        protected override void LoadContent()
        {
            Assets = new Assets();
            Assets.LoadAllAssets(Core.Content);

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
        }

        protected override void Initialize()
        {
            Core.Initialize(_virtualResolution, _deviceResolution, _fullscreen);
            base.Initialize();

            player = new Entity[4];

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
            Core.Update();
            KeyboardState keyboardState = Core.KeyboardState;
            GamePadState gamePadState = Core.GamePadState;
            MouseState mouseState = Core.MouseState;

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            var direction = Core.Camera.ScreenToWorld(
                new Vector2(
                    mouseState.X,
                    mouseState.Y)) - Core.Camera.Center;
            direction.Normalize();
            rotation = direction.ToAngle();

            //_world.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //_graphics.GraphicsDevice.Clear(Color.DarkSlateGray);

            //_graphics.GraphicsDevice.SetRenderTarget(_primaryRenderTarget);
            //_graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            Core.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);
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

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _world.Dispose();
                aseprite.Dispose();
                arrowSprite.Dispose();

                Assets.Dispose();
                Core.Dispose();
            }

            base.Dispose(disposing);
        }

        ~MonoGame()
        {
            Dispose(false);
        }
    }
}
