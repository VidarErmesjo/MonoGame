using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Entities;

namespace MonoGame
{
    public class HUD
    {
        private readonly SpriteFont _spriteFont;

        private float _fps;
        private System.TimeSpan _totalGameTime;
        private MouseState _mouseState;

        private Vector2 _direction;

        private Actor _player;
        private EntityManager _entityManager;
        private OrthographicCamera _camera;
        private ViewportAdapter _viewportAdapter;

        public HUD(SpriteFont spriteFont)
        {
            _spriteFont = spriteFont;
        }

        public void Mount(Actor player)
        {
            _player = player != null ? player : null;
        }

        public void Mount(EntityManager entityManager)
        {
            _entityManager = entityManager != null ? entityManager : null;
        }        

        public void Mount(OrthographicCamera camera)
        {
            _camera = camera != null ? camera : null;   
        }

        public void Mount(ViewportAdapter viewportAdapter)
        {
            _viewportAdapter = viewportAdapter != null ? viewportAdapter : null;
        }

        public void Update(GameTime gameTime)
        {
            if(_player == null || _entityManager == null || _camera == null)
                return;

            var deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            _direction = new Vector2(-MathF.Sin(_player.Rotation), MathF.Cos(_player.Rotation));

            _mouseState = Mouse.GetState();

            _totalGameTime = gameTime.TotalGameTime;

            _fps = 1 / (float) deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(_player == null || _entityManager == null || _camera == null || _viewportAdapter == null)
                return;

            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                transformMatrix: _viewportAdapter.GetScaleMatrix() * Core.ScaleToDevice);

               spriteBatch.DrawString(
                    _spriteFont,
                    "TotalGameTime: " + _totalGameTime.ToString(),
                    new Vector2(0.0f, 0.0f),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "FPS: " + _fps.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "EntityCount: " + _entityManager.Count.ToString(),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 2),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "Camera: " + _camera.Position.X.ToString("0") + ", " + _camera.Position.Y.ToString("0") + ", " + _camera.Zoom.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 3),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                var pointToScreen = _viewportAdapter.PointToScreen(new Point(_mouseState.X, _mouseState.Y));
                spriteBatch.DrawString(
                    _spriteFont,
                    "Cursor: " + pointToScreen.X.ToString("0") + ", " + pointToScreen.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 4),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                var screenToWorld = _camera.ScreenToWorld(_mouseState.Position.ToVector2());
                spriteBatch.DrawString(
                    _spriteFont,
                    "ScreenToWorld(): " + screenToWorld.X.ToString("0") + ", " + screenToWorld.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 5),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                var worldToScreen = _player.Position;
                spriteBatch.DrawString(
                    _spriteFont,
                    "Player: " + worldToScreen.X.ToString("0") + ", " + worldToScreen.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 6),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "Direction: " + _direction.X.ToString("0.0000") + ", " + _direction.Y.ToString("0.0000"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 7),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "Angle: " + (180 + _direction.ToAngle() * 180 / System.Math.PI).ToString("0.00"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 8),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                /*spriteBatch.DrawString(
                    _spriteFont,
                    "Charge: " + weaponComponent.charge.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * Core.ScaleToDevice * 9),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Core.ScaleToDevice,
                    SpriteEffects.None,
                    0);*/

            spriteBatch.End();
        }
    }
}