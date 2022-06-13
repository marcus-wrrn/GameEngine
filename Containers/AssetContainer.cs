using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using Graphics.Assets;
using Graphics.Rendering;

namespace Containers {
    // To be moved into a container namespace later
    public interface IBaseAsset : IDisposable {
        bool IsDisposed { get; }
        // Dispose falgs the object for disposal
        bool ToBeDisposed { get; }
        Classifier.AssetClassifier AssetInfo { get; }
        Rectangle DestinationRectangle { get; }
        Vector2 Location { get; }
        Vector2 RenderingLocation{ get; }
        void Save(BinaryWriter binWriter);
        void Update(GameTime gameTime);
        void Draw(SpriteBunch spriteBunch);
    }// end IBaseAssetContainer()

    public interface IMovingAsset : IBaseAsset {
        float AssetSpeed { get; }
        void ChangeAssetSpeed(float speed);
        // void MoveAssetUp(GameTime gameTime);
        // void MoveAssetDown(GameTime gameTime);
        // void MoveAssetLeft(GameTime gameTime);
        // void MoveAssetRight(GameTime gameTime);
        bool IsMoving { get; }
        Vector2 LocationMovingTo { get; }
        void MoveAssetToLocation(Vector2 vector);
        //void MoveAssetInDirection(Vector2 vector, GameTime gameTime);
        void Stop();
    }// end IMovingAssetContainer interface

    public interface ICharacterAsset : IMovingAsset {
        Classifier.CharacterClassifier CharacterInfo { get; }
        IStats CharacterStats { get; }
        void TakeDamage(int value);
        
    }// end ICharacterAssetContainer()

    public class Asset<T> : IBaseAsset where T : IAssetBody {
        protected T _asset;
        public bool ToBeDisposed { get; protected set; }
        public virtual Classifier.AssetClassifier AssetInfo { get; private set; }
        // Will later contain a Controller Object and a pointer to the MasterAssetContainer
        public Rectangle DestinationRectangle { get { return _asset.DestinationRectangle; } }
        public Vector2 Location { get { return _asset.Location; } }
        public Vector2 RenderingLocation { get { return _asset.DrawingLocation; } }
        public bool IsDisposed { get; private set; }
        private readonly int ANIMATION_TIME;
        private int _animationCounter = 0;

        public Asset(T asset, Classifier.AssetClassifier info, int animationSpeed = 7) {
            // Test to see if the object inherits from MovingAsset
            IMovingAssetBody test = asset as IMovingAssetBody;
            if(test == null && !info.IsStatic)
                throw new ArgumentException("Container cannot posses a dynamic object that inherits from a static asset");
            AssetInfo = info;
            _asset = asset;
            IsDisposed = false;
            ToBeDisposed = false;
            ANIMATION_TIME = 7;
        }// end AssetContainer constructor
        
        // Makes a base static object
        public Asset(T asset) : this(asset, new Classifier.AssetClassifier(true)) {}

        public virtual void Dispose() {
            if(IsDisposed)
                throw new ObjectDisposedException("Container already disposed");
            _asset.Dispose();
            IsDisposed = true;
        }// end Dispose()

        public virtual void Update(GameTime gameTime) {
            if(_animationCounter >= ANIMATION_TIME) {
                _asset.Update();
                _animationCounter = 0;
            } 
            else
                _animationCounter++;
        }// end Update()

        public virtual void Draw(SpriteBunch spriteBunch) {
            spriteBunch.Draw(_asset.Texture, _asset.SourceRectangle, _asset.DestinationRectangle, Color.AliceBlue);
        }// end Draw()

        public virtual void Save(BinaryWriter binWriter) {
            // All Save Systems should start with the proper ASSET_CODE, this ensures
            binWriter.Write(_asset.Location.X);
            binWriter.Write(_asset.Location.Y);
        }// end Save()

    }// end AssetContainer class

    public class MovingAsset<T> : Asset<T>, IMovingAsset where T : IMovingAssetBody {
        public float AssetSpeed { get { return _asset.Speed; } }
        public bool IsMoving { get; private set; }
        public Vector2 LocationMovingTo { get; private set; }
        
        public MovingAsset(T asset, Classifier.AssetClassifier classifier) : base(asset, classifier) {
            IsMoving = false;
            LocationMovingTo = asset.Location;
        }// end MovingAssetContainer()

        public void ChangeAssetSpeed(float speed) {
            _asset.ChangeSpeed(speed);
        }// end ChangeAssetSpeed()

        public void MoveAssetToLocation(Vector2 location) {
            // If the object is currently static than a request to move should never have been called
            if(AssetInfo.IsStatic)
                throw new MethodAccessException("Asset is currently static");
            // set IsMoving to true
            IsMoving = true;
            LocationMovingTo = location;
        }// end MoveAssetToLocation()

        public virtual void Stop() {
            if(AssetInfo.IsStatic)
                throw new MethodAccessException("Asset is currently static");
            LocationMovingTo = Location;
            IsMoving = false;
            _asset.Stop();
        }// end Stop()

        // This section will be replaced with a specific controller for a more defined container
        public override void Update(GameTime gameTime) {
            if(AssetInfo.IsStatic) {
                base.Update(gameTime);
                return;
            }
            if(IsMoving) {
                _asset.MoveToLocation(LocationMovingTo, gameTime);
            }
            // If Asset has reached its destination stop moving
            // Remember that once an asset is in a specific range of a point it will automatically set its location to that point
            if(IsMoving && LocationMovingTo == Location)
                IsMoving = false;
            base.Update(gameTime);
            
        }// end Update()

    }// end MovingAssetContainer class

    public class CharacterAsset<T> : MovingAsset<T>, ICharacterAsset where T: ICharacterAssetBody {
        // A bit redundant but allows for characters to be classified alongside non character assets
        public override Classifier.AssetClassifier AssetInfo { get { return CharacterInfo; } }
        public Classifier.CharacterClassifier CharacterInfo { get; private set; }
        public bool IsCharacterAlive { get { return _asset.IsAlive; } }
        public IStats CharacterStats { get; set; }

        public CharacterAsset(T asset, Classifier.CharacterClassifier characterClassifier, IStats stats) : base(asset, characterClassifier) {
            CharacterStats = stats;
            CharacterInfo = characterClassifier;
        }// end CharacterContainer 
        
        public virtual void TakeDamage(int value) {
            if(value > CharacterStats.Health)
                CharacterStats.Health = 0;
            else if (value < 0)
                return;
            else
                CharacterStats.Health -= (uint)value;
        }// end TakeDamage()

        public override void Save(BinaryWriter binWriter) {
            // This should all go into seperate Functions
            // Saves all of the Characters important info
            binWriter.Write(CharacterInfo.Allegiance.ToString());
            binWriter.Write(CharacterInfo.IsPlayerControlled);
            binWriter.Write(CharacterInfo.IsSentiant);
            binWriter.Write(CharacterInfo.IsStatic);
            binWriter.Write(CharacterInfo.Type.ToString());
            // Saves all Character Stats
            binWriter.Write(CharacterStats.MaxHealth);
            binWriter.Write(CharacterStats.Health);
            binWriter.Write(CharacterStats.HitChance);
            binWriter.Write(CharacterStats.CriticalChance);
            binWriter.Write(CharacterStats.Evasion);
            // Saves Asset Info
        }// end Save()

    }// end CharacterClassifier
    
}// end namespace


