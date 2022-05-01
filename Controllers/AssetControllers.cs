using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Graphics.Rendering;
using Graphics.Assets;


namespace AssetControl {
    // Need controllers for moving, static and sentient non player controlled assets
    // Should have a player controller as well (Maybe just call it player)

    // Controllers can be general but also asset specific
    // For example some static assets should only update their sprites when certain criteria are met

    // Moving sprites should have a path to follow and their sprites will continue to follow that path
    // Their paths should be able to be changed/Updated for certain criteria
    // Maybe implement an AI type object (Look up data structure to help)
    // Should the controller operate like the AI?
    // Right main controller works with collision + updating AI?
    // WTF this needs to be reworked (Controllers cannot deal with both collision and AI)

    // What should the base Controller interface have
    // Draw(SpriteBunch) SpriteBunch should be included as an asset can be drawn differently
    // Update(GameTime gameTime) gameTime is important for movement but not animation
    // SourceRectangle
    // DestinationRectangle
    // Get Asset (Problem since not all assets have the same generic types, so shouldn't include it)

    public interface IAssetController : IDisposable {
        //Texture2D Texture { get; }
        Rectangle AssetSourceRectangle{ get; }
        Rectangle AssetDestinationRectangle{ get; }
        void Update();
        void Draw(SpriteBunch spriteBunch);
    }// end IAssetController interface

    public class AssetController<T> : IAssetController where T: IAsset {
        private T _asset;
        // Determines how fast the animation of the sprite runs for
        private int _animationSpeed;
        private int _animationCounter;
        public bool IsDisposed{ get; private set; }
        public Rectangle AssetSourceRectangle{ get { return _asset.SourceRectangle; } }
        public Rectangle AssetDestinationRectangle{ get { return _asset.DestinationRectangle; } }

        public AssetController(T asset, int animationSpeed) {
            if(animationSpeed <= 0 || animationSpeed >= 50)
                throw new ArgumentOutOfRangeException("animation speed cannot br less than or equal to zero or greater than 50");
            _animationSpeed = animationSpeed;
            if(asset == null)
                throw new NullReferenceException("asset cannot be null");
            _asset = asset;
            _animationCounter = 0;
            IsDisposed = false;
        }// end constructor

        public virtual void Dispose() {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller already disposed");
            _asset.Dispose();
            IsDisposed = true;
        }// end Dispose()

        public virtual void Update() {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller already disposed");
            if(_animationCounter >= _animationSpeed) {
                _asset.UpdateSprite();
                _animationCounter = 0;
            }
            else
                _animationCounter++;
        }// end Update()

        public virtual void Draw(SpriteBunch spriteBunch) {
            spriteBunch.Draw(_asset.Texture, _asset.SourceRectangle, _asset.DestinationRectangle, Color.AliceBlue);
        }// end Draw()

    }// end AssetController class

}// end AssetControl namespace