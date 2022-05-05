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
        public RockGuy BuildRockGuy(TestingTactics.Game1 game, Vector2 location, uint health = 3, int initiative = 10, uint numberOfTurns = 2, int acceleration = 10, int maxSpeed = 1000) {
            // Assigns the Path
            string path = "./Characters/";
            var mainSprite = BuildRockGuySprite(game, path);
            // Build Asset
            HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>> asset = 
                                    new HorizontalMovingAsset<PlayerSprite<RockGuyAnimations>>(mainSprite, location, maxSpeed, acceleration);
            return new RockGuy(asset, health, initiative, numberOfTurns);
        }

        private PlayerSprite<RockGuyAnimations> BuildRockGuySprite(TestingTactics.Game1 game, string path) {
            // Load Textures
            Texture2D idleSprite = game.Content.Load<Texture2D>(path + "BandanGuyStandingAnim");
            Texture2D movingSpriteLeft = game.Content.Load<Texture2D>(path + "BlueBandanaAnimLeft");
            Texture2D movingSpriteRight = game.Content.Load<Texture2D>(path + "BlueBandanaAnimRight");
            Texture2D deathSprite = game.Content.Load<Texture2D>(path + "RockGuyDeathAnim");
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