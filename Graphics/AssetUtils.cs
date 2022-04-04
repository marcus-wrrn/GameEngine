using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Graphics.Utility {
    public class TextureLocation {
        public Vector2 Location{ get; private set; }   
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

    // public class GameVector2 {
    //     private Vector2 _vector;
    //     private Viewport _viewport;
    //     public float X{ get {return _vector.X;} }
    //     public float Y{ get {return _vector.Y;} } 
        
    //     public GameVector2(Vector2 vector2, Viewport viewport) {
    //         _viewport = viewport;
    //         InitializeVector(vector2);
    //     }

    //     public GameVector2(GameVector2 gameVector) {
    //         _vector = gameVector._vector;
    //         _viewport = gameVector._viewport;
    //     }

    //     private void InitializeVector(Vector2 vector2) {
    //         var gameVector = vector2;
    //         gameVector.Y = _viewport.Height - gameVector.Y;
    //         _vector = gameVector;
    //     }// end InitializeVector()

    //     public Vector2 GetVector2() {
    //         var vectorNorm = _vector;
    //         vectorNorm.Y = _viewport.Height + vectorNorm.Y;
    //         return vectorNorm;
    //     }

    //     public bool Equals(GameVector2 other) {
    //         if(_vector.Equals(other._vector) && _viewport.Equals(other._viewport))
    //             return true;
    //         return false;
    //     }

    //     public static GameVector2 operator +(GameVector2 vec1, GameVector2 vec2) {
    //         return new GameVector2(vec1.GetVector2() + vec2.GetVector2(), vec1._viewport);
    //     }

    //     public static GameVector2 operator -(GameVector2 vec1, GameVector2 vec2) {
    //         return new GameVector2(vec1.GetVector2() - vec2.GetVector2(), vec1._viewport);
    //     }

    //     public static GameVector2 operator *(GameVector2 vec1, GameVector2 vec2) {
    //         return new GameVector2(vec1.GetVector2() * vec2.GetVector2(), vec1._viewport);
    //     }

    //     public static GameVector2 operator /(GameVector2 vec1, GameVector2 vec2) {
    //         return new GameVector2(vec1.GetVector2() / vec2.GetVector2(), vec1._viewport);
    //     }
    // }

    
}// end namespace