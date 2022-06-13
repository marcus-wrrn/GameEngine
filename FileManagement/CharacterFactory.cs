using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Graphics.Sprites;
using Graphics.Assets;
using System.IO;
using System;


namespace Factory {

    public class CharacterFactory {
        private TestingTactics.Game1 _game;
        private Containers.MasterAssetContainer _masterContainer;


        public CharacterFactory(TestingTactics.Game1 game, Containers.MasterAssetContainer masterContainer) {
            if(game == null)
                throw new NullReferenceException("Game cannot be null");
            if(masterContainer == null)
                throw new NullReferenceException("Master Asset Container cannot be null");
            _game = game;
            _masterContainer = masterContainer;
        }// end CharacterFactory constructor

        // Creates A RockGuy Character and automatically adds it to the Master Asset Container. Due to the way the game environment is set up. No Character can exist not inside the container
        public Containers.RockGuyCharacter CreateRockGuyCharacter(Vector2 location, Classifier.CharacterClassifier classifier, Containers.BaseCharacterStats stats, bool addToMasterAssetList = true) {
            return this.CreateRockGuyCharacter(location, stats.MaxHealth, stats.Health, stats.Speed, stats.HitChance, stats.Evasion, stats.CriticalChance, 
                classifier.Allegiance, classifier.Type, classifier.IsStatic, classifier.IsSentiant, addToMasterAssetList);
        }// end CreateRockGuyCharacter

        public Containers.RockGuyCharacter CreateRockGuyCharacter(Vector2 location, Classifier.CharacterClassifier classifier, bool addToMasterAssetList = true) {
            var status = new Containers.BaseCharacterStats(10, 4, 0.65f, 0.25f, 0.05f);
            return this.CreateRockGuyCharacter(location, classifier, status, addToMasterAssetList);
        }// end CreateRockGuyCharacter()

        public Containers.RockGuyCharacter CreateRockGuyCharacter(Vector2 location, uint maxHealth, uint health, int speed, float hitChance, float evasion, float criticalChance,
                                           Classifier.CharacterAllegiance allegiance, Classifier.AssetType type, bool isStatic, bool isSentiant, bool addToMasterAssetList = true) {
            Containers.BaseCharacterStats stats = new Containers.BaseCharacterStats(maxHealth, health, speed, hitChance, evasion, criticalChance);
            Classifier.CharacterClassifier classifier = new Classifier.CharacterClassifier(allegiance, type, isStatic, isSentiant);
            var rockGuyAsset = BuildRockGuyAsset(location);
            if(!classifier.Type.Equals(Classifier.AssetType.ROCK_GUY))
                throw new ArgumentException("mismatching asset type: " + classifier.Type.ToString());
            var rockGuy = new Containers.RockGuyCharacter(rockGuyAsset, classifier, _masterContainer, stats);
            if(addToMasterAssetList)
                _masterContainer.AddAsset(rockGuy);
            return rockGuy;
        }// end CreateRockGuyCharacter()

        
        public RockGuy BuildRockGuyAsset(Vector2 location, uint health = 3, int initiative = 10, uint numberOfTurns = 2, int acceleration = 10, int maxSpeed = 1000) {
            // Assigns the Path
            string path = "./Characters/";
            var mainSprite = BuildRockGuySprite(path);
            // Build Asset
            HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>> asset = 
                new HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>>(mainSprite, location, maxSpeed, acceleration);
            return new RockGuy(asset);
        }

        private PlayerSprite<RockGuyAnimations> BuildRockGuySprite(string path) {
            // Load Textures
            Texture2D idleSprite =  _game.Content.Load<Texture2D>(path + "BandanGuyStandingAnim");
            Texture2D movingSpriteLeft = _game.Content.Load<Texture2D>(path + "BlueBandanaAnimLeft");
            Texture2D movingSpriteRight = _game.Content.Load<Texture2D>(path + "BlueBandanaAnimRight");
            Texture2D deathSprite = _game.Content.Load<Texture2D>(path + "RockGuyDeathAnim");
            // Build Animated Sprite
            AnimatedSprite idleAnimation = new AnimatedSprite(idleSprite, 6);
            AnimatedSprite movingLeftAnimationU = new AnimatedSprite(movingSpriteLeft, 18);     // U stands for Uncontrolled
            AnimatedSprite movingRightAnimationU = new AnimatedSprite(movingSpriteRight, 18);
            AnimatedSprite deathAnimationU = new AnimatedSprite(deathSprite, 9);
            // Builds Controlled Animated Sprites
            ControlledAnimatedSprite movingLeftAnimationC = new ControlledAnimatedSprite(movingLeftAnimationU, 6, 10);
            ControlledAnimatedSprite movingRightAnimationC = new ControlledAnimatedSprite(movingRightAnimationU, 6, 10);
            // Builds non repeatable animations
            NoRepeatSprite deathAnimationC = new NoRepeatSprite(deathAnimationU);
            // Puts sprites into list
            NoRepeatSprite[] animationList = new NoRepeatSprite[]{ deathAnimationC };
            RockGuyAnimations[] animationNamesList = new RockGuyAnimations[]{ RockGuyAnimations.DEATH };
            // Build SimpleMovingSprite
            SimpleMovingSprite baseSprite = new SimpleMovingSprite(idleAnimation, movingRightAnimationC, movingLeftAnimationC);
            // Builds Main Sprite
            return new PlayerSprite<RockGuyAnimations>(baseSprite, animationList, animationNamesList);
        }// end BuildRockGuySprite()

    }// end CharacterFactory Class

}// end Factory namespace