using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using TestingTactics;
using Containers;


namespace Controllers {

    // public class NPCTurnController : IController, IDisposable {
    //     private MasterAssetContainer _masterContainer;
    //     private HashSet<ICharacterAssetContainer> _allNPCs;

    //     public NPCTurnController(MasterAssetContainer container) {
    //         _masterContainer = container;
    //         _allNPCs = container.NonPlayerCharacters;
    //     }// end NPCTurnController()



    // }// end NPCTurnController class


    public class AssetController : IController, IDisposable {

        // Controller also needs to account for collision
        // Assets trying to move into objects need to have their locations changed to the outer bounds of said object
        // need to work out a physics system
        // Every asset should have a unique controller object for remembering momentum and direction?
        // I'll leave out momentum for now and just focus on getting the asset order done right first


        // All assets are stored in an array, the array will be sorted after every update

        // Assets need to be sorted based off location (y direction)
        // Assets with lower y values need to be drawn first
        private MasterAssetContainer _masterContainer;
        private List<IBaseAssetContainer> _allAssets;
        private HashSet<ICharacterAssetContainer> _allCharacters;


        public bool IsDisposed { get; private set; }

        public AssetController(Game1 game, Containers.MasterAssetContainer masterContainer) {
            _masterContainer = masterContainer;
            _allAssets = _masterContainer.AllAssetContainers;
            _allCharacters = _masterContainer.AllCharacters;
            IsDisposed = false;
        }// end constructor

        public void Dispose() {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
            // Dispose all assets
            foreach (var asset in _masterContainer.AllAssetContainers) {
                asset.Dispose();
            }
            IsDisposed = true;
        }// end Dispose()

        public void Update(GameTime gameTime) {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
            // Update all assets in the container

        }// end Update()

        public void Draw() {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
        }// end Draw() 

        public void SaveContent(string fileName) {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
        }// end SaveContent()

        public void LoadContent(string fileName) {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
        }// end LoadContent()
        

    }// end CharacterController class

}// end Controllers namespace