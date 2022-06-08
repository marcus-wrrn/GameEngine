using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Graphics.Assets;
using Graphics.Rendering;


namespace Containers {
    
    public class MasterAssetContainer {
        // Note while HashSet would be useful for getting rid of duplicates, it's important that I'm able to sort the main list of arrays
        // This is important for rendering objects by their location
        // All other collections of assets will be in Hashsets that will then be fed into the main list 
        public bool IsEmpty { get { return AllAssetContainers.Count == 0; } }
        public List<IBaseAssetContainer>           AllAssetContainers { get; private set; }             // All Assets will be stored in this list (useful for drawing or map wide effect)
        public HashSet<IBaseAssetContainer>        NonActiveObjects { get; private set; }      // Contains all nonsentiant objects (like rocks, trees, pillars....)
        public HashSet<IBaseAssetContainer>        ActiveObjects { get; private set; }         // For objects (not characters) who can affect the environment
        public HashSet<IBaseAssetContainer>        StaticObjects { get; private set; }         // Static objects go here
        public HashSet<IMovingAssetContainer>      MovingObjects { get; private set; }         // Objects that move but don't have a sophistcated AI go here
        public HashSet<ICharacterAssetContainer>   AllCharacters { get; private set; }         // Every Asset with an AI go here 
        public HashSet<ICharacterAssetContainer>   NonPlayerCharacters { get; private set; }   // All NPCs go here
        public HashSet<ICharacterAssetContainer>   PlayerCharacters { get; private set; }      // All Player Characters here

        public MasterAssetContainer() {
            InitializeLists();
        }// end constructor

        // TODO: Further optomizations
        public MasterAssetContainer(List<IBaseAssetContainer> assetContainers) {
            InitializeLists();
            SortContainerList(assetContainers);
        }// end construtor

        private void InitializeLists() {
            // Initialize all lists/hashsets
            AllAssetContainers = new List<IBaseAssetContainer>();
            NonActiveObjects = new HashSet<IBaseAssetContainer>();
            ActiveObjects = new HashSet<IBaseAssetContainer>();
            StaticObjects = new HashSet<IBaseAssetContainer>();
            MovingObjects = new HashSet<IMovingAssetContainer>();
            AllCharacters = new HashSet<ICharacterAssetContainer>();
            NonPlayerCharacters = new HashSet<ICharacterAssetContainer>();
            PlayerCharacters = new HashSet<ICharacterAssetContainer>();
        }// end InitializeLists()

        private void SortContainerList(List<IBaseAssetContainer> assetContainers) {
            // Add each container to their respective hashset
            foreach(var container in assetContainers) {
                // If container does not exist inside the main list add it and sort the value into its respective hashsets
                SortIntoContainers(container);
            }
        }// end SortContainerList

        public void AddAssets(List<IBaseAssetContainer> assetContainers) {
            SortContainerList(assetContainers);
        }// end AssAssets()

        public void AddAsset(IBaseAssetContainer assetContainer) {
            SortIntoContainers(assetContainer);
        }// end AddAsset()

        // Sorts containers into their respective hashsets
        private void SortIntoContainers(IBaseAssetContainer container) {
            if(AllAssetContainers.Contains(container))
                return;
            AllAssetContainers.Add(container);
            // Checks to see which container interface container inherits from
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
            if(!asset.AssetInfo.IsStatic)
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

        public void RemoveDisposedObjects() {
            foreach(var obj in AllAssetContainers) {
                if(obj.IsDisposed)
                    DeleteObject(obj);
            }
        }

        // For Deletion
        public void DeleteObject(IBaseAssetContainer obj) {
            // If the container does not exist in the list exit
            if(!AllAssetContainers.Contains(obj))
                return;
            // Remove container from main list
            AllAssetContainers.Remove(obj);
            // Check if the container is a character
            var characterContainer = obj as ICharacterAssetContainer;
            if(characterContainer != null) {
                DeleteCharacter(characterContainer);
                return;
            }
            // Check if Container is a moving object
            var movingContainer = obj as IMovingAssetContainer;
            if(movingContainer != null) {
                DeleteMoving(movingContainer);
                return;
            }
            // Container must be of a static object
            DeleteStatic(obj);
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

        public void SortAssets() {
            AllAssetContainers.Sort(delegate(IBaseAssetContainer x, IBaseAssetContainer y) { 
                return y.Location.Y.CompareTo(x.Location.Y); 
            });
        }// end SortAssets()

        private void DeleteObjects(List<IBaseAssetContainer> containers) {
            foreach(var container in containers)
                DeleteObject(container);
        }// end DeleteObjects()

        // Disposes of then clears all asset containers from the main container
        public void EmptyContainer() {
            foreach(var obj in AllAssetContainers) {
                obj.Dispose();
            }
            AllAssetContainers.Clear();
            NonActiveObjects.Clear();
            ActiveObjects.Clear();
            StaticObjects.Clear();
            MovingObjects.Clear();
            AllCharacters.Clear();
            NonPlayerCharacters.Clear();
            PlayerCharacters.Clear();
            // Intitializes lists again to avoid null values
            InitializeLists();
        }// end ClearContainer()

        public void Draw(SpriteBunch spriteBunch) {
            foreach(var obj in AllAssetContainers) {
                obj.Draw(spriteBunch);
            }
        }// end Draw()

    }// end AssetContainer class
}