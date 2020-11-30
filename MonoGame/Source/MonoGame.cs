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
using MonoGame.Components;

// RetroHerzen ???
namespace MonoGame
{
    public class MonoGame: Game
    {

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

        public MonoGame(Size resolution = default(Size), bool fullscreen = true)
        {
            Core = new Core(this);
            Core.Setup(resolution, fullscreen);
        }

        protected override void LoadContent()
        {
            Assets = new Assets();
            Assets.LoadAllAssets(Core.Content);

            // HUD components
            arrowSprite = new AsepriteSprite("Compass");
            arrowSprite.Position = new Vector2(
                Core.VirtualResolution.Width,
                Core.VirtualResolution.Height) * 0.5f;
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
            //Core.Initialize();
            base.Initialize();
            Core.Initialize();

            player = new Entity[4];

            _world = new WorldBuilder()
                .AddSystem(new RenderSystem())
                .AddSystem(new WeatherSystem())
                .AddSystem(new HUDSystem())
                .AddSystem(new ControllerSystem())
                .AddSystem(new WeaponSystem())
                .AddSystem(new CollisionSystem())
                .AddSystem(new ExpirySystem())
                .AddSystem(new RainfallSystem())
                .Build();
            Components.Add(_world);

            // Entities
            player[0] = _world.CreateEntity();
            player[0].Attach(new Player());
            player[0].Attach(new AsepriteSprite("Shitsprite"));
            player[0].Attach(new Collision());
            player[0].Attach(new WeaponComponent(Weapon.None, 0));

            entity = _world.CreateEntity();
            entity.Attach(new AsepriteSprite("Shitsprite"));
            entity.Attach(new Collision());
            entity.Attach(new WeaponComponent(Weapon.None, 0));
            entity.Get<AsepriteSprite>().Position = new Vector2(
                Core.VirtualResolution.Width / 4,
                Core.VirtualResolution.Height / 4);
            entity.Get<AsepriteSprite>().Color = Color.Red;
            //entity.Get<AsepriteSprite>().Rotation = (float) Math.PI/4;
            entity.Get<AsepriteSprite>().Play("Walk");
        }

        System.Diagnostics.Stopwatch Stoppwatch = new System.Diagnostics.Stopwatch();
        private List<long> drawTicks = new List<long>();
        private List<long> updateTicks = new List<long>();
        protected override void Update(GameTime gameTime)
        {
            Stoppwatch.Reset();
            Stoppwatch.Start();
            Core.Update();

            if(Core.KeyboardState.IsKeyDown(Keys.Up))
                Core.Camera.Zoom += 0.01f;

            if(Core.KeyboardState.IsKeyDown(Keys.Down))
                Core.Camera.Zoom -= 0.01f;

            if(Core.KeyboardState.IsKeyDown(Keys.Left))
                Core.Camera.Rotation -= 0.01f;

            if(Core.KeyboardState.IsKeyDown(Keys.Right))
                Core.Camera.Rotation += 0.01f;
    
            if(Core.KeyboardState.IsKeyDown(Keys.R))
                Core.ToggleRenderQuality();

            if (Core.GamePadState.Buttons.Back == ButtonState.Pressed || Core.KeyboardState.IsKeyDown(Keys.Escape))
            {
                long updateMean = 0;
                foreach(var measure in updateTicks)
                    updateMean += measure;

                long drawMean = 0;
                foreach(var measure in updateTicks)
                    drawMean += measure;

                System.Console.WriteLine("======SHUTING DOWN======");
                System.Console.WriteLine("Update(): {0} [mean ticks of {1} measures]", updateMean / updateTicks.Count, updateTicks.Count);
                System.Console.WriteLine("Draw(): {0} [mean ticks of {1} measures]", drawMean / drawTicks.Count, drawTicks.Count);
                Exit();
            }

            var direction = Core.Camera.ScreenToWorld(
                new Vector2(Core.MouseState.X, Core.MouseState.Y)) - Core.Camera.Center;
            direction.Normalize();
            rotation = direction.ToAngle();
            
            player[0].Get<AsepriteSprite>().Rotation = rotation;
            entity.Get<AsepriteSprite>().Position += Vector2.One * 0.1f;

            base.Update(gameTime);
            //System.Console.WriteLine(entity.Get<AsepriteSprite>().Position + ", " + entity.Get<AsepriteSprite>().Bounds.Position);
 
            Stoppwatch.Stop();
            updateTicks.Add(Stoppwatch.ElapsedTicks);
        }

        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch SpriteBatch = new SpriteBatch(Core.GraphicsDeviceManager.GraphicsDevice);
            Stoppwatch.Reset();
            Stoppwatch.Start();

            if(Core.LowResolution)
                Core.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(Core.MainRenderTarget);

            Core.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);

            if(Core.LowResolution)
            {
                Core.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

                SpriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.PointClamp);
                    //null,
                    //null,
                    //null,
                    //Core.ViewportAdapter.GetScaleMatrix());
                SpriteBatch.Draw(Core.MainRenderTarget, Core.TargetRectangle, Color.White);
                SpriteBatch.End();
            }

            Stoppwatch.Stop();
            drawTicks.Add(Stoppwatch.ElapsedTicks);
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
