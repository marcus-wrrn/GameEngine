using Microsoft.Xna.Framework;
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
        public int GetRowNumber(float location);
        public int GetColumnNumber(float location);
        public Vector2 GetTileLocation(Vector2 location);
        public void UpdateTile(Tile tile, Vector2 location);
        public void UpdateTile(Tile tile, int row, int column);

    }// end ITileBackground interface

    public sealed class TileBackground : ITileBackground {
        private Tile[,] _map;
        private Vector2[,] _tileLocations;
        public int Rows{ get; private set; }
        public int Columns{ get; private set; }
        public int TileWidth{ get; private set; }
        public int TileHeight{ get; private set; }
        public Rectangle Boundries{ get; private set; }
        private Vector2 _offset;
        public bool IsDisposed{ get; private set; }

        public TileBackground(Tile[,] background, Vector2 startLocation, Vector2 offset, int tileWidth, int tileHeight) {
            _offset = offset;
            InitializeMatrix(background);
            InitializeBounds(startLocation, tileWidth, tileHeight);
            InitializeTileLocations();
            IsDisposed = false;
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
        
        public void Dispose() {
            if(!IsDisposed) {
                for(int i = 0; i < Rows; i++) {
                    for(int j = 0; j < Columns; j++) {
                        _map[i,j].Dispose();
                    }
                }
                IsDisposed = true;
            }
        }// end Dispose

        public int GetColumnNumber(float locationX) {
            return (int)((locationX - _offset.X)/TileHeight);
        }// end GetColumnNumber()

        public int GetRowNumber(float locationY) {
            return (int)((locationY - _offset.Y)/TileWidth);
        }// end getRowNumber()

        public Tile GetTile(int row, int col) {
            if(row >= Rows || row < 0 || col >= Columns || col < 0)
                throw new Exception("Values out of bound\nRow: " + row + "\nCol: " + col);
            return _map[row,col];
        }// end GetTile()

        public Tile GetTile(Vector2 location) {
            return this.GetTile(this.GetRowNumber(location.Y), this.GetColumnNumber(location.X));
        }// end GetTile()

        public Vector2 GetTileLocation(Vector2 location) {
            int column = GetColumnNumber(location.X);
            int row = GetRowNumber(location.Y);
            return new Vector2(column*TileWidth, row*TileHeight);
        }

        public void UpdateTile(Tile tile, Vector2 location) {
            var tileToChange = this.GetTile(location);
            tileToChange.Dispose();
            tileToChange = tile;
        }// end UpdateTile()

        public void UpdateTile(Tile tile, int row, int col) {
            var tileToChange = this.GetTile(row, col);
            tileToChange.Dispose();
            tileToChange = tile;
        }// end UpdateTile()

    }// end TileBackground class

}// end Background namespace