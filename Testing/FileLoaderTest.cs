using FileIO;
using Microsoft.Xna.Framework;
using System;
using System.IO;


namespace Testing {

    public class MasterTest {
        // Yes I know it's basic but it'll probably be improved upon later... probably
        public void Test(string testName) {
            Console.WriteLine("Beggining Test: " + testName);
        }// end Test()
    }// end MasterTest class

    public class FileLoaderTest {
        private TestingTactics.Game1 _game;
        private FileIO.FileSaveLoader _fileLoader;
        private string _fileName1 = "AssetFile";
        private string _fileName2 = "Test2";

        public void Test1() {
            //BinaryReader binaryReader = new BinaryReader(new FileStream(_fileName1, FileMode.Open));
            // for(int i = 1; i <= 20; i++) {
            //     Console.WriteLine(i + ": " + binaryReader.ReadString());
            // }
        }
    }






}// end Testing namespace