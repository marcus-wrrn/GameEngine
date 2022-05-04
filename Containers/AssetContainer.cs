using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Graphics.Assets;

namespace Containers {
    public class AssetContainer {
        public List<IAsset>             AllAssets{ get; private set; }
        public List<IMovingAsset>       MovingObjects { get; private set; }
        public List<ICharacterAsset>    Enemies { get; private set; }
        public RockGuy                  Player { get; private set; }


    }// end AssetContainer class
}


