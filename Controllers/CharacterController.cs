using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using TestingTactics;
using Graphics.Assets;


namespace Controllers {
    public class AssetController : IController, IDisposable {

        // Controller also needs to account for collision
        // Assets trying to move into objects need to have their locations changed to the outer bounds of said object
        // need to work out a physics system
        // Every asset should have a unique controller object for remembering momentum and direction?
        // I'll leave out momentum for now and just focus on getting the asset order done right first


        // All assets are stored in an array, the array will be sorted after every update

        // Assets need to be sorted based off location (y direction)
        // Assets with lower y values need to be drawn first
        private List<IAsset> _allAssets;
        private List<IAsset> _staticAssets;
        private List<IMovingAsset> _movingAssets;
        private List<ICharacterAsset> _characterAssets;

        public bool IsDisposed { get; private set; }

        public AssetController(Game1 game) {
            var factory = new Graphics.Assets.CharacterFactory();
            // var rockGuy = factory.BuildRockGuy(game, Vector2.Zero);
            // var rockGuy2 = factory.BuildRockGuy(game, new Vector2(300f, 200f));
            // _assets.Add(rockGuy);
            // _assets.Add(rockGuy2);
            IsDisposed = false;
        }// end constructor

        public void Dispose() {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
            // Dispose all assets
            foreach (var asset in _allAssets) {
                asset.Dispose();
            }
            IsDisposed = true;
        }// end Dispose()

        public void Update() {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
            foreach (var asset in _characterAssets) {
                asset.Update();
            }
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