using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Graphics.Assets;
using Graphics.Rendering;

namespace Containers {
    // To be moved into a container namespace later
    public interface IBaseAssetContainer : IDisposable {
        bool IsDisposed { get; }
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
            // Test to see if the object inherits from MovingAsset
            IMovingAsset test = asset as IMovingAsset;
            if(test == null && !info.IsStatic)
                throw new ArgumentException("Container cannot posses a dynamic object that inherits from a static asset");
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
        public List<IBaseAssetContainer>           AllAssetContainers { get; private set; }             // All Assets will be stored in this list (useful for drawing or map wide effect)
        public HashSet<IBaseAssetContainer>        NonActiveObjects { get; private set; }      // Contains all nonsentiant objects (like rocks, trees, pillars....)
        public HashSet<IBaseAssetContainer>        ActiveObjects { get; private set; }         // For objects (not characters) who can affect the environment
        public HashSet<IBaseAssetContainer>        StaticObjects { get; private set; }         // Static objects go here
        public HashSet<IMovingAssetContainer>      MovingObjects { get; private set; }         // Objects that move but don't have a sophistcated AI go here
        public HashSet<ICharacterAssetContainer>   AllCharacters { get; private set; }         // Every Asset with an AI go here 
        public HashSet<ICharacterAssetContainer>   NonPlayerCharacters { get; private set; }   // All NPCs go here
        public HashSet<ICharacterAssetContainer>   PlayerCharacters { get; private set; }      // All Player Characters here

        // TODO: Further optomizations
        public MasterAssetContainer(List<IBaseAssetContainer> containers) {
            // Initialize all lists/hashsets
            AllAssetContainers = new List<IBaseAssetContainer>();
            NonActiveObjects = new HashSet<IBaseAssetContainer>();
            ActiveObjects = new HashSet<IBaseAssetContainer>();
            StaticObjects = new HashSet<IBaseAssetContainer>();
            MovingObjects = new HashSet<IMovingAssetContainer>();
            AllCharacters = new HashSet<ICharacterAssetContainer>();
            NonPlayerCharacters = new HashSet<ICharacterAssetContainer>();
            PlayerCharacters = new HashSet<ICharacterAssetContainer>();

            foreach(var container in containers) {
                // If container does not exist inside the main list add it and sort the value into its respective hashsets
                if(!AllAssetContainers.Contains(container)) {
                    AllAssetContainers.Add(container);
                    // Sorts container into all applicable hashsets
                    SortContainer(container);
                }
            }

        }// end construtor

        private void SortContainer(IBaseAssetContainer container) {
            ICharacterAssetContainer characterContainer = container as ICharacterAssetContainer; 
            if(characterContainer != null) {
                LoadCharacterContainer(characterContainer);
                return;
            }
            IMovingAssetContainer movingContainer = container as IMovingAssetContainer;
            if(movingContainer != null) {
                LoadMovingContainer(movingContainer);
                return;
            }
            LoadStaticContainer(container);
        }// end SortContainer()

        private void LoadCharacterContainer(ICharacterAssetContainer container) {
            // Add to main character list
            AddAllCharacterObject(container);
            // Sort object
            if(container.CharacterInfo.IsPlayerControlled)
                AddPlayerCharacterObject(container);
            else
                AddNonPlayerCharacterObject(container);
        }// end LoadCharacterContainer()

        private void LoadMovingContainer(IMovingAssetContainer container) {
            var info = container.AssetInfo;
            if(!info.IsSentiant)
                AddNonActiveObject(container);
            else
                AddActiveObject(container);
            if(!info.IsStatic)
                AddMovingObject(container);
            else
                AddStaticObject(container);
        }// end LoadMovingContainer()

        private void LoadStaticContainer(IBaseAssetContainer container) {
            var info = container.AssetInfo;
            if(!info.IsSentiant)
                AddNonActiveObject(container);
            else
                throw new ArgumentException("static object cannot be sentiant");
            if(info.IsStatic)
                AddStaticObject(container);
            else
                throw new ArgumentException("Static object cannot be non static");
        }

