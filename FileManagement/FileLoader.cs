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

        private void SaveObject(BinaryWriter binWriter, Containers.IBaseAssetContainer baseContainer) {
            var characterContainer = baseContainer as Containers.ICharacterAssetContainer;
            if(characterContainer != null) {
                SaveCharacter(binWriter, characterContainer);
            }
            else
                SaveAsset(binWriter, baseContainer);
        }

        private void SaveAsset(BinaryWriter binWriter, Containers.IBaseAssetContainer baseContainer) {
            if(baseContainer.IsDisposed || baseContainer.ToBeDisposed)
                return;
            var info = baseContainer.AssetInfo;
            // Copy Info
            binWriter.Write(info.Type.ToString());
            binWriter.Write(info.IsSentiant);
            binWriter.Write(info.IsStatic);
            // Copying Location
            binWriter.Write(baseContainer.Location.X);
            binWriter.Write(baseContainer.Location.Y);
        }// end SaveAsset


        private void SaveCharacterInfo(BinaryWriter binWriter, Containers.ICharacterAssetContainer character) {
            var info = character.CharacterInfo;
            // Record Character type
            binWriter.Write(info.Type.ToString());
            // Record Character Allegiance
            binWriter.Write(info.Allegiance.ToString());
            // Record Player Info
            binWriter.Write(info.IsPlayerControlled);
            binWriter.Write(info.IsSentiant);
            binWriter.Write(info.IsStatic);
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

        private void SaveCharacter(BinaryWriter binWriter, Containers.ICharacterAssetContainer character) {
            var info = character.CharacterInfo;
            var stats = character.CharacterStats;
            // Copying info
            
            // Copying Stats
            
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