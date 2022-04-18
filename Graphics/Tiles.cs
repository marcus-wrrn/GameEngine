using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
// The Tiles objects are currently very bland; this is subject to change. ITile will eventually contain things like sound and
// and status effects when the characters pass over them

namespace Graphics.Tiles {
    public interface ITile {
        public Texture2D Texture{ get; }
        public bool IsDisposed{ get; }
    }

    public class Tile : ITile, IDisposable {
        public Texture2D Texture{ get; private set; }
        public bool IsDisposed{ get; private set; }

        public Tile(Texture2D texture) {
            Texture = texture;
            IsDisposed = false;
        }// end constructor

        public Tile GetCopy() {
            return new Tile(Texture);
        }


        public void Dispose() {
            if(!IsDisposed) {
                Texture.Dispose();
                IsDisposed = true;
            }
        }// end Dispose()

    }// end Tile

}// end Graphics.Tiles interface