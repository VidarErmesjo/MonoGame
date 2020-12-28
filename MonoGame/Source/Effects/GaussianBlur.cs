using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Effects
{
    /// <summary>
    /// A Gaussian blur filter this.Kernel class. A Gaussian blur filter this.Kernel is
    /// perfectly symmetrical and linearly separable. This means we can split
    /// the full 2D filter this.Kernel matrix into two smaller horizontal and
    /// vertical 1D filter this.Kernel matrices and then perform the Gaussian blur
    /// in two passes. Contrary to what you might think performing the Gaussian
    /// blur in this way is actually faster than performing the Gaussian blur
    /// in a single pass using the full 2D filter this.Kernel matrix.
    /// <para>
    /// The GaussianBlur class is intended to be used in conjunction with an
    /// HLSL Gaussian blur shader. The following code snippet shows a typical
    /// Effect file implementation of a Gaussian blur.
    /// The RADIUS constant in the _effect file must match the this.Radius value in
    /// the GaussianBlur class. The _effect file's weights global variable
    /// corresponds to the GaussianBlur class' this.Kernel field. The _effect file's
    /// offsets global variable corresponds to the GaussianBlur class'
    /// this.TextureOffsetsX and this.TextureOffsetsY fields.
    /// </para>
    /// Copyright (c) 2008-2011 dhpoware. All Rights Reserved.
    /// </summary>

    public class GaussianBlur : IDisposable
    {
        private bool isDisposed;

        private const int BLUR_RADIUS = 7;
        private const float BLUR_AMOUNT = 2f;

        private readonly MonoGame _game;
        private readonly Effect _effect;

        private RenderTarget2D _renderTarget1;
        private RenderTarget2D _renderTarget2;

        /// <summary>
        /// Returns the this.Radius of the Gaussian blur filter this.Kernel in pixels.
        /// </summary>
        public int Radius { get; private set; }

        /// <summary>
        /// Returns the blur amount. This value is used to calculate the
        /// Gaussian blur filter this.Kernel's this.Sigma value. Good values for this
        /// property are 2 and 3. 2 will give a more blurred result whilst 3
        /// will give a less blurred result with sharper details.
        /// </summary>
        public float Amount { get; private set; }

        /// <summary>
        /// Returns the Gaussian blur filter's standard deviation.
        /// </summary>
        public float Sigma { get; private set; }

        /// <summary>
        /// Returns the Gaussian blur filter this.Kernel matrix. Note that the
        /// this.Kernel returned is for a 1D Gaussian blur filter this.Kernel matrix
        /// intended to be used in a two pass Gaussian blur operation.
        /// </summary>
        public float[] Kernel { get; private set; }

        /// <summary>
        /// Returns the texture offsets used for the horizontal Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsX { get; private set; }

        /// <summary>
        /// Returns the texture offsets used for the vertical Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsY { get; private set; }
        
        /// <summary>
        /// This overloaded constructor instructs the GaussianBlur class to
        /// load and use its GaussianBlur.fx _effect file that implements the
        /// two pass Gaussian blur operation on the GPU. The _effect file must
        /// be already bound to the asset name: 'Effects\GaussianBlur' or
        /// 'GaussianBlur'.
        /// </summary>
        public GaussianBlur(MonoGame game)
        {
            _game = game;
            _effect = _game.Content.Load<Effect>("Effects/GaussianBlur");
            this.ComputeKernel(BLUR_RADIUS, BLUR_AMOUNT);
            this.ComputeOffsets(_game.GameManager.VirtualResolution.Width, _game.GameManager.VirtualResolution.Height);
        }

        public void Initialize()
        {
            
        }

        /// <summary>
        /// Calculates the Gaussian blur filter this.Kernel. This implementation is
        /// ported from the original Java code appearing in chapter 16 of
        /// "Filthy Rich Clients: Developing Animated and Graphical Effects for
        /// Desktop Java".
        /// </summary>
        /// <param name="blurRadius">The blur this.Radius in pixels.</param>
        /// <param name="blurAmount">Used to calculate this.Sigma.</param>
        public void ComputeKernel(int blurRadius, float blurAmount)
        {
            this.Radius = blurRadius;
            this.Amount = blurAmount;

            this.Kernel = new float[this.Radius * 2 + 1];
            this.Sigma = this.Radius / this.Amount;

            float twoSigmaSquare = 2f * this.Sigma * this.Sigma;
            float sigmaRoot = (float) MathF.Sqrt(twoSigmaSquare * MathF.PI);
            float total = 0f;
            float distance = 0f;
            int index = 0;

            for(int i = -this.Radius; i <= this.Radius; ++i)
            {
                distance = i * i;
                index = i + this.Radius;
                this.Kernel[index] = (float) MathF.Exp(-distance / twoSigmaSquare) / sigmaRoot;
                total += this.Kernel[index];
            }

            for(int i = 0; i < this.Kernel.Length; ++i)
                this.Kernel[i] /= total;
        }

        /// <summary>
        /// Calculates the texture coordinate offsets corresponding to the
        /// calculated Gaussian blur filter this.Kernel. Each of these offset values
        /// are added to the current pixel's texture coordinates in order to
        /// obtain the neighboring texture coordinates that are affected by the
        /// Gaussian blur filter this.Kernel. This implementation has been adapted
        /// from chapter 17 of "Filthy Rich Clients: Developing Animated and
        /// Graphical Effects for Desktop Java".
        /// </summary>
        /// <param name="textureWidth">The texture width in pixels.</param>
        /// <param name="textureHeight">The texture height in pixels.</param>
        public void ComputeOffsets(float textureWidth, float textureHeight)
        {
            this.TextureOffsetsX = new Vector2[this.Radius * 2 + 1];
            this.TextureOffsetsY = new Vector2[this.Radius * 2 + 1];

            int index = 0;
            float xOffset = 1f / textureWidth;
            float yOffset = 1f / textureHeight;

            for (int i = -this.Radius; i <= this.Radius; ++i)
            {
                index = i + this.Radius;
                this.TextureOffsetsX[index] = new Vector2(i * xOffset, 0.0f);
                this.TextureOffsetsY[index] = new Vector2(0.0f, i * yOffset);
            }
        }

        /// <summary>
        /// Performs the Gaussian blur operation on the source texture image.
        /// The Gaussian blur is performed in two passes: a horizontal blur
        /// pass followed by a vertical blur pass. The output from the first
        /// pass is rendered to renderTarget1. The output from the second pass
        /// is rendered to renderTarget2. The dimensions of the blurred texture
        /// is therefore equal to the dimensions of renderTarget2.
        /// </summary>
        /// <param name="srcTexture">The source image to blur.</param>
        /// <param name="renderTarget1">Stores the output from the horizontal blur pass.</param>
        /// <param name="renderTarget2">Stores the output from the vertical blur pass.</param>
        /// <param name="spriteBatch">Used to draw quads for the blur passes.</param>
        /// <returns>The resulting Gaussian blurred image.</returns>
        public Texture2D PerformGaussianBlur(Texture2D texture, SpriteBatch spriteBatch)
        {
            if(_effect == null)
                throw new InvalidOperationException("GaussianBlur.fx effect not loaded.");

            var renderTargetWidth = texture.Width / 2;
            var renderTargetHeight = texture.Height / 2;

            _renderTarget1 = new RenderTarget2D(
                _game.GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                _game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            _renderTarget2 = new RenderTarget2D(
                _game.GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                _game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            Texture2D outputTexture = null;
            Rectangle srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle destRect1 = new Rectangle(0, 0, _renderTarget1.Width, _renderTarget1.Height);
            Rectangle destRect2 = new Rectangle(0, 0, _renderTarget2.Width, _renderTarget2.Height);
                        
            // Perform horizontal Gaussian blur.

            _game.GraphicsDevice.SetRenderTarget(_renderTarget1);

            _effect.CurrentTechnique = _effect.Techniques["GaussianBlur"];
            _effect.Parameters["weights"].SetValue(this.Kernel);
            _effect.Parameters["screenTexture"].SetValue(texture);
            _effect.Parameters["offsets"].SetValue(this.TextureOffsetsX);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _effect);
            spriteBatch.Draw(texture, destRect1, Color.White);
            spriteBatch.End();
                        
            // Perform vertical Gaussian blur.

            _game.GraphicsDevice.SetRenderTarget(_renderTarget2);
            outputTexture = (Texture2D) _renderTarget1;

            _effect.Parameters["screenTexture"].SetValue(outputTexture);
            _effect.Parameters["offsets"].SetValue(this.TextureOffsetsY);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _effect);
            spriteBatch.Draw(outputTexture, destRect2, Color.White);
            spriteBatch.End();

            // Return the Gaussian blurred texture.

            _game.GraphicsDevice.SetRenderTarget(null);

            return (Texture2D) _renderTarget2;;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if(isDisposed)
                return;

            if(disposing)
            {
                _effect.Dispose();
                _game.Dispose();
                if(_renderTarget1 != null)
                    _renderTarget1.Dispose();

                if(_renderTarget2 != null)
                    _renderTarget2.Dispose();
            }

            isDisposed = true;
        }

        ~GaussianBlur()
        {
            this.Dispose(false);
        }
    }
}