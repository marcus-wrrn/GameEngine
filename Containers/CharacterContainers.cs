using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Graphics.Assets;



namespace Containers {



    public class RockGuyContainer : CharacterContainer<RockGuy> {
        // These lists are important to determine where everything is
        private HashSet<ICharacterAssetContainer> _playerCharacters;
        private HashSet<ICharacterAssetContainer> _nonPlayerCharacters;
        private HashSet<ICharacterAssetContainer> _allCharacters;


        public RockGuyContainer(RockGuy asset, Classifier.CharacterClassifier classifier, MasterAssetContainer masterContainer) : base(asset, classifier) { 
            _playerCharacters = masterContainer.PlayerCharacters;
            _nonPlayerCharacters = masterContainer.NonPlayerCharacters;
            _allCharacters = masterContainer.AllCharacters;
        }// end constructor

        private double FindDistance(IBaseAssetContainer container) {
            Vector2 distance = container.Location - Location;
            return Math.Sqrt(distance.X*distance.X + distance.Y*distance.Y);
        }

        private ICharacterAssetContainer FindNearestPlayer() {
            ICharacterAssetContainer nearestPlayer = null;
            foreach(var player in _playerCharacters) {
                if(nearestPlayer == null)
                    nearestPlayer = player;
                else if(FindDistance(player) >= FindDistance(nearestPlayer))
                    nearestPlayer = player;
            }
            return nearestPlayer;
        }// end FindNearestPlayer()

        public override void Update(GameTime gameTime) {
            if(CharacterInfo.IsStatic || CharacterInfo.Allegiance == Classifier.CharacterAllegiance.PLAYER) {
                base.Update(gameTime);
                return;
            }
            if(CharacterInfo.Allegiance == Classifier.CharacterAllegiance.ENEMY) {
                var nearestPlayer = FindNearestPlayer();
                double distanceToPlayer = FindDistance(nearestPlayer);
                if(distanceToPlayer <= 10) {
                    TakeDamage(1);
                }
            }

        }
        
    }// end RockGuyContainer class

}// end namespace