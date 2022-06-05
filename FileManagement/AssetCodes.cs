using System;

// This namespace contains the codes for every game object for the purpose of telling the Save/Load System how to properly load each object
// The reason we use this and not Classifier.CharacterType is becuase CharacterType doesn't cover every object and is only to be used for the purpose
// of character interactions not File I/O
namespace Codes {
    public sealed class AssetCodes {
        // Code for every asset/character code goes here
        public string RockGuy { get { return "RockGuy"; } }
        public string RockMan { get { return "RockMan"; } }
        private static readonly Lazy<AssetCodes> Lazy = new Lazy<AssetCodes>(() => new AssetCodes());

        public static AssetCodes Instance { get { return Lazy.Value; } }

        public AssetCodes() {}
    }
}