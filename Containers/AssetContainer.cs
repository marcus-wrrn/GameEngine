using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Graphics.Assets;
using Graphics.Rendering;

namespace Containers {
    // To be moved into a container namespace later
    public interface IBaseAssetContainer : IDisposable {
        Classifier.AssetClassifier AssetInfo { get; }
        Rectangle DestinationRectangle { get; }
        Vector2 Location { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBunch spriteBunch);
    }// end IBaseAssetContainer()

    public interface IMovingAssetContainer : IBaseAssetContainer {
        float AssetSpeed { get; }
        void ChangeAssetSpeed(float speed);
        // void MoveAssetUp(GameTime gameTime);
        // void MoveAssetDown(GameTime gameTime);
        // void MoveAssetLeft(GameTime gameTime);
        // void MoveAssetRight(GameTime gameTime);
        void MoveAssetToLocation(Vector2 vector);
        //void MoveAssetInDirection(Vector2 vector, GameTime gameTime);
        void Stop();
    }// end IMovingAssetContainer interface

    public interface ICharacterAssetContainer : IMovingAssetContainer {
        Classifier.CharacterClassifier CharacterInfo { get; }
        uint CharacterHealth { get; }
        uint CharacterMaxHealth { get; }
        bool IsCharacterAlive { get; }
        int CharacterInitiative { get; }
        uint CharacterNumberOfTurns { get; }
        
    }// end ICharacterAssetContainer()

    public class AssetContainer<T> : IBaseAssetContainer where T : IAsset {
        protected T _asset;
        public virtual Classifier.AssetClassifier AssetInfo { get; private set; }
        // Will later contain a Controller Object and a pointer to the MasterAssetContainer
        public Rectangle DestinationRectangle { get { return _asset.DestinationRectangle; } }
        public Vector2 Location { get { return _asset.Location; } }
        public bool IsDisposed { get; private set; }

        public AssetContainer(T asset, Classifier.AssetClassifier info) {
            AssetInfo = info;
            _asset = asset;
            IsDisposed = false;
        }// end AssetContainer constructor
        
        // Makes a base static object
        public AssetContainer(T asset) : this(asset, new Classifier.AssetClassifier(true)) {}

        public virtual void Dispose() {
            if(IsDisposed)
                throw new ObjectDisposedException("Container already disposed");
            _asset.Dispose();
            IsDisposed = true;
        }// end Dispose()

        public virtual void Update(GameTime gameTime) {
            _asset.Update();
        }// end Update()

        public virtual void Draw(SpriteBunch spriteBunch) {
            spriteBunch.Draw(_asset.Texture, _asset.SourceRectangle, _asset.DestinationRectangle, Color.AliceBlue);
        }// end Draw()

    }// end AssetContainer class

    public class MovingAssetContainer<T> : AssetContainer<T>, IMovingAssetContainer where T : IMovingAsset {
        public float AssetSpeed { get { return _asset.Speed; } }
        public bool IsMoving { get; private set; }
        private Vector2 _locationToMove;
        
        public MovingAssetContainer(T asset, Classifier.AssetClassifier classifier) : base(asset, classifier) {
            IsMoving = false;
            _locationToMove = asset.Location;
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
            _locationToMove = location;
        }// end MoveAssetToLocation()

        public virtual void Stop() {
            if(AssetInfo.IsStatic)
                throw new MethodAccessException("Asset is currently static");
            _locationToMove = Location;
            IsMoving = false;
            _asset.Stop();
        }// end Stop()

        // This section will be replaced with a specific controller for a more defined container
        public override void Update(GameTime gameTime)
        {
            if(AssetInfo.IsStatic) {
                base.Update(gameTime);
                return;
            }
            if(IsMoving) {
                _asset.MoveToLocation(_locationToMove, gameTime);
                base.Update(gameTime);
            }

            // If Asset has reached its destination stop moving
            // Remember that once an asset is in a specific range of a point it will automatically set its location to that point
            if(IsMoving && _locationToMove == Location)
                IsMoving = false;
        }// end Update()

    }// end MovingAssetContainer class

    public class CharacterContainer<T> : MovingAssetContainer<T>, ICharacterAssetContainer where T: ICharacterAsset {
        // A bit redundant but allows for characters to be classified alongside non character assets
        public override Classifier.AssetClassifier AssetInfo { get { return CharacterInfo; } }
        public Classifier.CharacterClassifier CharacterInfo { get; private set; }
        public uint CharacterHealth { get { return _asset.Health; } }
        public uint CharacterMaxHealth { get { return _asset.MaxHealth; } }
        public int CharacterInitiative { get { return _asset.Initiative; } }
        public uint CharacterNumberOfTurns { get { return _asset.NumberOfTurns; } }
        public bool IsCharacterAlive { get { return _asset.IsAlive; } }

        public CharacterContainer(T asset, Classifier.CharacterClassifier characterClassifier) : base(asset, characterClassifier) {
            CharacterInfo = characterClassifier;
        }// end CharacterContainer constructor

    }// end CharacterClassifier


