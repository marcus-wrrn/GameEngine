using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Graphics.Sprites;
using System;

namespace Graphics.Assets {
    
    public interface IRockGuy : IDisposable {
        int Health{ get; }
        int Initiative{ get; }
        void MoveToLocation(Vector2 location, GameTime gameTime);
        void Kill();
    }// end IRockGuy interface

    public class RockGuy : IRockGuy, IDisposable {
        public int Health{ get; private set; }
        public int NumberOfTurns{ get; private set; }
        public int Initiative{ get; private set; }
        public Vector2 Location{ get { return _asset.LocationOnMap.Location; } }
        public bool IsAlive{ get; private set; }
        public bool IsDisposed{ get; private set; }
        private HorizontalMovingAsset<SimpleMovingSprite> _asset;
        private Asset<ControlledAnimatedSprite> _deadAsset;


        public RockGuy(HorizontalMovingAsset<SimpleMovingSprite> asset, Asset<ControlledAnimatedSprite> deathAsset, int health, int initiative, int numberOfTurns) {
            // Error checking
            if(asset == null)
                throw new NullReferenceException("Null RockGuy asset");
            if(deathAsset == null)
                throw new NullReferenceException("Null Rockguy asset");
            if(health <= 0)
                throw new ArgumentOutOfRangeException("Health cannot be negative");
            if(numberOfTurns <= 0)
                throw new ArgumentOutOfRangeException("Turn number cannot be less than or equal to zero");
            if(initiative <= 0)
                throw new ArgumentOutOfRangeException("Initiative cannot be less than zero");
            // Assigning values
            _asset = asset;
            _deadAsset = deathAsset;
            Health = health;
            Initiative = initiative;
            NumberOfTurns = numberOfTurns;
            IsAlive = true;
        }// end constructor

        public void MoveToLocation(Vector2 location, GameTime gameTime) {
            if(IsAlive)
                _asset.MoveToLocation(location, gameTime);
        }// end MoveToLocation()

        public void Dispose() {
            if(IsDisposed)
                return;
            _asset.Dispose();
            IsDisposed = true;
        }// end Dispose()

        public void Kill() {
            IsAlive = false;
        }// end Kill()

    }// end RockGuy class

}// end Graphics.Assets namespace