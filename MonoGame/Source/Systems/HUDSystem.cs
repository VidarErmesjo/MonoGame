using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.Entities.Systems
{
    public class HUDSystem: DrawSystem
    {
        private GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        public HUDSystem(GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _graphicsDevice = graphicsDevice;
        }

        public override void Draw(GameTime gameTime)
        {
            WeaponComponent testComponent;
            Vector2 worldPosition;

            testComponent = MonoGame.player.Get<WeaponComponent>();
            worldPosition = MonoGame.camera.ScreenToWorld(new Vector2(MonoGame.mouseState.X, MonoGame.mouseState.Y));

            var direction = worldPosition - MonoGame.camera.Center;
            direction.Normalize();

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp);

                _spriteBatch.DrawString(
                    MonoGame.spriteFontConsolas,
                    "TotalGameTime: " + gameTime.TotalGameTime.ToString(),
                    new Vector2(0.0f, 0.0f),
                    Color.White);

                _spriteBatch.DrawString(
                    MonoGame.spriteFontConsolas,
                    "FPS: " + (1 / (float) gameTime.ElapsedGameTime.TotalSeconds).ToString("0"),
                    new Vector2(0.0f, MonoGame.spriteFontConsolas.LineSpacing),
                    Color.White);

                _spriteBatch.DrawString(
                    MonoGame.spriteFontConsolas,
                    "Camera: " + MonoGame.camera.Position.X.ToString("0") + ", " + MonoGame.camera.Position.Y.ToString("0"),
                    new Vector2(0.0f, MonoGame.spriteFontConsolas.LineSpacing * 2),
                    Color.White);

                _spriteBatch.DrawString(
                    MonoGame.spriteFontConsolas,
                    "Cursor: " + worldPosition.X.ToString("0") + ", " + worldPosition.Y.ToString("0"),
                    new Vector2(0.0f, MonoGame.spriteFontConsolas.LineSpacing * 3),
                    Color.White);

                _spriteBatch.DrawString(
                    MonoGame.spriteFontConsolas,
                    "Direction: " + direction.X.ToString("0.0000") + ", " + direction.Y.ToString("0.0000"),
                    new Vector2(0.0f, MonoGame.spriteFontConsolas.LineSpacing * 4),
                    Color.White);

                _spriteBatch.DrawString(
                    MonoGame.spriteFontConsolas,
                    "Angle: " + (direction.ToAngle() * 180 / System.Math.PI + 180).ToString("0.00"),
                    new Vector2(0.0f, MonoGame.spriteFontConsolas.LineSpacing * 5),
                    Color.White);

                _spriteBatch.DrawString(
                    MonoGame.spriteFontConsolas,
                    "Charge: " + testComponent.charge.ToString("0"),
                    new Vector2(0.0f, MonoGame.spriteFontConsolas.LineSpacing * 6),
                    Color.White);

                var origin = MonoGame.camera.Center - MonoGame.camera.Position;
                var scale = Vector2.One * 0.025f;
                Transform2 transform = new Transform2(
                    origin.X,
                    origin.Y,
                    (float) (direction.ToAngle() + System.Math.PI),
                    scale.X,
                    scale.Y);                
                _spriteBatch.Draw(MonoGame.arrowSprite, transform);

            _spriteBatch.End();
        }
    }
}