    public class MasterAssetContainer {
        // Note while HashSet would be useful for getting rid of duplicates, it's important that I'm able to sort the main list of arrays
        // This is important for rendering objects by their location
        // All other collections of assets will be in Hashsets that will then be fed into the main list 
        public List<IBaseAssetContainer>           AllAssets { get; private set; }              // All Assets will be stored in this list (useful for drawing or map wide effect)
        public HashSet<IBaseAssetContainer>        NonSentiantObjects { get; private set; }     // Contains all nonsentiant objects (like rocks, trees, pillars....)
        public HashSet<IBaseAssetContainer>        StaticObjects { get; private set; }          // Static objects go here
        public HashSet<IMovingAssetContainer>      MovingObjects { get; private set; }         // Objects that move but don't have a sophistcated AI go here
        public HashSet<ICharacterAssetContainer>   AllCharacters { get; private set; }         // Every Asset with an AI go here 
        public HashSet<ICharacterAssetContainer>   NonPlayerCharacters { get; private set; }   // All NPCs go here
        public HashSet<ICharacterAssetContainer>   PlayerCharacters { get; private set; }      // All Player Characters here

        // TODO: Further optomizations
        public MasterAssetContainer(List<IBaseAssetContainer> staticObjects, List<IMovingAssetContainer> movingObjects, List<ICharacterAssetContainer> characters) {
            // Sort into corresponding containers
            LoadStaticObjects(staticObjects);
            LoadMovingObjects(movingObjects);
            LoadCharacters(characters);

        }// end construtor

        private void LoadStaticObjects(List<IBaseAssetContainer> staticAssets) {
            foreach(var asset in staticAssets) {
                if(asset.AssetInfo.IsStatic) {
                    AddStaticObject(asset);
                    AddNonSentiantObject(asset);
                }
                else
                    throw new ArgumentException("Asset in static asset list not static");
            }
        }// end LoadStaticSet()

        private void LoadMovingObjects(List<IMovingAssetContainer> movingAssets) {
            foreach(var asset in movingAssets) {
                if(!asset.AssetInfo.IsStatic) {
                    // Add to moving objects list
                    AddMovingObject(asset);
                    AddNonSentiantObject(asset);
                }
                else
                    throw new ArgumentException("Asset in moving asset list is static");
            }
        }// end LoadMovingAsset

        private void LoadCharacters(List<ICharacterAssetContainer> characters) {
            foreach(var character in characters) {
                AddAllCharacterObject(character);
                if(character.CharacterInfo.IsPlayerControlled)
                    AddPlayerCharacterObject(character);
                else
                    AddNonPlayerCharacterObject(character);
            }
        }// end LoadCharacters()

        private void LoadAllAssetsIntoMainList() {
            foreach(var obj in NonSentiantObjects) {
                AllAssets.Add(obj);
            }
            foreach(var obj in AllCharacters) {
                AllAssets.Add(obj);
            }
        }// end LoadAllAssetsIntoMainList()

        // Adders (Mainly for debugging)
        private void AddMovingObject(IMovingAssetContainer asset) {
            if(!MovingObjects.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddMovingObject()

        private void AddStaticObject(IBaseAssetContainer asset) {
            if(!StaticObjects.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddStaticObject()

        private void AddNonSentiantObject(IBaseAssetContainer asset) {
            if(asset.AssetInfo.IsSentiant)
                throw new ArgumentException("Object cannot be sentiant");
            if(!NonSentiantObjects.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddNonSentiantObject()

        private void AddNonPlayerCharacterObject(ICharacterAssetContainer asset) {
            if(!NonPlayerCharacters.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddNonPlayerCharacterObject()

        private void AddPlayerCharacterObject(ICharacterAssetContainer asset) {
            if(!PlayerCharacters.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddPlayerCharacterObject()

        private void AddAllCharacterObject(ICharacterAssetContainer asset) {
            if(!AllCharacters.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddAllCharacterObject()

        // For Deletion

        private void DeleteMovingObject(IMovingAssetContainer obj) {
            
        }// end DeleteMovingObject()

        public void AddCharacter(ICharacterAssetContainer character) {
            // Determine which type of character to add to which set
            if(character.CharacterInfo.IsPlayerControlled)
                AddPlayerCharacterObject(character);
            else
                AddNonPlayerCharacterObject(character);
            // Add to the main sets
            AddAllCharacterObject(character);
            AllAssets.Add(character);
        }// end AddCharacter()

        public void AddObject(IMovingAssetContainer obj) {
            if(obj.AssetInfo.IsSentiant)
                throw new ArgumentException("Sentiant Character cannot be treated as an object");
            if(obj.AssetInfo.IsStatic)
                AddStaticObject(obj);
            else
                AddMovingObject(obj);
            // Add to the main sets
            AddNonSentiantObject(obj);
            AllAssets.Add(obj);
        }// end AddObject()

        public void AddObject(IBaseAssetContainer obj) {
            if(obj.AssetInfo.IsSentiant)
                throw new ArgumentException("Sentiant Character cannot be treated as an object");
            if(!obj.AssetInfo.IsStatic)
                throw new ArgumentException("Cannot be a dynamic object yet inherit from the base asset container");
            AddStaticObject(obj);
            AddNonSentiantObject(obj);
            AllAssets.Add(obj);
        }// end AddObject()

        public void DeleteObject(IMovingAssetContainer obj) {
            if(!obj.AssetInfo.IsStatic) {

            }
            else {

            }
        }// end DeleteObject()

        private void SortAssets() {
            AllAssets.Sort(delegate(IBaseAssetContainer x, IBaseAssetContainer y) { 
                return x.Location.Y.CompareTo(y.Location.Y); 
            });
        }// end SortAssets()

        public void Update(GameTime gameTime) {
            // Update all Assets
            foreach(var obj in AllAssets)
                obj.Update(gameTime);
            // Sort Assets for rendering
            SortAssets();
        }// end Update()

        public void Draw(SpriteBunch spriteBunch) {
            foreach(var obj in AllAssets) {
                obj.Draw(spriteBunch);
            }
        }

    }// end AssetContainer class

    
}// end namespace