        private void LoadStaticObjects(List<IBaseAssetContainer> staticAssets) {
            foreach(var asset in staticAssets) {
                if(asset.AssetInfo.IsStatic) {
                    AddStaticObject(asset);
                    AddNonActiveObject(asset);
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
                    AddNonActiveObject(asset);
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
            foreach(var obj in NonActiveObjects) {
                AllAssetContainers.Add(obj);
            }
            foreach(var obj in AllCharacters) {
                AllAssetContainers.Add(obj);
            }
        }// end LoadAllAssetsIntoMainList()

        // Adders (Mainly for debugging)
        private void AddMovingObject(IMovingAssetContainer asset) {
            if(asset.AssetInfo.IsStatic)
                throw new Exception("Asset is static");
            if(!MovingObjects.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddMovingObject()

        private void AddStaticObject(IBaseAssetContainer asset) {
            if(asset.AssetInfo.IsStatic)
                throw new Exception("Asset is not static");
            if(!StaticObjects.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddStaticObject()

        private void AddNonActiveObject(IBaseAssetContainer asset) {
            if(asset.AssetInfo.IsSentiant)
                throw new ArgumentException("Object cannot be sentiant");
            if(!NonActiveObjects.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddNonSentiantObject()

        private void AddActiveObject(IBaseAssetContainer asset) {
            if(!asset.AssetInfo.IsSentiant)
                throw new ArgumentException("Object must be listed sentiant");
            if(!ActiveObjects.Add(asset))
                throw new ArgumentException("Duplicate asset found in list");
        }

        private void AddNonPlayerCharacterObject(ICharacterAssetContainer asset) {
            if(asset.CharacterInfo.IsPlayerControlled)
                throw new Exception("Asset cannot be player controlled");
            if(!NonPlayerCharacters.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddNonPlayerCharacterObject()

        private void AddPlayerCharacterObject(ICharacterAssetContainer asset) {
            if(!asset.CharacterInfo.IsPlayerControlled)
                throw new Exception("Asset is not player controlled");
            if(!PlayerCharacters.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddPlayerCharacterObject()

        private void AddAllCharacterObject(ICharacterAssetContainer asset) {
            if(!AllCharacters.Add(asset))
                throw new Exception("Duplicate asset found in list");
        }// end AddAllCharacterObject()

        // For Deletion



        public void DeleteObject(IBaseAssetContainer container) {
            // If the container does not exist in the list exit
            if(!AllAssetContainers.Contains(container))
                return;
            // Remove container from main list
            AllAssetContainers.Remove(container);
            // Check if the container is a character
            var characterContainer = container as ICharacterAssetContainer;
            if(characterContainer != null) {
                DeleteCharacter(characterContainer);
                return;
            }
            // Check if Container is a moving object
            var movingContainer = container as IMovingAssetContainer;
            if(movingContainer != null) {
                DeleteMoving(movingContainer);
                return;
            }
            // Container must be of a static object
            DeleteStatic(container);
        }// end DeleteObject()

        private void DeleteCharacter(ICharacterAssetContainer container) {
            AllCharacters.Remove(container);
            if(container.CharacterInfo.IsPlayerControlled)
                PlayerCharacters.Remove(container);
            else
                NonPlayerCharacters.Remove(container);
        }// end DeleteCharacter()

        private void DeleteMoving(IMovingAssetContainer container) {
            if(!container.AssetInfo.IsSentiant)
                NonActiveObjects.Remove(container);
            else
                ActiveObjects.Remove(container);
            if(container.AssetInfo.IsStatic)
                StaticObjects.Remove(container);
            else
                MovingObjects.Remove(container);
        }// end DeleteMoving()

        private void DeleteStatic(IBaseAssetContainer container) {
            if(container.AssetInfo.IsStatic)
                StaticObjects.Remove(container);
            else
                throw new ArgumentException("Container must be static");
            if(container.AssetInfo.IsSentiant)
                ActiveObjects.Remove(container);
            else
                NonActiveObjects.Remove(container); 
        }// end DeleteStatic()

        private void SortAssets() {
            AllAssetContainers.Sort(delegate(IBaseAssetContainer x, IBaseAssetContainer y) { 
                return x.Location.Y.CompareTo(y.Location.Y); 
            });
        }// end SortAssets()

        private void DeleteObjects(List<IBaseAssetContainer> containers) {
            foreach(var container in containers)
                DeleteObject(container);
        }// end DeleteObjects()

        public void Update(GameTime gameTime) {
            List<IBaseAssetContainer> deletionList = new List<IBaseAssetContainer>();
            // Update all Assets
            foreach(var obj in AllAssetContainers) {
                obj.Update(gameTime);
                if(obj.IsDisposed)
                    deletionList.Add(obj);
            }
            // Deletes all disposed objects
            // Does not delete in main loop to avoid errors
            DeleteObjects(deletionList);
            // Sort Remaining Assets for rendering
            SortAssets();
        }// end Update()

        public void Draw(SpriteBunch spriteBunch) {
            foreach(var obj in AllAssetContainers) {
                obj.Draw(spriteBunch);
            }
        }// end Draw()

    }// end AssetContainer class

    
}// end namespace


