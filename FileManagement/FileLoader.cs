using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Collections.Generic;

namespace FileIO {

    public sealed class FileSaveLoader {
        // ID's From organization of assets in the binary file
        // Asset ID's are to tell the File Loader Which methods to use to load a specific asset
        private const string CHARACTER_ID = "CHARACTER";
        private const string MOVING_ASSET_ID = "MOVING";
        private const string BASE_ASSET_ID = "BASE";
        private const string END_MESSAGE = "END_FILE";
        // List of all character values that 
        private Classifier.AssetType[] _baseCharacters = new Classifier.AssetType[] { Classifier.AssetType.ROCK_GUY, Classifier.AssetType.ROCK_MAN };
        private Factory.CharacterFactory _assetFactory;
        private Containers.MasterAssetContainer _masterContainer;        

        private string _fileName;

        public FileSaveLoader(TestingTactics.Game1 game, Containers.MasterAssetContainer masterAssetContainer, string fileName) {
            if(CHARACTER_ID == MOVING_ASSET_ID || CHARACTER_ID == BASE_ASSET_ID || MOVING_ASSET_ID == BASE_ASSET_ID)
                throw new ArgumentException("Asset ID's cannot be equal to each other");
            if(END_MESSAGE == CHARACTER_ID || END_MESSAGE == MOVING_ASSET_ID || END_MESSAGE == BASE_ASSET_ID)
                throw new ArgumentException("End message cannot be the same as any asset ID's");
            _masterContainer = masterAssetContainer;
            _fileName = fileName;
            _assetFactory = new Factory.CharacterFactory(game, _masterContainer);
        }// end FileSaver() constructor

        // Useful Algorithms ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private Classifier.AssetType FindAssetType(string type) {
            foreach(Classifier.AssetType enumType in Enum.GetValues(typeof(Classifier.AssetType))) {
                if(type == enumType.ToString())
                    return enumType;
            }
            return Classifier.AssetType.NOT_AVAILABLE;
        }// end FindAssetType()

        private Classifier.CharacterAllegiance FindCharacterAllegiance(string allegiance) {
            foreach(Classifier.CharacterAllegiance enumAllegiance in Enum.GetValues(typeof(Classifier.CharacterAllegiance))) {
                if(allegiance == enumAllegiance.ToString())
                    return enumAllegiance;
            }
            return Classifier.CharacterAllegiance.NOT_AVAILABLE;
        }// end FindCharacterAllegiance()


