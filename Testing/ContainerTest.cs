using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using Containers;
using Graphics.Assets;
using Classifier;

namespace Testing {

    public class CharacterContainerTest {
        public CharacterContainer<RockGuy> TestingContainerEnemyNPC { get; private set; }
        public CharacterContainer<RockGuy> TestingContainerPlayer { get; private set; }
        public CharacterContainer<RockGuy> TestingContainerFriendNPC { get; private set; }
        public CharacterContainer<RockGuy> TestingNonSentiantCharacter { get; private set; }
        public CharacterContainer<RockGuy> TestingStaticCharacter { get; private set; }

        public CharacterContainerTest(TestingTactics.Game1 game) {
            Console.WriteLine("Testing Character initialization");
            Console.WriteLine("===================================");
            Console.WriteLine(" Building Character Containers");
            try {
                // Building all containers
                Console.WriteLine("Building Enemy NPC: ");
                TestingContainerEnemyNPC = LoadCharacterContainer(game, new Vector2(700f, 870f), false, CharacterAllegiance.ENEMY);
                Console.WriteLine("Building Enemy NPC: PASSED");
                Console.WriteLine("===================================");

                Console.WriteLine("Building Player Character: ");
                TestingContainerPlayer = LoadCharacterContainer(game, new Vector2(675f, 750f), true, CharacterAllegiance.FRIEND);
                Console.WriteLine("Building Player Character: PASSED");
                Console.WriteLine("===================================");

                Console.WriteLine("Building Friendly NPC: ");
                TestingContainerFriendNPC = LoadCharacterContainer(game, new Vector2(800f, 120f), false, CharacterAllegiance.FRIEND);
                Console.WriteLine("Building Friendly NPC: PASSED");
                Console.WriteLine("===================================");

                Console.WriteLine("Building Static Character: ");
                TestingStaticCharacter = LoadCharacterContainer(game, new Vector2(600f, 600f), false, CharacterAllegiance.NEUTRAL, true);
                Console.WriteLine("Building Static Character: PASSED");
                Console.WriteLine("===================================");

                Console.WriteLine("Building Non Sentiant Character: ");
                TestingNonSentiantCharacter =LoadCharacterContainer(game, new Vector2(800f, 150f), false, CharacterAllegiance.NEUTRAL, false, false);
                Console.WriteLine("Building Non Sentiant Character: PASSED");
                Console.WriteLine("===================================");

            } catch(Exception exc) {
                Console.Write("Failed");
                Console.WriteLine(exc);
                Console.WriteLine("===================================");
            }
        }// end CharacterContainerTest constructor

        private CharacterContainer<RockGuy> LoadCharacterContainer(TestingTactics.Game1 game, Vector2 location, bool isPlayerControlled, CharacterAllegiance allegiance, bool isStatic = false, bool isSentiant = true) {
            try {  
                var factory = new Factory.CharacterFactory();
                Console.WriteLine("==================================="); 
                var asset = factory.BuildRockGuy(game, location);
                Console.WriteLine("Asset loading: Passed");
                CharacterClassifier classifier = new CharacterClassifier(isStatic, isSentiant, isPlayerControlled, CharacterAllegiance.ENEMY);
                Console.WriteLine("Classifier Loading: Passed");
                CharacterContainer<RockGuy> container = new CharacterContainer<RockGuy>(asset, classifier);
                Console.WriteLine("Container Loaded: Passed");
                Console.WriteLine("===================================");
                return container;
            } catch {
                throw new Exception("Enemy NPC loading failed");
            }
        }// end LoadCharacterContainer()

        public void Test1() {
            
        }


    }// end CharacterContainerTest





    public class MasterContainerTest {
        // All of this is for testing purposes
        public MasterAssetContainer MasterContainer;
        public List<ICharacterAssetContainer>   CharacterContainers;
        public List<IMovingAssetContainer>      MovingAssetContainers;
        public List<IBaseAssetContainer>        StaticAssetContainers;






    }

}// end Testing namespace