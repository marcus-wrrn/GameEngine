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

    public interface IStats {
        uint Health { get; }
        int Speed { get; }
        float HitChance { get; }
        float Evasion { get; }
        float CriticalChance { get; }
    }// end IStats

    public class BaseCharacterStats : IStats {
        public uint Health { get; set; }
        public int Speed { get; set; }
        public float HitChance { get; set; }
        public float Evasion { get; set; }
        public float CriticalChance { get; set; }

        public BaseCharacterStats(uint health, int speed, float hitChance, float evasion, float criticalChance) {
            Health = health;
            Speed = speed;
            HitChance = hitChance;
            Evasion = evasion;
            CriticalChance = criticalChance;
        }// end constructor

    }// end BaseCharacterStats class

    public class RockGuyCharacter : CharacterContainer<RockGuy>, ICharacter {
        // These lists are important to determine where everything is
        private HashSet<ICharacterAssetContainer> _playerCharacters;
        private HashSet<ICharacterAssetContainer> _nonPlayerCharacters;
        private HashSet<ICharacterAssetContainer> _allCharacters;

        // TODO: Make All Stats responsible in the ICharacter interfaces
        // I need to remove dependency from the asset
        public int Health { get; }
        public int Speed { get { return (int)_asset.Speed; } }
        public float HitChance { get; }
        public float CriticalChance { get; }
        public float Evasion { get; }
        public bool IsAlive { get { return _asset.IsAlive; } }
        public IStats CharacterStats { get; private set; }

        public RockGuyCharacter(RockGuy asset, Classifier.CharacterClassifier classifier, MasterAssetContainer masterContainer, BaseCharacterStats stats) : base(asset, classifier) { 
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
            // if(CharacterInfo.IsStatic || CharacterInfo.Allegiance == Classifier.CharacterAllegiance.PLAYER) {
            //     base.Update(gameTime);
            //     UpdateAnimation();
            //     return;
            // }
            base.Update(gameTime);
            //UpdateAnimation();
            // TODO: Make a proper faction system to account for differing allegiances
            // if(CharacterInfo.Allegiance == Classifier.CharacterAllegiance.ENEMY) {
            //     var nearestPlayer = FindNearestPlayer();
            //     double distanceToPlayer = FindDistance(nearestPlayer);
            //     if(distanceToPlayer <= 10) {
            //         // TODO : Create a proper damage system
            //         // Will probably have to flesh out character stats properly to get it to work
            //         TakeDamage(1);
            //     }
            // }
        }// end Update()



    }// end RockGuyContainer class

}// end namespace