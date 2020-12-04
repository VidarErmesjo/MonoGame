using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Aseprite;
using MonoGame.Components;

// RetroHerzen ???
namespace MonoGame
{
    public class MonoGame: Game
    {
        private bool isDisposed = false;

        // Make Array of players, npcs, etc.
        public static Entity[] player { get; set; }
        public static Entity entity { get; set;}
        //private List<Entity> _entities { get; set; }

        //private float _scale = 1.0f;
        public static float rotation { get; private set; }

        public static Core Core { get; private set; }
        public static Assets Assets { get; private set; }
        public static World World { get; private set; }

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
        }

        protected override void Initialize()
        {
            base.Initialize();
            Core.Initialize();

            player = new Entity[4];

            World = new WorldBuilder()
                .AddSystem(new PlayerSystem())
                .AddSystem(new ActorSystem())
                .AddSystem(new ControllerSystem())
                .AddSystem(new WeaponSystem())
                .AddSystem(new RenderSystem())
                .AddSystem(new WeatherSystem())
                .AddSystem(new HUDSystem())
                .AddSystem(new CollisionSystem())
                .AddSystem(new ExpirySystem())
                .AddSystem(new RainfallSystem())
                .Build();
            Components.Add(World);
            World.Initialize();

            // Entities
            player[0] = World.CreateEntity();
            player[0].Attach(new PlayerComponent(player[0].Id, Core.Camera.Center));
            player[0].Attach(new AsepriteSprite("Shitsprite"));
            player[0].Attach(new Collision());
            player[0].Attach(new WeaponComponent(Weapon.None, 0));

            entity = World.CreateEntity();
            entity.Attach(new ActorComponent(
                entity.Id,
                new Vector2(
                    Core.VirtualResolution.Width * 0.25f,
                    Core.VirtualResolution.Height * 0.25f),
                Vector2.Zero));
            entity.Attach(new AsepriteSprite("Shitsprite"));
            entity.Attach(new Collision());
            entity.Attach(new WeaponComponent(Weapon.None, 0));
        }

        System.Diagnostics.Stopwatch Stoppwatch = new System.Diagnostics.Stopwatch();
        private List<long> drawTicks = new List<long>();
        private List<long> updateTicks = new List<long>();
        protected override void Update(GameTime gameTime)
        {
            Stoppwatch.Reset();
            Stoppwatch.Start();
            Core.Update();

            if(Core.KeyboardState.IsKeyDown(Keys.W))
                Core.Camera.Zoom += 0.01f;

            if(Core.KeyboardState.IsKeyDown(Keys.S))
                Core.Camera.Zoom -= 0.01f;

            if(Core.KeyboardState.IsKeyDown(Keys.A))
                Core.Camera.Rotation -= 0.01f;

            if(Core.KeyboardState.IsKeyDown(Keys.D))
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

            //World.Update(gameTime);
            base.Update(gameTime);

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
            //World.Draw(gameTime);
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
            if(isDisposed)
                return;

            if(disposing)
            {
                Assets.Dispose();
                Core.Dispose();
                World.Dispose();

                foreach(Entity entity in player)
                    if(entity != null)
                        entity.Destroy();

                 entity.Destroy();
            }

            base.Dispose(disposing);

            isDisposed = true;
        }

        ~MonoGame()
        {
            Dispose(false);
        }
    }
}
