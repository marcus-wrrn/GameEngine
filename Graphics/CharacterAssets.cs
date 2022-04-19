using Microsoft.Xna.Framework;
using Graphics.Sprites;
using System;

namespace Graphics.Assets {
    
    public interface IRockGuy {
        int Health{ get; }
        int Initiative{ get; }
        void MoveToLocation(Vector2 location);
        void Kill();
    }// end IRockGuy interface

    public class RockGuy<T> : IRockGuy, IDisposable where T: ISimpleMovingSprite {
        public int Health{ get; private set; }
        public int NumberOfTurns{ get; private set; }
        public int Initiative{ get; private set; }
        public Vector2 Location{ get { return _asset.LocationOnMap.Location; } }
        private bool _isAlive;
        private bool _isDisposed;
        private HorizontalMovingAsset<T> _asset;


        public RockGuy(HorizontalMovingAsset<T> asset, int health, int initiative, int numberOfTurns) {
            // Error checking
            if(asset == null)
                throw new NullReferenceException("Null RockGuy asset");
            if(health <= 0)
                throw new ArgumentOutOfRangeException("Health cannot be negative");
            if(numberOfTurns <= 0)
                throw new ArgumentOutOfRangeException("Turn number cannot be less than or equal to zero");
            if(initiative <= 0)
                throw new ArgumentOutOfRangeException("Initiative cannot be less than zero");
            // Assigning values
            _asset = asset;
            Health = health;
            Initiative = initiative;
            NumberOfTurns = numberOfTurns;
            _isAlive = true;
        }// end constructor

        public void Dispose() {
            if(_isDisposed)
                return;
        }

    }// end RockGuy class

}// end Graphics.Assets namespace