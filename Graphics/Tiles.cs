using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
// The Tiles objects are currently very bland; this is subject to change. ITile will eventually contain things like sound and
// and status effects when the characters pass over them



namespace Graphics.Tiles {
    public interface ITile {
        public Texture2D Texture{ get; }
        
    }

    public class Tile : ITile, IDisposable {
        public Texture2D Texture{ get; private set; }
        private bool _isDisposed;

        public Tile(Texture2D texture) {
            Texture = texture;
            _isDisposed = false;
        }// end constructor

        public void Dispose() {
            if(!_isDisposed) {
                Texture.Dispose();
                _isDisposed = true;
            }
        }// end Dispose()

    }// end Tile
}