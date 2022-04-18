using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using TestingTactics;
using Background;

namespace Controllers {
    public interface IController {
        Graphics.Rendering.SpriteBunch Draw();
        //void Update(GameTime gameTime);
        void LoadContent(string fileName);
        void SaveContent(string fileName);
    }// end IController interface

    public class BackgroundController : IController {
        private Game1 _game;
        private Graphics.Rendering.SpriteBunch _spriteBunch;
        private TileBackground _tiledBackground;

        public BackgroundController(Game1 game, string backgroundFileName) {
            if(game == null)
                throw new NullReferenceException("BackgroundController");
            _game = game;
            _spriteBunch = new Graphics.Rendering.SpriteBunch(_game);
            _tiledBackground = RetrieveBackgroundFromFile(backgroundFileName);
            if(_tiledBackground == null)
                throw new NullReferenceException("File Corrupted");
        }// end BackgroundController

        public BackgroundController(Game1 game, TileBackground background) {
            if(game == null || background == null)
                throw new NullReferenceException("BackgroundController");
            _game = game;
            _spriteBunch = new Graphics.Rendering.SpriteBunch(_game);
            _tiledBackground = background;
        }// end BackgroundController()

        // Reads the 
        private TileBackground RetrieveBackgroundFromFile(string fileName) {
            try {
                BinaryReader binReader = new BinaryReader(new FileStream(fileName, FileMode.Open));
                // Finds the offset
                Vector2 offset = new Vector2();
                offset.X = (float)binReader.ReadDouble();
                offset.Y = (float)binReader.ReadDouble();
                // Finds the start location
                Vector2 startLocation = new Vector2();
                startLocation.X = (float)binReader.ReadInt32();
                startLocation.Y = (float)binReader.ReadInt32();
                // Finds the width and height of each tile
                int tileHeight = binReader.ReadInt32();
                int tileWidth = binReader.ReadInt32();
                Console.WriteLine("Tile Height: " + tileHeight);
                Console.WriteLine("Tile Width: " + tileWidth);
                // Gets the tile information
                int rows = binReader.ReadInt32();
                int columns = binReader.ReadInt32();
                Console.WriteLine("Rows: " + rows);
                Console.WriteLine("Columns: " + columns);
                var tiles = new Graphics.Tiles.Tile[rows,columns];
                // Initializes the tile map
                for(int i = 0; i < rows; i++) {
                    for(int j = 0; j < columns; j++) {
                        tiles[i,j] = new Graphics.Tiles.Tile(_game.Content.Load<Texture2D>(binReader.ReadString()));
                    }
                }
                // Returns the tileBackground
                return new TileBackground(tiles, startLocation, offset, tileWidth, tileHeight);
            } catch (Exception ioexp) {
                Console.WriteLine("Error: {0}", ioexp.Message);
                return null;
            }
        }// end RetrieveBrackgroundFromFile()

        // Saves the tiled Background to a file
        public void SaveContent(string fileName) {
             using (BinaryWriter binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create))) {
                try {
                    // Record the OffSet of the map
                    binWriter.Write((double)_tiledBackground.Offset.X);
                    binWriter.Write((double)_tiledBackground.Offset.Y);
                    // Records the starting location
                    binWriter.Write(_tiledBackground.Boundries.X);
                    binWriter.Write(_tiledBackground.Boundries.Y);
                    // Records the height and width of each tile
                    binWriter.Write(_tiledBackground.TileHeight);
                    binWriter.Write(_tiledBackground.TileWidth);
                    // Records total number of elements in the map
                    binWriter.Write(_tiledBackground.Rows);
                    binWriter.Write(_tiledBackground.Columns);
                    // Saves the names of all the tiles to the file
                    for(int i = 0; i < _tiledBackground.Rows; i++)
                        for(int j = 0; j < _tiledBackground.Columns; j++) {
                            string name = _tiledBackground.GetTile(i, j).Texture.Name;
                            binWriter.Write(name);
                        }
                    binWriter.Close();
                } catch (IOException ioexp) {
                    Console.WriteLine("Error: {0}", ioexp.Message);
                }
            }
        }//  end ExportToBinary()

        public void LoadContent(string fileName) {
            var tempVal = RetrieveBackgroundFromFile(fileName);
            if(tempVal == null)
                throw new Exception("Bad File");
            _tiledBackground = tempVal;
        }// end LoadContent()

        public Graphics.Rendering.SpriteBunch Draw() {
            _spriteBunch.Begin(true);
            for(int i = 0; i < _tiledBackground.Rows; i++) {
                for(int j = 0; j < _tiledBackground.Columns; j++) {
                    _spriteBunch.Draw(_tiledBackground.GetTile(i,j).Texture, _tiledBackground.GetTileLocation(i,j), Color.AliceBlue);
                }
            }
            _spriteBunch.End();
            return _spriteBunch;
        }

    }// end BackgroundController class

}// end Controllers.Background namespace