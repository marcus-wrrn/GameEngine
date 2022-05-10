// Classifiers are used to further classify Assets into specific types
// These classifications will allow for more efficent storage through the save/load system
// Additionally Classifiers will be used to determine how an object will be built/treated in the loading process

// Load in from file
// Character code used to call a builder function that will continue to take specific values to build an asset
// New Asset loaded into classifier
// Classifier will then take data from file to further classify the asset
// Using classifications assign asset to a controller 
// Using specific classifications asset will be loaded into container

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Graphics.Assets;

namespace Classifier {


    public enum AssetMovement { STATIC, DYNAMIC };
    public enum CharacterAllegiance { FRIEND, ENEMY, NEUTRAL, NA };
    public enum CharacterType { NPC, PLAYER };

    public interface IAssetClassifier {
        //CharacterAllegiance Allegiance { get; }
        bool IsStatic { get; }
        bool IsSentiant { get; }
        //bool IsPlayerControlled { get; }
    }// end IAssetClassifier interface

    public interface ICharacterClassifier : IAssetClassifier {
        CharacterAllegiance Allegiance { get; }
        bool IsPlayerControlled { get; }
    }// ICharacterClassifier

    public class AssetClassifier : IAssetClassifier {
        // Decides whether an asset can move or not
        // This is important for non sentaint objects which can still be moved
        // A non sentiant static object is like a wall or a pillar, however a non sentiant dynamic object would be like a ball or something movable
        public bool IsStatic { get; private set; }
        // For all standard assets IsSentiant will be false however inhereted classifiers will be able to choose
        public bool IsSentiant { get; private set; }
        public AssetClassifier(bool isStatic) {
            IsStatic = isStatic;
            IsSentiant = false;
        }// end AssetClassifier constructor

        protected AssetClassifier(bool isStatic, bool isSentiant) {
            IsStatic = isStatic;
            IsSentiant = isSentiant;
        }// end protected constructor

    }// end AssetClassifier class

    // Classifier for characters
    public class CharacterClassifier : AssetClassifier, ICharacterClassifier {
        public CharacterAllegiance Allegiance { get; private set; }
        public bool IsPlayerControlled { get; private set; }
        // By making IsSentiant false, the character will effectively be brain dead (i.e the AI will be non functional)
        // This is mostly useful for testing purposes
        // This will also get rid of any stats or health the character may have if sorted into the non sentiant object list
                // Try to avoid this but it's not the end of the world since it should never occur normally
                // All constructors for characters should be designed to feed directly into the characters list unless speciffically told not to
        // For most purposes IsSentiant should always be true
        public CharacterClassifier(bool isStatic, bool isSentiant, bool isPlayerControlled, CharacterAllegiance allegiance) : 
                        base(isStatic, isSentiant) {
            Allegiance = allegiance;
            IsPlayerControlled = isPlayerControlled;
        }// end constructor

        public CharacterClassifier(CharacterClassifier classifier) : base(classifier.IsStatic, classifier.IsSentiant) {
            Allegiance = classifier.Allegiance;
            IsPlayerControlled = classifier.IsPlayerControlled;
        }// end constructor

    }// end CharacterClassifier class

}// end Classifier namespace



