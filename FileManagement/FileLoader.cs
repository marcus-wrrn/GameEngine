using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Collections.Generic;

namespace FileIO {
    public sealed class FileLoader {
        
    }// end FileLoader class

    public sealed class FileSaver {
        private List<Containers.IBaseAssetContainer> _allAssets;
        private string _fileName;

        public FileSaver(Containers.MasterAssetContainer masterAssetContainer, string fileName) {
            _allAssets = masterAssetContainer.AllAssetContainers;
            _fileName = fileName;
        }// end FileSaver() constructor

        private void SaveAsset(Containers.IBaseAssetContainer baseContainer) {
            var characterContainer = baseContainer as Containers.ICharacterAssetContainer;
            
        }

        private void SaveCharacter(BinaryWriter binWriter, Containers.ICharacterAssetContainer character) {
            var info = character.CharacterInfo;
            var stats = character.CharacterStats;
            // Copying info
            binWriter.Write(info.Type.ToString());
            binWriter.Write(info.Allegiance.ToString());
            binWriter.Write(info.IsPlayerControlled);
            binWriter.Write(info.IsSentiant);
            binWriter.Write(info.IsStatic);
            // Copying Stats
            binWriter.Write(stats.MaxHealth);
            binWriter.Write(stats.Health);
            binWriter.Write(stats.HitChance);
            binWriter.Write(stats.Speed);
            binWriter.Write(stats.Evasion);
            binWriter.Write(stats.CriticalChance);
            // Copying Location
            binWriter.Write(character.Location.X);
            binWriter.Write(character.Location.Y);
        }// end SaveCharacter()

        public void Save() {
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(_fileName, FileMode.Create))) {
                try {
                    foreach(var asset in _allAssets) {

                    }
                } catch (Exception exp) {
                    Console.WriteLine("Saving to File " + _fileName + " Failed: " + exp);
                }
            }
        }

    }// end FileSaver class

}// end FileIO namespace