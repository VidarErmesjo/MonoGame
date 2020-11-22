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
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _spriteFont;

        public HUDSystem(GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            MonoGame.spriteFonts.TryGetValue("Consolas", out _spriteFont);
        }

        public override void Draw(GameTime gameTime)
        {
            WeaponComponent weaponComponent;
            Vector2 screenToWorld;

            weaponComponent = MonoGame.player[(int) MonoGame.Player.One].Get<WeaponComponent>();
            screenToWorld = MonoGame.camera.ScreenToWorld(new Vector2(MonoGame.mouseState.X, MonoGame.mouseState.Y));

            var direction = screenToWorld - MonoGame.camera.Center;
            direction.Normalize();

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "TotalGameTime: " + gameTime.TotalGameTime.ToString(),
                    new Vector2(0.0f, 0.0f),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "FPS: " + (1 / (float) gameTime.ElapsedGameTime.TotalSeconds).ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Camera: " + MonoGame.camera.Position.X.ToString("0") + ", " + MonoGame.camera.Position.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 2),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Cursor: " + screenToWorld.X.ToString("0") + ", " + screenToWorld.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 3),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Direction: " + direction.X.ToString("0.0000") + ", " + direction.Y.ToString("0.0000"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 4),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Angle: " + (180 + direction.ToAngle() * 180 / System.Math.PI).ToString("0.00"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 5),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Charge: " + weaponComponent.charge.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 6),
                    Color.White);

                MonoGame.arrowSprite.RenderDefinition.Rotation = direction.ToAngle();
                MonoGame.arrowSprite.Render(_spriteBatch);
            _spriteBatch.End();
        }
    }
}