        // For Saving to a File ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SaveObjectsToFile() {
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(_fileName, FileMode.Create))) {
                try {
                    foreach(var asset in _masterContainer.AllAssetContainers) {
                        SaveObject(binWriter, asset);
                    }
                    binWriter.Write(END_MESSAGE);
                } catch (Exception exp) {
                    Console.WriteLine("Saving to File " + _fileName + " Failed: " + exp);
                }
            }
        }// end Save()

        // Determines what interface the object inherits from and determines which method to save object
        private void SaveObject(BinaryWriter binWriter, Containers.IBaseAssetContainer baseContainer) {
            binWriter.Write(CHARACTER_ID);
            var characterContainer = baseContainer as Containers.ICharacterAssetContainer;
            if(characterContainer != null) {
                SaveCharacter(binWriter, characterContainer);
            }
            var movingAssetContainer = baseContainer as Containers.IMovingAssetContainer;
            if(movingAssetContainer != null) {
                SaveMovingAsset(binWriter, movingAssetContainer);
            }
            else
                SaveBaseAsset(binWriter, baseContainer);
        }// end SaveObject()

        private void SaveAssetInfo(BinaryWriter binWriter, Containers.IBaseAssetContainer container) {
            var info = container.AssetInfo;
            binWriter.Write(info.Type.ToString());
            binWriter.Write(info.IsSentiant);
            binWriter.Write(info.IsStatic);
        }// end SaveAssetInfo()

        private void SaveBaseAsset(BinaryWriter binWriter, Containers.IBaseAssetContainer baseContainer) {
            if(baseContainer.IsDisposed || baseContainer.ToBeDisposed)
                return;
            binWriter.Write(BASE_ASSET_ID);
            // Copy Info
            SaveAssetInfo(binWriter, baseContainer);
            // Copying Location
            binWriter.Write(baseContainer.Location.X);
            binWriter.Write(baseContainer.Location.Y);
        }// end SaveAsset

        private void SaveMovingAsset(BinaryWriter binWriter, Containers.IMovingAssetContainer movingContainer) {
            binWriter.Write(MOVING_ASSET_ID);
            SaveAssetInfo(binWriter, movingContainer);
            SaveMovingAssetLocation(binWriter, movingContainer);
        }// end SaveMovingAsset()


        private void SaveCharacterInfo(BinaryWriter binWriter, Containers.ICharacterAssetContainer character) {
            var info = character.CharacterInfo;
            // Record standard asset info
            SaveAssetInfo(binWriter, character);
            // Record Character specific info
            binWriter.Write(info.Allegiance.ToString());
        }// end SaveCharacterInfo()

        private void SaveCharacterStats(BinaryWriter binWriter, Containers.ICharacterAssetContainer character) {
            var stats = character.CharacterStats;
            // Record Health
            binWriter.Write(stats.MaxHealth);
            binWriter.Write(stats.Health);
            // Record Speed and Evasion
            binWriter.Write(stats.Speed);
            binWriter.Write(stats.Evasion);
            // Record Hit/Crit chance
            binWriter.Write(stats.HitChance);
            binWriter.Write(stats.CriticalChance);
        }// end SaveCharacterStats

        private void SaveMovingAssetLocation(BinaryWriter binWriter, Containers.IMovingAssetContainer movingContainer) {
            binWriter.Write(movingContainer.Location.X);
            binWriter.Write(movingContainer.Location.Y);
            binWriter.Write(movingContainer.IsMoving);
            if(movingContainer.IsMoving) {
                binWriter.Write(movingContainer.LocationMovingTo.X);
                binWriter.Write(movingContainer.LocationMovingTo.Y);
            }
        }// end SaveMovingAssetLocation()

        private void SaveCharacter(BinaryWriter binWriter, Containers.ICharacterAssetContainer character) {
            binWriter.Write(CHARACTER_ID);
            // Copying info
            SaveCharacterInfo(binWriter, character);
            // Copying Stats
            SaveCharacterStats(binWriter, character);
            // Save Location/Location asset is moving to
            SaveMovingAssetLocation(binWriter, character);
        }// end SaveCharacter()

        // For Loading to a file \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\        

        private Classifier.CharacterClassifier LoadCharacterInfo(BinaryReader binReader) {
            Classifier.AssetType type = FindAssetType(binReader.ReadString());
            if(type == Classifier.AssetType.NOT_AVAILABLE)
                throw new Exception("Asset Type did not return proper result");
            bool isSentiant = binReader.ReadBoolean();
            bool isStatic = binReader.ReadBoolean();
            Classifier.CharacterAllegiance allegiance = FindCharacterAllegiance(binReader.ReadString());
            if(allegiance == Classifier.CharacterAllegiance.NOT_AVAILABLE)
                throw new Exception("Character Allegiance did not return proper result");
            return new Classifier.CharacterClassifier(allegiance, type, isStatic, isSentiant);
        }// end LoadCharacterInfo()

        public Containers.BaseCharacterStats LoadCharacterStats(BinaryReader binReader) {
            uint maxHealth = binReader.ReadUInt32();
            uint health = binReader.ReadUInt32();
            int speed = binReader.ReadInt32();
            float evasion = binReader.ReadSingle();
            float hitChance = binReader.ReadSingle();
            float critChance = binReader.ReadSingle();
            return new Containers.BaseCharacterStats(maxHealth, health, speed, evasion, hitChance, critChance);
        }// end LoadCharacterStats()

        private Vector2 LoadLocation(BinaryReader binReader) {
            float x = binReader.ReadSingle();
            float y = binReader.ReadSingle();
            return new Vector2(x, y);
        }// end LoadLocation()

        private Containers.ICharacterAssetContainer BuildCharacter(Classifier.CharacterClassifier classifier, Containers.BaseCharacterStats stats, Vector2 location) {
            switch (classifier.Type) {
                case (Classifier.AssetType.ROCK_GUY) :
                    var character = _assetFactory.CreateRockGuyCharacter(location, classifier, stats);
                    return character;
                case (Classifier.AssetType.STANDARD_ASSET):
                    return _assetFactory.CreateRockGuyCharacter(location, classifier, stats);
                default:
                    return _assetFactory.CreateRockGuyCharacter(location, classifier, stats);
            }
        }// end BuildCharacter()

        public void LoadCharacterAssetFromFile(BinaryReader binReader) {
            var info = LoadCharacterInfo(binReader);
            var stats = LoadCharacterStats(binReader);
            Vector2 Location = LoadLocation(binReader);
            var character = BuildCharacter(info, stats, Location);
            // Checks if character is moving
            if(binReader.ReadBoolean()) {
                // If Character is moving, move character to the proper location
                 Vector2 _locationToMove = LoadLocation(binReader);
                 character.MoveAssetToLocation(_locationToMove);
            }
        }// end LoadCharacterAssetFromFile()

        private Classifier.AssetClassifier LoadAssetInfo(BinaryReader binReader) {
            Classifier.AssetType type = FindAssetType(binReader.ReadString());
            bool isSentiant = binReader.ReadBoolean();
            bool isStatic = binReader.ReadBoolean();
            return new Classifier.AssetClassifier(isStatic, isSentiant, type);
        }// end LoadAssetInfo()

        // private void SaveMovingAssetLocation(BinaryWriter binWriter, Containers.IMovingAssetContainer movingContainer) {
        //     binWriter.Write(movingContainer.Location.X);
        //     binWriter.Write(movingContainer.Location.Y);
        //     binWriter.Write(movingContainer.IsMoving);
        //     if(movingContainer.IsMoving) {
        //         binWriter.Write(movingContainer.LocationMovingTo.X);
        //         binWriter.Write(movingContainer.LocationMovingTo.Y);
        //     }
        // }// end SaveMovingAssetLocation()

        private void LoadMovingAssetFromFile(BinaryReader binReader) {
            Classifier.AssetClassifier assetInfo = LoadAssetInfo(binReader);
            Vector2 location = LoadLocation(binReader);
            // TODO add an example moving object
            if(binReader.ReadBoolean()) {
                Vector2 movingToLocation = LoadLocation(binReader);
            }

        }// end LoadMovingAssetFromFile()

        // Function Responsible for loading all assets from a file into a master container
        public void LoadAssetsFromFile(string fileName) {
            // Empties container of all current assets
            _masterContainer.EmptyContainer();
            try {
                BinaryReader binReader = new BinaryReader(new FileStream(fileName, FileMode.Open));
                List<Containers.IBaseAssetContainer> assetsFromFile = new List<Containers.IBaseAssetContainer>();
                while(true) {
                    string id = binReader.ReadString();
                    switch (id)
                    {
                        case END_MESSAGE:
                            binReader.Close();
                            break;
                        case CHARACTER_ID:
                            LoadCharacterAssetFromFile(binReader);
                            break;
                        case MOVING_ASSET_ID:
                            break;
                        case BASE_ASSET_ID:
                            break;
                        default:
                            throw new Exception("Error loading asset file. Expected ID got: " + id);
                    }
                }
            } catch (Exception exp) {
                Console.WriteLine("Loading from File " + fileName + " Failed: " + exp);
            }
        }// end LoadAssetsFromFile()

    }// end FileSaver class

}// end FileIO namespace