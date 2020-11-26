using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Entities.Systems
{
    public class HUDSystem: EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _spriteFont;

        public HUDSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All())
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _spriteFont = Assets.Font("Consolas");
        }

        public override void Initialize(IComponentMapperService mapperService)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            WeaponComponent weaponComponent;
            weaponComponent = MonoGame.player[(int) MonoGame.Player.One].Get<WeaponComponent>();

            var direction = new Vector2(
                (float) System.Math.Sin(MonoGame.rotation),
                (float) -System.Math.Cos(MonoGame.rotation));
            direction.Normalize();

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                transformMatrix: MonoGame.viewportAdapter.GetScaleMatrix());

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
                    "ActiveEntities: " + ActiveEntities.Count,
                    new Vector2(0.0f, _spriteFont.LineSpacing * 2),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Camera: " + MonoGame.camera.Position.X.ToString("0") + ", " + MonoGame.camera.Position.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 3),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Cursor: " + MonoGame.mouseState.X.ToString("0") + ", " + MonoGame.mouseState.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 4),
                    Color.White);

                var screenToWorld = MonoGame.camera.ScreenToWorld(Vector2.One * MonoGame.mouseState.Position.ToVector2());
                _spriteBatch.DrawString(
                    _spriteFont,
                    "ScreenToWorld(): " + screenToWorld.X.ToString("0") + ", " + screenToWorld.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 5),
                    Color.White);

                var worldToScreen = MonoGame.camera.WorldToScreen(Vector2.One * MonoGame.mouseState.Position.ToVector2());
                _spriteBatch.DrawString(
                    _spriteFont,
                    "WorldToScreen(): " + worldToScreen.X.ToString("0") + ", " + worldToScreen.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 6),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Direction: " + direction.X.ToString("0.0000") + ", " + direction.Y.ToString("0.0000"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 7),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Angle: " + (180 + direction.ToAngle() * 180 / System.Math.PI).ToString("0.00"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 8),
                    Color.White);

                _spriteBatch.DrawString(
                    _spriteFont,
                    "Charge: " + weaponComponent.charge.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * 9),
                    Color.White);

                //var arrow = Assets.Texture("Compass");
                MonoGame.arrowSprite.Rotation = direction.ToAngle();
                //MonoGame.arrowSprite.Render(_spriteBatch);
            _spriteBatch.End();
        }
    }
}