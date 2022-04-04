using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using System;

namespace Graphics {
    public sealed class Sprites : IDisposable {
        private SpriteBatch _sprites;
        private BasicEffect _effect;
        private Game _game;
        private bool _isDisposed;
        private bool _hasStarted;

        public Sprites(Game game) {
                if(game is null)
                    throw new ArgumentNullException("game");
                _game = game;
                _isDisposed = false;
                _hasStarted = false;
                _sprites = new SpriteBatch(_game.GraphicsDevice);
                InitializeEffect();
        }// end constructor

        private void InitializeEffect() {
            _effect = new BasicEffect(_game.GraphicsDevice);
            _effect.FogEnabled = false;
            _effect.TextureEnabled = true;
            _effect.LightingEnabled = false;
            _effect.VertexColorEnabled = true;
            _effect.World = Matrix.Identity;
            _effect.Projection = Matrix.Identity;
            _effect.View = Matrix.Identity;
        }

        public void Dispose() {
            if(_isDisposed)
                return;
            _effect?.Dispose();
            _sprites?.Dispose();
            _isDisposed = true;
        }// end Dispose()

        public void Begin(bool isTextureFilteringEnabled) {
            if(_hasStarted)
                throw new Exception("SpriteBatch has already started");
            Viewport vp = _game.GraphicsDevice.Viewport;
            SamplerState sampler = SamplerState.PointClamp;
            if(isTextureFilteringEnabled)
                sampler = SamplerState.LinearClamp;

            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0f, 1f);
            _sprites.Begin(blendState : BlendState.AlphaBlend, samplerState: sampler, rasterizerState: RasterizerState.CullNone, effect: _effect);
            _hasStarted = true;
        }

        public void End() {
            _sprites.End();
            _hasStarted = false;
        }

        public void Draw(Assets.Asset asset, Color color) {
            _sprites.Draw(asset.Texture, asset.LocationOnMap.GetLocationToDraw(), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }


        public void Draw(Assets.Asset asset, Rectangle? sourceRectangle, Vector2 originOfTransformation, Vector2 position, float rotation, Vector2 scale, Color color) {
            _sprites.Draw(asset.Texture, asset.LocationOnMap.GetLocationToDraw(), sourceRectangle, color, rotation, originOfTransformation, scale, SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Assets.Asset asset, Rectangle? sourceRectangle, Rectangle destinationRectangle, Color color) {
            _sprites.Draw(asset.Texture, destinationRectangle, sourceRectangle, color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
        }
    }
}