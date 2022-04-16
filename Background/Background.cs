using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Graphics.Tiles;
using System;

namespace Background {

    public interface ITileBackground : IDisposable {
        public int Rows{ get; }
        public int Columns{ get; }
        public int TileWidth{ get; }
        public int TileHeight{ get; }
        public Rectangle Boundries{ get; }
        public Tile GetTile(Vector2 location);
        public Tile GetTile(int row, int column);
        public int GetRowNumber(Vector2 location);
        public int GetColumnNumber(Vector2 location);
        public void UpdateTile(Tile tile, Vector2 location);
        public void UpdateTile(Tile tile, int row, int column);
        public void ExportToFile(string fileName);
        public void SaveToFile(string fileName);

    }

    public sealed class TileBackground : ITileBackground {
        private Tile[,] _map;
        private Vector2[,] _tileLocations;
        public int Rows{ get; private set; }
        public int Columns{ get; private set; }
        public int TileWidth{ get; private set; }
        public int TileHeight{ get; private set; }
        public Rectangle Boundries{ get; private set; }
        private Vector2 _offset;

        public TileBackground(Tile[,] background, Vector2 startLocation, Vector2 offset, int tileWidth, int tileHeight) {
            _offset = offset;
            InitializeMatrix(background);
            InitializeBounds(startLocation, tileWidth, tileHeight);
            InitializeTileLocations();
        }// end constructor

        private void InitializeMatrix(Tile[,] background) {
            // Initialize Rows and Columns
            Rows = background.GetLength(0);
            Columns = background.GetLength(1);
            // Initialize the map
            _map = background;
        }// end InitializeMatrix()

        // Finds the height and width of each tile and the bounds of the map
        private void InitializeBounds(Vector2 startLocation, int tileWidth, int tileHeight) {
            if(tileWidth <= 0)
                throw new Exception("Width cannot be less than or equal to zero" + tileWidth);
            if(tileHeight <= 0)
                throw new Exception("Height cannot be less than or equal to zero" + tileHeight);

            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Boundries = new Rectangle((int)startLocation.X, (int)startLocation.Y, tileWidth * Columns, tileHeight * Rows);
        }// end InitializeBounds()

        // Initializes the location of each tile
        private void InitializeTileLocations() {
            _tileLocations = new Vector2[Rows, Columns];
            int y = Boundries.Height - TileHeight;
            int x = 0;
            for(int i = 0; i < Rows; i++) {
                for(int j = 0; j < Columns; j++) {
                    _tileLocations[i,j] = new Vector2(x, y);
                    x += TileWidth;
                }
                y -= TileHeight;
                x = 0;
            }
        }// end InitializeTileLocations()



    }
}