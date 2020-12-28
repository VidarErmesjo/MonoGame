using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Entities;

namespace MonoGame.Components
{
    public class HUD : IDisposable
    {
        private bool isDisposed;

        private readonly MonoGame _game;
        private SpriteFont _spriteFont;

        private float _fps;
        private System.TimeSpan _totalGameTime;
        private MouseState _mouseState;

        private Vector2 _direction;

        private Player _player;
        private OrthographicCamera _camera;
        private ViewportAdapter _viewportAdapter;

        public HUD(MonoGame game)
        {
            _game = game;
            _spriteFont = null;
        }
        
        public void LoadContent()
        {
            _spriteFont = _game.AssetManager.Font("Consolas");
        }

        public void Initialize()
        {
            _camera = _game.GameManager.Camera;
            _player = _game.EntityManager.Players.First();
            _viewportAdapter = _game.GameManager.ViewportAdapter;
        }   

        public void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            _direction = new Vector2(-MathF.Sin(_player.Rotation), MathF.Cos(_player.Rotation));

            _mouseState = _game.GameManager.MouseState;

            _totalGameTime = gameTime.TotalGameTime;

            _fps = 1 / (float) deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                transformMatrix: _viewportAdapter.GetScaleMatrix() * _game.GameManager.ScaleToDevice);

               spriteBatch.DrawString(
                    _spriteFont,
                    "TotalGameTime: " + _totalGameTime.ToString(),
                    new Vector2(0.0f, 0.0f),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "FPS: " + _fps.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * _game.GameManager.ScaleToDevice),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "EntityCount: " + _game.EntityManager.Count.ToString(),
                    new Vector2(0.0f, _spriteFont.LineSpacing * _game.GameManager.ScaleToDevice * 2),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "Camera: " + _camera.Position.X.ToString("0") + ", " + _camera.Position.Y.ToString("0") + ", " + _camera.Zoom.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * _game.GameManager.ScaleToDevice * 3),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                var pointToScreen = _viewportAdapter.PointToScreen(new Point(_mouseState.X, _mouseState.Y));
                spriteBatch.DrawString(
                    _spriteFont,
                    "Cursor: " + pointToScreen.X.ToString("0") + ", " + pointToScreen.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * _game.GameManager.ScaleToDevice * 4),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                var screenToWorld = _camera.ScreenToWorld(_mouseState.Position.ToVector2());
                spriteBatch.DrawString(
                    _spriteFont,
                    "ScreenToWorld(): " + screenToWorld.X.ToString("0") + ", " + screenToWorld.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * _game.GameManager.ScaleToDevice * 5),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "Player: " + _player.Position.X.ToString("0") + ", " + _player.Position.Y.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * _game.GameManager.ScaleToDevice * 6),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "Direction: " + _direction.X.ToString("0.0000") + ", " + _direction.Y.ToString("0.0000"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * _game.GameManager.ScaleToDevice * 7),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                spriteBatch.DrawString(
                    _spriteFont,
                    "Angle: " + (180 + _direction.ToAngle() * 180 / System.Math.PI).ToString("0.00"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * _game.GameManager.ScaleToDevice * 8),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);

                /*spriteBatch.DrawString(
                    _spriteFont,
                    "Charge: " + weaponComponent.charge.ToString("0"),
                    new Vector2(0.0f, _spriteFont.LineSpacing * _game.GameManager.ScaleToDevice * 9),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _game.GameManager.ScaleToDevice,
                    SpriteEffects.None,
                    0);*/

            spriteBatch.End();
        }

        public void UnloadContent()
        {
            this.Dispose();
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
            {
                _game.Dispose();
                _player.Dispose();
                _viewportAdapter.Dispose(); 
            }

            isDisposed = true;
        }
   
        ~HUD()
        {
            this.Dispose(false);
        }
    }
}