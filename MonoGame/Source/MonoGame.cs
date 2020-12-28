using System;
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

using MonoGame.Assets;
using MonoGame.Components;
using MonoGame.Collisions;
using MonoGame.Particles;
using MonoGame.Effects;
using MonoGame.Entities;
using MonoGame.Weapons;

// RetroHerzen ???
namespace MonoGame
{
    public class MonoGame: Game
    {
        private bool isDisposed = false;

        private Effect _pixelatorShader;
        private SpriteBatch _spriteBatch;

        public readonly AssetManager AssetManager;
        public readonly CollisionManager CollisionManager;
        public readonly EffectManager EffectManager;
        public readonly EntityManager EntityManager;
        public readonly FastRandom FastRandom;
        public readonly GameManager GameManager;
        public readonly HUD HUD;
        public readonly ParticleManager ParticleManager;
        public readonly WeaponManager WeaponManager;

        public MonoGame(Size resolution = default(Size), bool fullscreen = true)
        {
            AssetManager = new AssetManager(Content);
            CollisionManager = new CollisionManager(this);
            EffectManager = new EffectManager(this);
            EntityManager = new EntityManager();
            FastRandom = new FastRandom(Environment.TickCount);
            GameManager = new GameManager(this, resolution, fullscreen);
            HUD = new HUD(this);
            ParticleManager = new ParticleManager(this);
            WeaponManager = new WeaponManager(this);
        }

        protected override void LoadContent()
        {
            AssetManager.LoadContent();
            EffectManager.LoadContent();
            HUD.LoadContent();

            AsepriteDocument asepriteDocument = AssetManager.Sprite("Shitsprite");//Content.Load<AsepriteDocument>("Aseprite/Shitsprite");

            for(int i = 0; i < 99; i++)
            {
                var entity = EntityManager.AddEntity(new Actor(asepriteDocument));

                entity.Position = new Vector2(GameManager.VirtualResolution.Width * FastRandom.NextSingle(0f, 1f), GameManager.VirtualResolution.Height * FastRandom.NextSingle(0f, 1f));
                entity.Scale = Vector2.One * FastRandom.NextSingle(1f, 4f);
                entity.Velocity = new Vector2(FastRandom.NextSingle(-1f, 1f), FastRandom.NextSingle(-1f, 1f)) * FastRandom.NextSingle(50f, 100f);
            }

            var player = EntityManager.AddEntity(new Player(asepriteDocument));
            player.Position = new Vector2(
                GameManager.VirtualResolution.Width * 0.5f,
                GameManager.VirtualResolution.Height * 0.5f);
            player.Scale = Vector2.One * 8f;

            _pixelatorShader = Content.Load<Effect>("Effects/Sepia");

            // Different types
            //_particleManager = new ParticleManager(GameManager.GraphicsDeviceManager.GraphicsDevice, _effectManager);
            //var particle = _particleManager.AddParticle(new Particle(_assetManager.Texture("Pixel")));
            ParticleManager.AddParticle(new Particle(AssetManager.Texture("Pixel")));

            Console.WriteLine("[Entities] Players:{0}, Actors:{1}: Total:{2}", EntityManager.Players.Count(), EntityManager.Actors.Count(), EntityManager.Entities.Count());
            base.LoadContent();
        }

        protected override void Initialize()
        {
            base.Initialize();
            GameManager.Initialize();
            CollisionManager.Initialize();
            EffectManager.Initialize();
            HUD.Initialize();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        Stopwatch Stoppwatch = new Stopwatch();
        private List<long> drawTicks = new List<long>();
        private List<long> updateTicks = new List<long>();
        protected override void Update(GameTime gameTime)
        {
            Stoppwatch.Reset();
            Stoppwatch.Start();
            GameManager.Update();

            if(GameManager.KeyboardState.IsKeyDown(Keys.W))
                GameManager.Camera.Zoom += 0.01f;

            if(GameManager.KeyboardState.IsKeyDown(Keys.S))
                GameManager.Camera.Zoom -= 0.01f;

            if(GameManager.KeyboardState.IsKeyDown(Keys.A))
                GameManager.Camera.Rotation -= 0.01f;

            if(GameManager.KeyboardState.IsKeyDown(Keys.D))
                GameManager.Camera.Rotation += 0.01f;
    
            if(GameManager.KeyboardState.IsKeyDown(Keys.R))
                GameManager.ToggleRenderQuality();

            if(GameManager.GamePadState.Buttons.Back == ButtonState.Pressed || GameManager.KeyboardState.IsKeyDown(Keys.Escape))
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

            EntityManager.Update(gameTime);
            ParticleManager.Update(gameTime);
            CollisionManager.Update(gameTime);
            HUD.Update(gameTime);
            base.Update(gameTime);

            GameManager.Camera.LookAt(EntityManager.Players.First().Position);

            Stoppwatch.Stop();
            updateTicks.Add(Stoppwatch.ElapsedTicks);
        }

        protected override void Draw(GameTime gameTime)
        {
            Stoppwatch.Reset();
            Stoppwatch.Start();

            GameManager.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget((GameManager.LowResolution ? GameManager.VirtualRenderTarget : GameManager.DeviceRenderTarget));
            GameManager.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Transparent);

            EntityManager.Draw(
                _spriteBatch,
                transformMatrix: GameManager.Camera.GetViewMatrix());

            ParticleManager.Draw(
                _spriteBatch, transformMatrix:
                GameManager.Camera.GetViewMatrix());

            this.Draw(_spriteBatch, null);
            HUD.Draw(_spriteBatch);
            base.Draw(gameTime);

            Stoppwatch.Stop();
            drawTicks.Add(Stoppwatch.ElapsedTicks);
        }

        /// <summary>
        ///     Draws to device
        /// </summary>
        /// <remarks>   
        ///     Includes effects
        /// </remarks>
        private void Draw(SpriteBatch spriteBatch, Effect effect = null)
        {
           GameManager.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                effect: effect);

            spriteBatch.Draw(
                (GameManager.LowResolution ? GameManager.VirtualRenderTarget : GameManager.DeviceRenderTarget),
                GameManager.DeviceRectangle,
                Color.White);

            spriteBatch.End();
        }

        protected override void UnloadContent()
        {
            GameManager.UnloadContent();
            AssetManager.UnloadContent();
            CollisionManager.UnloadContent();
            EffectManager.UnloadContent();
            EntityManager.UnloadContent();
            HUD.UnloadContent();
            ParticleManager.UnloadContent();
            base.UnloadContent();
            this.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
            {
                _pixelatorShader.Dispose();
                _spriteBatch.Dispose();
            }

            isDisposed = true;
        }

        ~MonoGame()
        {
            Dispose(false);
        }
    }
}
