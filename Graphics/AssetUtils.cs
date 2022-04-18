using Microsoft.Xna.Framework;

namespace Graphics.Utility {
    public class TextureLocation<T> where T: Graphics.Sprites.ISprite {
        public Vector2 Location{ get; private set; }
        public double X{ get {return Location.X;} }
        public double Y{ get{return Location.Y;} }   
        private Vector2 _drawingLocation;
        private readonly T _sprite;     // TODO Change too a spriteCollection later

        public TextureLocation(T sprite, Vector2 location) {
            _drawingLocation = location;
            _sprite = sprite;
            SetCentreLocation();
        }// end TextureLocation()

        // Sets the location at the centre of its texture
        private void SetCentreLocation() {
            var destinationRectangle = _sprite.DestinationRectangle(_drawingLocation);
            Location = destinationRectangle.Center.ToVector2();
        }// end SetCentreLocation()

        // Changes the location of the Texture
        public void ChangeLocation(Vector2 location) {
            Location = location;
            _drawingLocation = new Vector2(location.X - _sprite.Width / 2, location.Y - _sprite.Height / 2);
        }// end ChangeLocation()

        public Vector2 GetLocationToDraw() {
            return _drawingLocation;
        }// end GetLocationToDraw()
    }// end TextureLocation class
}// end namespace