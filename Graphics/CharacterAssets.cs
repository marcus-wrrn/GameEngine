using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Graphics.Sprites;
using System.IO;
using System;

namespace Graphics.Assets {
    
    public interface ICharacterAsset : IAsset, IDisposable  {
        uint Health{ get; }
        uint MaxHealth{ get; }
        bool IsAlive { get; }
        int Initiative{ get; }
        uint NumberOfTurns{ get; }
        void MoveToLocation(Vector2 location, GameTime gameTime);
        void Kill();
    }// end IRockGuy interface



    public class BaseCharacter<T> : ICharacterAsset where T : IHorizontalMovingAsset {
        public Texture2D Texture{ get { return GetTexture(); } }
        public Rectangle SourceRectangle{ get { return _asset.SourceRectangle; } }
        public Rectangle DestinationRectangle { get { return _asset.DestinationRectangle; } }
        public uint Health{ get; protected set; }
        public uint MaxHealth { get; protected set; }
        public uint NumberOfTurns{ get; protected set; }
        public int Initiative{ get; protected set; }
        public Vector2 Location{ get { return _asset.Location; } }
        public Vector2 DrawingLocation{ get { return _asset.DrawingLocation; } }
        public bool IsAlive{ get; private set; }
        public bool IsDisposed{ get; private set; }
        protected T _asset;

        public BaseCharacter(T asset, uint health, int initiative, uint numberOfTurns) {
            // Error checking
            if(asset == null)
                throw new NullReferenceException("Null RockGuy asset");
            if(initiative <= 0)
                throw new ArgumentOutOfRangeException("Initiative cannot be less than zero");
            // Assigning values
            _asset = asset;
            MaxHealth = health;
            Health = MaxHealth;
            Initiative = initiative;
            NumberOfTurns = numberOfTurns;
            IsAlive = true;
        }// end constructor

        private Texture2D GetTexture() {
            if(IsDisposed)
                throw new ObjectDisposedException("Asset is disposed");
            return _asset.Texture;
        }// end GetTexture()

        public virtual void MoveToLocation(Vector2 location, GameTime gameTime) {
            if(IsDisposed)
                throw new ObjectDisposedException("Asset is disposed");
            if(IsAlive)
                _asset.MoveToLocation(location, gameTime);
        }// end MoveToLocation()

        public virtual void Dispose() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            _asset.Dispose();
            IsDisposed = true;
        }// end Dispose()

        public virtual void Update() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            // If health has reached zero and is still alive Kill the asset
            // This check shouldn't be neccessary but just in case
            if (Health <= 0 && IsAlive)
                Kill();
        }// end Update()

        public virtual void Kill() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            // set health to zero
            Health = 0;
            IsAlive = false;
        }// end Kill()

        public virtual void BringBackFromDead() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            Health = MaxHealth;
            IsAlive = true;
        }// end BringBackFromDead()

    }// end BaseCharacter class

    public enum RockGuyAnimations { DEATH, TAKE_DAMAGE }

    public class RockGuy : BaseCharacter<HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>>> {


        public RockGuy(HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>> asset, uint health, int initiative, uint numberOfTurns) :
                base(asset, health, initiative, numberOfTurns) { }
        // end constructor

        public override void Update() {
            if(IsDisposed)
                throw new ObjectDisposedException("Rock Guy is disposed");
            // If health has reached zero and is still alive Kill the asset
            // This check shouldn't be neccessary but just in case
            if (Health <= 0 && IsAlive)
                Kill();
            _asset.AssetSprite.Update();
        }// end Update()

        public override void Kill() {
            // Sets the base state to dead
            base.Kill();
            // Update the asset to play a death animation
            _asset.AssetSprite.PlayFinalAnimation(RockGuyAnimations.DEATH);
        }// end Kill()

        public void HitForDamage(int damage) {
            if(damage < 0)
                return;
            // Make sure to not make Health < 0 to avoid overflow
            if(damage > Health)
                Health = 0;
            else
                Health -= (uint)damage;
        }// end HitForDamage()

        public void HealDamage(int healingAmount) {
            if(healingAmount <= 0)
                return;
            if(healingAmount + Health >= MaxHealth)
                Health = MaxHealth;
            else
                Health += (uint)healingAmount;
        }// end HealDamage()

        public override void BringBackFromDead() {
            base.BringBackFromDead();
            _asset.AssetSprite.ResetSprite();
        }// end BringBackFromDead()

        // This should be moved to inside the controller
        public void SaveAsset(BinaryWriter binWriter) {
            try {
                // Writes the name to tell the reader to start reading a rockGuy
                binWriter.Write((string)"RockGuy");
                // Writes Information
                binWriter.Write(IsAlive);
                binWriter.Write(Health);
                binWriter.Write(Initiative);
                binWriter.Write(NumberOfTurns);
                // Writes Location
                binWriter.Write(Location.X);
                binWriter.Write(Location.Y);
            } catch (IOException ioexp) {
                throw new IOException("Error: " + ioexp.Message);
            }
        }// end SaveAsset()

    }// end RockGuy class

    public enum PlayerAnimations { DEATH, ATTACK }

    public class Player : BaseCharacter<HorizontalMovingAsset<PlayerSprite<PlayerAnimations>>> {
        public Player(HorizontalMovingAsset<PlayerSprite<PlayerAnimations>> asset, uint health, int initiative, uint numberOfTurns) :
                base(asset, health, initiative, numberOfTurns) {}
        
        public override void Update() {
            if(IsDisposed)
                throw new ObjectDisposedException("Player is disposed");
            // If health has reached zero and is still alive Kill the asset
            // This check shouldn't be neccessary but just in case
            if (Health <= 0 && IsAlive)
                Kill();
            _asset.AssetSprite.Update();
        }// end Update()

        public override void Kill() {
            // Sets the base state to dead
            base.Kill();
            // Update the asset to play a death animation
            _asset.AssetSprite.PlayFinalAnimation(PlayerAnimations.DEATH);
        }// end Kill()

        public override void BringBackFromDead() {
            base.BringBackFromDead();
            _asset.AssetSprite.ResetSprite();
        }// end BringBackFromDead()

    }// end Player class

    public class CharacterFactory {
        public RockGuy BuildRockGuy(TestingTactics.Game1 game, Vector2 location) {
            int maxSpeed = 1000;
            int acceleration = 10;
            uint health = 3;
            int initiative = 10;
            uint numberOfTurns = 2;
            return BuildRockGuy(game, location, health, initiative, numberOfTurns, acceleration, maxSpeed);
        }// end BuildRockGuy()

        public RockGuy BuildRockGuy(TestingTactics.Game1 game, Vector2 location, uint health = 3, int initiative = 10, uint numberOfTurns = 2, int acceleration = 10, int maxSpeed = 1000) {
            // Assigns the Path
            string path = "./Characters/";
            var mainSprite = BuildRockGuySprite(game, path);
            // Build Asset
            HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>> asset = 
                                    new HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>>(mainSprite, location, maxSpeed, acceleration);
            return new RockGuy(asset, health, initiative, numberOfTurns);
        }

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