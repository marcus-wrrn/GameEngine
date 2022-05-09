using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using Containers;
using Graphics.Assets;
using Classifier;
using Graphics.Sprites;

// This is a unit test for the purpose of testing the containerization of different assets
// As the MasterContainer + All minor containers compromise the storage of all assets I need to be sure that it works perfectly
// And has no bugs. This means that everything needs to be sorted properly and is able to be updated quickly


namespace Testing {


    public class CharacterContainerTest {
        private TestingTactics.Game1 _game;
        public CharacterContainer<RockGuy> TestingContainerEnemyNPC { get; private set; }
        public CharacterContainer<RockGuy> TestingContainerPlayer { get; private set; }
        public CharacterContainer<RockGuy> TestingContainerFriendNPC { get; private set; }
        public CharacterContainer<RockGuy> TestingNonSentiantCharacter { get; private set; }
        public CharacterContainer<RockGuy> TestingStaticCharacter { get; private set; }

        public CharacterContainerTest(TestingTactics.Game1 game) {
            _game = game;
            Console.WriteLine("Testing Character initialization");
            Console.WriteLine("===================================");
            Console.WriteLine(" Building Character Containers");
            try {
                // Building all containers
                Console.WriteLine("Building Enemy NPC: ");
                TestingContainerEnemyNPC = LoadCharacterContainer(game, new Vector2(700f, 870f), false, CharacterAllegiance.ENEMY);
                Console.WriteLine("Building Enemy NPC: PASSED");
                Console.WriteLine("");

                Console.WriteLine("Building Player Character: ");
                TestingContainerPlayer = LoadCharacterContainer(game, new Vector2(675f, 750f), true, CharacterAllegiance.FRIEND);
                Console.WriteLine("Building Player Character: PASSED");
                Console.WriteLine("");

                Console.WriteLine("Building Friendly NPC: ");
                TestingContainerFriendNPC = LoadCharacterContainer(game, new Vector2(800f, 120f), false, CharacterAllegiance.FRIEND);
                Console.WriteLine("Building Friendly NPC: PASSED");
                Console.WriteLine("");

                Console.WriteLine("Building Static Character: ");
                TestingStaticCharacter = LoadCharacterContainer(game, new Vector2(600f, 600f), false, CharacterAllegiance.NEUTRAL, true);
                Console.WriteLine("Building Static Character: PASSED");
                Console.WriteLine("");

                Console.WriteLine("Building Non Sentiant Character: ");
                TestingNonSentiantCharacter =LoadCharacterContainer(game, new Vector2(800f, 150f), false, CharacterAllegiance.NEUTRAL, false, false);
                Console.WriteLine("Building Non Sentiant Character: PASSED");
                Console.WriteLine("");

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

        public List<ICharacterAssetContainer> ContainerListTest() {
            // Loading containers into a single list
            Console.WriteLine("Inserting all Characters into a List");
            List<ICharacterAssetContainer> list = new List<ICharacterAssetContainer>();
            list.Add(TestingContainerEnemyNPC);
            list.Add(TestingContainerFriendNPC);
            list.Add(TestingContainerPlayer);
            list.Add(TestingNonSentiantCharacter);
            list.Add(TestingStaticCharacter);
            Console.WriteLine("All Characters inserted");
            return list;
        }// end ContainerListTest()

        public void FindDuplicatesTest1() {
            Console.WriteLine("Making similar item of TestingContainerEnemyNPC");
            var factory = new Factory.CharacterFactory();
            var assetTest = factory.BuildRockGuy(_game, TestingContainerEnemyNPC.Location);
            var classifierTest = new CharacterClassifier(TestingContainerEnemyNPC.AssetInfo.IsStatic, TestingContainerEnemyNPC.AssetInfo.IsSentiant, TestingContainerEnemyNPC.CharacterInfo.IsPlayerControlled, TestingContainerEnemyNPC.CharacterInfo.Allegiance);
            var testContainer = new CharacterContainer<RockGuy>(assetTest, classifierTest);
            var list = ContainerListTest();
            if(list.Contains(testContainer))
                Console.WriteLine("Test1: FAILED");
            else
                Console.WriteLine("Test1: PASSED");
        }// end FindDuplicatesTest1()

        private CharacterContainer<RockGuy> CreateDuplicateContainer(ICharacterAssetContainer container) {
            // copy Asset
            var factory = new Factory.CharacterFactory();
            var asset = factory.BuildRockGuy(_game, container.Location, container.CharacterHealth, container.CharacterInitiative, container.CharacterNumberOfTurns);
            var classifier = new CharacterClassifier(container.CharacterInfo);
            return new CharacterContainer<RockGuy>(asset, classifier);
        }// end CreateDuplicateContainer()

        private bool TestForDuplicate(CharacterContainer<RockGuy> container, List<ICharacterAssetContainer> list) {
            return list.Contains(container);
        }// end TestForDuplicate()

        public void FindDuplicateTest2() {
            Console.WriteLine("\n=================================================");
            Console.WriteLine("Starting Test2: Testing for all duplicates");
            var list = ContainerListTest();
            CharacterContainer<RockGuy> testCharacter;
            try {
                for(int i = 0; i < list.Count; i++) {
                    Console.Write("Testing if duplicate caught by character " + i + ": ");
                    testCharacter = CreateDuplicateContainer(list[i]);
                    if(TestForDuplicate(testCharacter, list))
                        Console.Write("FAILED\n");
                    else
                        Console.Write("PASSED\n");
                }
            } catch {
                Console.WriteLine("Test2: FAILED");
            }
            Console.WriteLine("Test2: PASSED");
        }// end FindDuplicateTest2()

        public void FindDuplicateTest3() {
            Console.WriteLine("\n=================================================");
            Console.WriteLine("Starting Test3: Testing for duplicate asset but not duplicate classifier");
            var factory = new Factory.CharacterFactory();
            var asset = factory.BuildRockGuy(_game, Vector2.Zero);
            CharacterClassifier classifier1 = new CharacterClassifier(false, true, false, CharacterAllegiance.FRIEND);
            var character1 = new CharacterContainer<RockGuy>(asset, classifier1);
            // Creating second classifier
            CharacterClassifier classifier2 = new CharacterClassifier(classifier1);
            CharacterContainer<RockGuy> character2 = new CharacterContainer<RockGuy>(asset, classifier2);
            // Add character one to a list
            var list = ContainerListTest();
            list.Add(character1);
            if(list.Contains(character2))
                Console.WriteLine("Test 3: FAILED");
            else
                Console.WriteLine("Test 3: PASSED");
        }// end FindDuplicateTest3

        public void FindDuplicateTest4() {
            Console.WriteLine("\n=================================================");
            Console.WriteLine("Starting Test4: Testing for the same container added to a list");
            var list = ContainerListTest();
            if(list.Contains(TestingContainerFriendNPC))
                Console.WriteLine("Test 4: PASSED");
            else
                Console.WriteLine("Test 5: FAILED");
        }// end FindDuplicateTest4()

        private CharacterContainer<RockGuy> CreateTestCharacter() {
            var factory = new Factory.CharacterFactory();
            var asset = factory.BuildRockGuy(_game, Vector2.Zero);
            CharacterClassifier classifier = new CharacterClassifier(false, true, false, CharacterAllegiance.FRIEND);
            return new CharacterContainer<RockGuy>(asset, classifier);
        }// end CreateTestCharacter

        public void FindDuplicateTest5() {
            Console.WriteLine("\n=================================================");
            Console.WriteLine("Starting Test5: Testing for the same container added to a list after modification outside of list");
            var list = ContainerListTest();
            var characterTest = CreateTestCharacter();
            var character2 = CreateTestCharacter();
            list.Add(characterTest);
            characterTest.MoveAssetToLocation(new Vector2(800f, 900f));
            if(list.Contains(characterTest))
                Console.WriteLine("Test 5: PASSED");
            else
                Console.WriteLine("Test 5: FAILED");
        }// end FindDuplicateTest5



    }// end CharacterContainerTest





    public class MasterContainerTest {
        // All of this is for testing purposes
        private TestingTactics.Game1 _game;
        public MasterAssetContainer MasterContainer;
        public List<ICharacterAssetContainer>   CharacterContainers;
        public List<IMovingAssetContainer>      MovingAssetContainers;
        public List<IBaseAssetContainer>        StaticAssetContainers;

        public MasterContainerTest(TestingTactics.Game1 game) {
            _game = game;
            // Create List of Static Assets
            // Create a List of Moving Asset Containers
            // Create a List of Static Containers
        }

        private Asset<Sprite> GenerateBaseAsset() {
            Sprite sprite = new Sprite(_game.Content.Load<Texture2D>("Ball"));
            return new Asset<Sprite>(sprite, Vector2.Zero);
        }// end GenerateBaseSprite()

        private AssetContainer<Asset<Sprite>> GetBaseContainer(bool isStatic) {
            Asset<Sprite> asset = GenerateBaseAsset();
            AssetClassifier classifier = new AssetClassifier(isStatic);
            return new AssetContainer<Asset<Sprite>>(asset, classifier);
        }

        private void GenerateBaseStaticAssetList() {
            // different types of static objects

        }// end GenerateStaticAssetList()




    }

}// end Testing namespace