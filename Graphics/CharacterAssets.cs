using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Graphics.Sprites;
using System;

namespace Graphics.Assets {
    
    public interface ICharacterAsset : IAsset, IDisposable  {
        int Health{ get; }
        int Initiative{ get; }
        int NumberOfTurns{ get; }
        void MoveToLocation(Vector2 location, GameTime gameTime);
        void Kill();
    }// end IRockGuy interface

    public enum RockGuyAnimations { DEATH }

    public class RockGuy : ICharacterAsset, IDisposable {
        public Texture2D Texture{ get { return GetTexture(); } }
        public Rectangle SourceRectangle{ get { return _asset.SourceRectangle; } }
        public Rectangle DestinationRectangle { get { return _asset.DestinationRectangle; } }
        public int Health{ get; private set; }
        public int NumberOfTurns{ get; private set; }
        public int Initiative{ get; private set; }
        public Vector2 Location{ get { return _asset.Location; } }
        public Vector2 DrawingLocation{ get { return _asset.DrawingLocation; } }
        public bool IsAlive{ get; private set; }
        public bool IsDisposed{ get; private set; }

        private HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>> _asset;

        public RockGuy(HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>> asset, int health, int initiative, int numberOfTurns) {
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
            IsAlive = true;
        }// end constructor

        private Texture2D GetTexture() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            return _asset.Texture;
        }// end GetTexture()

        public void MoveToLocation(Vector2 location, GameTime gameTime) {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            if(IsAlive)
                _asset.MoveToLocation(location, gameTime);
        }// end MoveToLocation()

        public void Dispose() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            _asset.Dispose();
            IsDisposed = true;
        }// end Dispose()

        public void UpdateSprite() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            _asset.AssetSprite.Update();
        }// end Update()

        public void Kill() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            IsAlive = false;
            _asset.AssetSprite.PlayFinalAnimation(RockGuyAnimations.DEATH);
        }// end Kill()

        public void BringBackFromDead() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            IsAlive = true;
            _asset.AssetSprite.ResetSprite();
        }

    }// end RockGuy class

    public class CharacterFactory {
        public RockGuy BuildRockGuy(TestingTactics.Game1 game, Vector2 location) {
            int MAX_SPEED = 1000;
            int ACCELERATION = 10;
            int HEALTH = 3;
            int INITIATIVE = 10;
            int NUM_OF_TURNS = 2;
            // Assigns the Path
            string path = "./Characters/";
            var mainSprite = BuildRockGuySprite(game, path);
            // Build Asset
            HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>> asset = 
                                    new HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>>(mainSprite, location, MAX_SPEED, ACCELERATION);
            return new RockGuy(asset, HEALTH, INITIATIVE, NUM_OF_TURNS);
        }// end BuildRockGuy()

        private PlayerSprite<RockGuyAnimations> BuildRockGuySprite(TestingTactics.Game1 game, string path) {
            // Load Textures
            Texture2D idleSprite = game.Content.Load<Texture2D>(path + "BandanGuyStandingAnim");
            Texture2D movingSpriteLeft = game.Content.Load<Texture2D>(path + "BlueBandanaAnimLeft");
            Texture2D movingSpriteRight = game.Content.Load<Texture2D>(path + "BlueBandanaAnimRight");
            Texture2D deathSprite = game.Content.Load<Texture2D>(path + "RockGuyDeathAnim");
            // Build Animated Sprite
            AnimatedSprite idleAnimation = new AnimatedSprite(idleSprite, 6);
            AnimatedSprite movingLeftAnimationU = new AnimatedSprite(movingSpriteLeft, 18);     // U stands for Uncontrolled
            AnimatedSprite movingRightAnimationU = new AnimatedSprite(movingSpriteRight, 18);
            AnimatedSprite deathAnimationU = new AnimatedSprite(deathSprite, 9);
            // Builds Controlled Animated Sprites
            ControlledAnimatedSprite movingLeftAnimationC = new ControlledAnimatedSprite(movingLeftAnimationU, 6, 10);
            ControlledAnimatedSprite movingRightAnimationC = new ControlledAnimatedSprite(movingRightAnimationU, 6, 10);
            // Builds non repeatable animations
            NoRepeatSprite deathAnimationC = new NoRepeatSprite(deathAnimationU);
            // Puts sprites into list
            NoRepeatSprite[] animationList = new NoRepeatSprite[]{ deathAnimationC };
            RockGuyAnimations[] animationNamesList = new RockGuyAnimations[]{ RockGuyAnimations.DEATH };
            // Build SimpleMovingSprite
            SimpleMovingSprite baseSprite = new SimpleMovingSprite(idleAnimation, movingRightAnimationC, movingLeftAnimationC);
            // Builds Main Sprite
            return new PlayerSprite<RockGuyAnimations>(baseSprite, animationList, animationNamesList);
        }// end BuildRockGuySprite()

    }// end Factory Class

}// end Graphics.Assets namespace