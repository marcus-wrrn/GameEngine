using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Graphics.Utility {
    public class TextureLocation {
        public Vector2 Location{ get; private set; }
        public double X{ get {return Location.X;} }
        public double Y{ get{return Location.Y;} }   
        private Vector2 _drawingLocation;
        private readonly Texture2D _texture;     // TODO Change too a spriteCollection later

        public TextureLocation(Texture2D texture, Vector2 location) {
            _drawingLocation = location;
            _texture = texture;
            SetCentreLocation();
        }// end TextureLocation()

        // Sets the location at the centre of its texture
        private void SetCentreLocation() {
            Location = new Vector2(_drawingLocation.X - _texture.Width / 2, _drawingLocation.Y - _texture.Height / 2);
        }// end SetCentreLocation()

        // Changes the location of the Texture
        public void ChangeLocation(Vector2 location) {
            Location = location;
            _drawingLocation = new Vector2(location.X - _texture.Width / 2, location.Y - _texture.Height / 2);
        }// end ChangeLocation()

        public Vector2 GetLocationToDraw() {
            return _drawingLocation;
        }// end GetLocationToDraw()
    }// end TextureLocation class
}// end namespace