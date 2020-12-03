using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Aseprite;

namespace MonoGame.Extended.Entities.Systems
{
    public class HUDSystem: DrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _spriteFont;

        public HUDSystem()
        {
            _spriteBatch = new SpriteBatch(Core.GraphicsDeviceManager.GraphicsDevice);
            _spriteFont = Assets.Font("Consolas");
        }

        public override void Draw(GameTime gameTime)
        {
            WeaponComponent weaponComponent;
            weaponComponent = MonoGame.player[0].Get<WeaponComponent>();

            var direction = new Vector2(
                (float) System.Math.Sin(MonoGame.rotation),
                (float) -System.Math.Cos(MonoGame.rotation));
            direction.Normalize();

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                transformMatrix: Core.ViewportAdapter.GetScaleMatrix() * Core.ScaleToDevice);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "TotalGameTime: " + gameTime.TotalGameTime.ToString(),
                    new Vector2(0.0f, 0.0f),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "FPS: " + (1 / (float) gameTime.ElapsedGameTime.TotalSeconds).ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "EntityCount: " + MonoGame.World.EntityCount,
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 2),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Camera: " + Core.Camera.Position.X.ToString("0") + ", " + Core.Camera.Position.Y.ToString("0") + ", " + Core.Camera.Zoom.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 3),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                var pointToScreen = Core.ViewportAdapter.PointToScreen(new Point(Core.MouseState.X, Core.MouseState.Y));
                _spriteBatch.DrawString(
                    _spriteFont,
                    "Cursor: " + pointToScreen.X.ToString("0") + ", " + pointToScreen.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 4),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                var screenToWorld = Core.Camera.ScreenToWorld(Core.MouseState.Position.ToVector2());
                _spriteBatch.DrawString(
                    _spriteFont,
                    "ScreenToWorld(): " + screenToWorld.X.ToString("0") + ", " + screenToWorld.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 5),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                var worldToScreen = MonoGame.player[0].Get<AsepriteSprite>().Position; //Core.Camera.WorldToScreen(MonoGame.aseprite.Position);
                _spriteBatch.DrawString(
                    _spriteFont,
                    "Player: " + worldToScreen.X.ToString("0") + ", " + worldToScreen.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 6),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Direction: " + direction.X.ToString("0.0000") + ", " + direction.Y.ToString("0.0000"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 7),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Angle: " + (180 + direction.ToAngle() * 180 / System.Math.PI).ToString("0.00"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 8),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Charge: " + weaponComponent.charge.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 9),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                //var arrow = Assets.Texture("Compass");
                //MonoGame.arrowSprite.Rotation = direction.ToAngle();
                //MonoGame.arrowSprite.Render(_spriteBatch);
            _spriteBatch.End();
        }
    }
}