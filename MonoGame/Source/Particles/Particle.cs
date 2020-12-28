using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;

namespace MonoGame.Particles
{
    public class Particle : IParticle
    {
        private bool isDisposed;

        private ParticleEffect _particleEffect;

        public Particle(Texture2D texture)
        {
           _particleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = new Vector2(400f, 200),
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter("Name", new TextureRegion2D(texture), 400, TimeSpan.FromSeconds(2.5),
                        Profile.Spray(-Vector2.One, 100f))//.BoxUniform(100,250))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 3,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(1.0f, 10.0f)
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ColorInterpolator
                                    {
                                        StartValue = new HslColor(0.33f, 0.5f, 0.5f),
                                        EndValue = new HslColor(0.5f, 0.9f, 1.0f)
                                    }
                                }
                            },
                            new RotationModifier {RotationRate = -2.1f},
                            new RectangleContainerModifier {Width = 800, Height = 480},
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f},
                        }
                    }
                }
            };
        }

        public void Update(GameTime gameTime)
        {
            _particleEffect.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_particleEffect);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
                _particleEffect.Dispose();

            isDisposed = true;
        }

        ~Particle()
        {
            this.Dispose(false);
        }
    }
}