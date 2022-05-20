using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Graphics.Assets;



namespace Containers {



    public interface ICharacter {
        // int Health { get; }
        // int Speed { get; }
        // float HitChance { get; }
        // float Evasion { get; }
        // float CriticalChance { get; }
        IStats CharacterStats { get; }
        bool IsAlive { get; }
        
    }// end ICharacter

    

    public class RockGuyCharacter : CharacterContainer<RockGuy>, ICharacter {
        // These lists are important to determine where everything is
        private HashSet<ICharacterAssetContainer> _playerCharacters;
        private HashSet<ICharacterAssetContainer> _nonPlayerCharacters;
        private HashSet<ICharacterAssetContainer> _allCharacters;
        private int _deathTimer = 0;      
        private readonly static int DISPOSAL_TIME = 100;                             // Death timer is for determining when to properly dispose the asset
        public bool IsAlive { get { return _asset.IsAlive; } }
        

        public RockGuyCharacter(RockGuy asset, Classifier.CharacterClassifier classifier, MasterAssetContainer masterContainer, BaseCharacterStats stats) : base(asset, classifier, stats) { 
            _playerCharacters = masterContainer.PlayerCharacters;
            _nonPlayerCharacters = masterContainer.NonPlayerCharacters;
            _allCharacters = masterContainer.AllCharacters;
        }// end constructor

        private double FindDistance(IBaseAssetContainer container) {
            Vector2 distance = container.Location - Location;
            return Math.Sqrt(distance.X*distance.X + distance.Y*distance.Y);
        }// end FindDistance()

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
            if(IsDisposed)
                throw new ObjectDisposedException("Rockguy is Disposed");
            // if(CharacterInfo.IsStatic || CharacterInfo.Allegiance == Classifier.CharacterAllegiance.PLAYER) {
            //     base.Update(gameTime);
            //     UpdateAnimation();
            //     return;
            // }
            
            base.Update(gameTime);

            if(!IsAlive) {
                //Console.WriteLine("Hello");
                _deathTimer++;
                if(_deathTimer >= DISPOSAL_TIME) {
                    ToBeDisposed = true;
                }
            }
            if(CharacterStats.Health <= 0 && IsAlive) {
                _asset.KillAsset();
            }

        }// end Update()

    }// end RockGuyContainer class

}// end namespace