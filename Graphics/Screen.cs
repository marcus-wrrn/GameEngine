using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Graphics.Rendering;
using Utilities;

namespace Graphics {
    public sealed class Screen : IDisposable {
        public int Width{
            get { return _target.Width; }
        }
        public int Height {
            get { return _target.Height; }
        }

        private readonly static int MIN_DIMENSION = 64;
        private readonly static int MAX_DIMENSION = 4096;
        private bool _isDisposed;
        private bool _isSet;
        private Game _game;
        private RenderTarget2D _target;

        public Screen(Game game, int width, int height) 
        {
            // Get pointer to game
            _game = game ?? throw new ArgumentNullException("Game");
            // Initialize Target
            SetTarget(width, height);
            // Raise all flags to their default values
            _isDisposed = false;
            _isSet = false;
        }// end constructor

        private void SetTarget(int width, int height) {
            width = Util.Clamp(width, MIN_DIMENSION, MAX_DIMENSION);
            height = Util.Clamp(height, MIN_DIMENSION, MAX_DIMENSION);
            _target = new RenderTarget2D(_game.GraphicsDevice, width, height);
        } 

        public void Dispose() {
            if(_isDisposed)
                return;
            _target?.Dispose();
            this._isDisposed = true;
        }// end Dispose

        public void Set() {
            if(_isSet)
                throw new Exception("Render target already set");
            _game.GraphicsDevice.SetRenderTarget(_target);
            _isSet = true;
        }// end Set()
        
        public void UnSet() {
            if(!_isSet)
                throw new Exception("Render target is already unset");
            _game.GraphicsDevice.SetRenderTarget(null);
            _isSet = false;
        }// end UnSet()

        public void Present(SpriteBunch sprites, bool textureFiltering = true) {
            if(sprites is null)
                throw new ArgumentNullException("sprites");
#if DEBUG
                _game.GraphicsDevice.Clear(Color.HotPink);
#else
                _game.GraphicsDevice.Clear(Color.Black);
#endif

            Rectangle destinationRectangle = CalculateDestinationRectangle();
            sprites.Begin(textureFiltering);
            var sprite = new Graphics.Sprites.Sprite(_target);
            sprites.Draw(new Assets.Asset<Graphics.Sprites.Sprite>(sprite), null, destinationRectangle, Color.White);
            sprites.End();
        }

        private Rectangle CalculateDestinationRectangle() {
            Rectangle backBufferBounds = _game.GraphicsDevice.PresentationParameters.Bounds;
            float backBufferAspectRatio = (float)backBufferBounds.Width / backBufferBounds.Height;
            float screenAspectRatio = (float)this.Width / this.Height;

            return CalcDestinationRectangleHelper(backBufferBounds, backBufferAspectRatio, screenAspectRatio);
        }

        private Rectangle CalcDestinationRectangleHelper(Rectangle backBufferBounds, float backBufferAspectRatio, float screenAspectRatio) {
            // Initialize bounds of the rectangle
            float rectangleX = 0f;
            float rectangleY = 0f;
            float rectangleWidth = backBufferBounds.Width;
            float rectangleHeight = backBufferBounds.Height;
            // Adjust bounds to be inside the chosen ratio
            if(backBufferAspectRatio > screenAspectRatio) {
                rectangleWidth = rectangleHeight * screenAspectRatio;
                rectangleX = (float)(backBufferBounds.Width - rectangleHeight) / 2f;
            }
            else if (backBufferAspectRatio < screenAspectRatio) {
                rectangleHeight = rectangleWidth / screenAspectRatio;
                rectangleY = (float)(backBufferBounds.Height - rectangleHeight) / 2f;
            }
            // Return destination rectangle
            return new Rectangle((int)rectangleX, (int)rectangleY, (int)rectangleWidth, (int)rectangleHeight);
        }

    }// end Screen
}