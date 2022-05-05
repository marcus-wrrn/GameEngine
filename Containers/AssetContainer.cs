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
        float Speed { get; }
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
        uint NumberOfTurns { get; }
        
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
        public float Speed { get { return _asset.Speed; } }
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


    public class MasterAssetContainer {
        public List<IAsset>             AllAssets{ get; private set; }              // All Assets will be stored in this list (useful for drawing or map wide effect)
        public List<IAsset>             StaticAssets { get; private set; }          // Static objects go here
        public List<IMovingAsset>       MovingAssets { get; private set; }         // Assets that move but don't have a sophistcated AI go here
        public List<ICharacterAsset>    AllCharacters { get; private set; }         // Every Asset with an AI go here
        public List<ICharacterAsset>    NonPlayerCharacters { get; private set; }   // All NPCs go here
        public List<ICharacterAsset>    PlayerCharacters { get; private set; }      // All Player Characters here

        // TODO: Further optomizations
        public MasterAssetContainer(List<IAsset> staticAssets, List<IMovingAsset> movingAssets, List<ICharacterAsset> nonPlayerCharacters, List<ICharacterAsset> playerCharacters) {
            // Check validity of moving vs static assets
            CheckStaticVsMovingAssets(staticAssets, movingAssets);
            // Check for validity of player vs non player characters
            CheckCharacters(nonPlayerCharacters, playerCharacters);
            // Add all values to the list
            AddToList(StaticAssets, staticAssets);
            AddToList(MovingAssets, movingAssets);
            // Add all charaters to their respective lists
            AddToList(NonPlayerCharacters, nonPlayerCharacters);
            AddToList(PlayerCharacters, playerCharacters);
            // Add all characters to the All characters list
            AddToList(AllCharacters, PlayerCharacters);
            AddToList(AllCharacters, NonPlayerCharacters);
        }// end AssetContainer constructor

        private void CheckStaticVsMovingAssets(List<IAsset> staticAssets, List<IMovingAsset> movingAssets) {
            // Checking to see if moving assets contains a static asset
            foreach(var movingAsset in movingAssets) {
                if(staticAssets.Contains(movingAsset))
                    throw new ArgumentException("An Asset cannot be both static and dynamic");
            }
        }// end CheckStaticVsMovingAssets()

        private void CheckCharacters(List<ICharacterAsset> npcs, List<ICharacterAsset> players) {
            foreach(var npc in players) {
                if(players.Contains(npc))
                    throw new ArgumentException("A Character cannot be player controlled and an npc");
            }
        }// end CheckCharacters()

        // Add all non duplicate items to a list from another list
        private void AddToList<T>(List<T> mainList, List<T> listToBeAdded)  {
            foreach(T item in listToBeAdded) {
                if(!mainList.Contains(item)) {
                    mainList.Add(item);
                }
            }
        }// end AddToList()

        // AddToMainList needs its own method as the List might not have the same interface type
        private void AddToMainList<T>(List<T> list) where T: IAsset {
            foreach(var asset in list) {
                if(!AllAssets.Contains(asset))
                    AllAssets.Add(asset);
            }
        }// end AddToMainList()

        // sorts assets by Y location (ensures that no object is drawn infront of the other)
        private void SortMainAssetList() {
            AllAssets.Sort((x, y) => x.Location.Y.CompareTo(y.Location.Y));
        }// end SortMainAssetList()

        private void AllAssetListConstructor() {
            // Adds all assets into the assets list then sorts it by the Y location on the map
            AddToMainList(MovingAssets);
            AddToMainList(AllCharacters);
            // Sort Asset list
            SortMainAssetList();
        }// end AllAssetListContructor()

        private void AddAssetToMainList(IAsset asset) {
            AllAssets.Add(asset);
            AllAssets.Sort((x, y) => x.Location.Y.CompareTo(y.Location.Y));
        }// end AddAssetToMainList()

        private void CheckForDuplicateAsset(ICharacterAsset asset) {
            foreach(var currAsset in AllAssets) {
                if(currAsset.Equals(asset))
                    throw new DuplicateWaitObjectException("Asset already exists whithin array");
            }
        }// end CheckForDuplicateAsset()



        public void AddNPC(ICharacterAsset asset) {
            // If object already present in the list throw an error
            CheckForDuplicateAsset(asset);
            // If object not in the array add it to the NPC list
            NonPlayerCharacters.Add(asset);
            // Add Character to the main characters list
            AllCharacters.Add(asset);
            // Add asset to the main container
            AddAssetToMainList(asset);
        }// end AddNPC()

        public void AddPlayerCharacter(ICharacterAsset asset) {
            // If object already present in the list throw an error
            CheckForDuplicateAsset(asset);
            PlayerCharacters.Add(asset);
            AllCharacters.Add(asset);
            AddAssetToMainList(asset);
        }// end AddPlayerCharacter()


    }// end AssetContainer class

    
}// end namespace


