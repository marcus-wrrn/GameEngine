using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Graphics.Sprites;
using System.IO;
using System;

namespace Graphics.Assets {
    
    public interface ICharacterAsset : IMovingAsset, IDisposable  {
        uint Health{ get; }
        uint MaxHealth{ get; }
        bool IsAlive { get; }
        int Initiative{ get; }
        uint NumberOfTurns{ get; }
        //void MoveToLocation(Vector2 location, GameTime gameTime);
        void Kill();
    }// end IRockGuy interface



    public class BaseCharacter<T> : ICharacterAsset where T : IMovingAsset {
        public Texture2D Texture{ get { return GetTexture(); } }
        public Rectangle SourceRectangle{ get { return _asset.SourceRectangle; } }
        public Rectangle DestinationRectangle { get { return _asset.DestinationRectangle; } }
        public uint Health{ get; protected set; }
        public uint MaxHealth { get; protected set; }
        public uint NumberOfTurns{ get; protected set; }
        public int Initiative{ get; protected set; }
        public float Speed { get { return _asset.Speed; } }
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

        public virtual void MoveUp(GameTime gameTime) {
            if(IsAlive)
                _asset.MoveUp(gameTime);
        }// end MoveUp()

        public virtual void MoveDown(GameTime gameTime) {
            if(IsAlive)
                _asset.MoveDown(gameTime);
        }// end MoveDown()

        public virtual void MoveLeft(GameTime gameTime) {
            if(IsAlive)
                _asset.MoveLeft(gameTime);
        }// end MoveLeft()

        public virtual void MoveRight(GameTime gameTime) {
            if(IsAlive)
                _asset.MoveRight(gameTime);
        }// end MoveRight()

        public void ChangeSpeed(float speed) {
            _asset.ChangeSpeed(speed);
        }// end ChangeSpeed()

        public virtual void Stop() {
            if(IsAlive)
                _asset.Stop();
        }// end Stop()

        public void MoveDirection(Vector2 direction, GameTime gameTime) {
            _asset.MoveDirection(direction, gameTime);
        }// end MoveDirection()

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

}// end Graphics.Assets namespace