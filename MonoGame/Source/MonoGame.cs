﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Aseprite.Graphics;
using MonoGame.Aseprite.Documents;

using MonoGame.Entities;

// RetroHerzen ???
namespace MonoGame
{
    public class MonoGame: Game
    {
        private bool isDisposed = false;


        //private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly EntityManager _entityManager;

        private HUD _hud;
        private Entity[] _actors;
        private Player _player;

        private CollisionComponent _collisionComponent;
        private CollisionWorld _collisionWorld;

        private SpriteBatch _spriteBatch;

        public static Core Core { get; private set; }   // => GameManager?
        public static Assets Assets { get; private set; }   // => AssetsManager?

        // Experimental
        public enum Weapon {
            None = 0,
            Something
        };

        public MonoGame(Size resolution = default(Size), bool fullscreen = true)
        {
            Core = new Core(this);
            Core.Setup(resolution, fullscreen);

            //_graphicsDeviceManager = new GraphicsDeviceManager(this);
            _entityManager = new EntityManager();
        }

        protected override void LoadContent()
        {
            Assets = new Assets();
            Assets.LoadAllAssets(Core.Content);

            AsepriteDocument asepriteDocument = Assets.Sprite("Shitsprite");//Content.Load<AsepriteDocument>("Aseprite/Shitsprite");

            _actors = new Entity[199];

            var fastRandom = new FastRandom(Environment.TickCount);

            for(int i = 0; i < _actors.Length; i++)
            {
                _actors[i] = _entityManager.AddEntity(new Actor(asepriteDocument));
                _actors[i].Position = new Vector2(Core.VirtualResolution.Width * fastRandom.NextSingle(0f, 1f), Core.VirtualResolution.Height * fastRandom.NextSingle(0f, 1f));
                _actors[i].Scale = Vector2.One * fastRandom.NextSingle(1f, 4f);
                _actors[i].Velocity = new Vector2(fastRandom.NextSingle(-1f, 1f), fastRandom.NextSingle(-1f, 1f)) * fastRandom.NextSingle(50f, 100f);
            }

            _player = _entityManager.AddEntity(new Player(asepriteDocument));
            _player.Position = new Vector2(
                Core.VirtualResolution.Width * 0.5f,
                Core.VirtualResolution.Height * 0.5f);
            _player.Scale = Vector2.One * 8f;

            base.LoadContent();
        }

        protected override void Initialize()
        {
            base.Initialize();
            Core.Initialize();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _hud = new HUD(Assets.Font("Consolas"));
            _hud.Mount(_player);
            _hud.Mount(_entityManager);
            _hud.Mount(Core.Camera);
            _hud.Mount(Core.ViewportAdapter);

            /*_collisionComponent = new CollisionComponent(
                new RectangleF(
                    Core.Camera.Position.X,
                    Core.Camera.Position.Y,
                    Core.VirtualResolution.Width,
                    Core.VirtualResolution.Height));         
            foreach(var entity in _entityManager.Entities.Where(e => e.IsCollidable))
                _collisionComponent.Insert(entity);*/

            _collisionWorld = new CollisionWorld(new Vector2(0f, 0f));
            _collisionWorld.CreateActor(_player);
            foreach(var actor in _actors.Where(entity => entity.IsCollidable))
                _collisionWorld.CreateActor(actor);
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

            if(Core.GamePadState.Buttons.Back == ButtonState.Pressed || Core.KeyboardState.IsKeyDown(Keys.Escape))
            {
                long updateMin = long.MaxValue;
                long updateMax = long.MinValue;
                long updateAvg = 0;
                foreach(var measure in updateTicks)
                {
                    if(measure <= updateMin)
                        updateMin = measure;

                    if(measure >= updateMax)
                        updateMax = measure;

                    updateAvg += measure;
                }
                updateAvg /= updateTicks.Count;

                long drawMin = long.MaxValue;
                long drawMax = long.MinValue;
                long drawAvg = 0;
                foreach(var measure in drawTicks)
                {
                    if(measure <= drawMin)
                        drawMin = measure;

                    if(measure >= drawMax)
                        drawMax = measure;

                    drawAvg += measure;
                }
                drawAvg /= drawTicks.Count;

                System.Console.WriteLine("======SHUTING DOWN======");
                System.Console.WriteLine("Update(): min:{0}, max:{1}, avg:{2} [of {3} measures]", updateMin, updateMax, updateAvg, updateTicks.Count);
                System.Console.WriteLine("Draw(): min:{0}, max:{1}, avg:{2} [of {3} measures]", drawMin, drawMax, drawAvg, drawTicks.Count);
                Exit();
            }

            // Manager? Dynamic boundary or move to Initialize?
            _collisionComponent = new CollisionComponent(new RectangleF(
                Core.Camera.Position.X,
                Core.Camera.Position.Y,
                Core.VirtualResolution.Width,
                Core.VirtualResolution.Height));
           
            foreach(var entity in _entityManager.Entities.Where(e => e.IsCollidable))
                _collisionComponent.Insert(entity);
            ///

            _entityManager.Update(gameTime);
            _collisionComponent.Update(gameTime);
            _collisionWorld.Update(gameTime);
            _hud.Update(gameTime);
            base.Update(gameTime);

            Core.Camera.LookAt(_player.Position);

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

            Core.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Transparent);

            _entityManager.Draw(_spriteBatch);
            _hud.Draw(_spriteBatch);
            base.Draw(gameTime);

            if(Core.LowResolution)
            {
                Core.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

                SpriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.PointClamp);
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
                //_graphicsDeviceManager.Dispose();
                //_actor.Dispose();
                foreach(var actor in _actors)
                    actor.Dispose();
                _player.Dispose();
                _collisionComponent.Dispose();
                _collisionWorld.Dispose();
                _spriteBatch.Dispose();
                Assets.Dispose();
                Core.Dispose();
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
