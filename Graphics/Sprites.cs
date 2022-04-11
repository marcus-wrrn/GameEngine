using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
namespace Graphics.Sprites {

    public interface ISprite {
        Texture2D Texture{ get; }
        Rectangle SourceRectangle{ get; }
        int Width{ get; }
        int Height{ get; }
        Rectangle DestinationRectangle(Vector2 loc);
        void Update();
    }// end ISprite

    public class Sprite : ISprite {
        public virtual Texture2D Texture{ get; }
        public virtual int Width{ get { return Texture.Width; } }
        public virtual int Height{ get { return Texture.Height; } }
        public virtual Rectangle SourceRectangle{ get { return GetSourceRectangle(); } }
        public Sprite(Texture2D texture) {
            Texture = texture;
        }// end constructor

        public virtual Rectangle DestinationRectangle(Vector2 location) {
            return new Rectangle((int)location.X, (int)location.Y, Texture.Width, Texture.Height);
        }// end DestinationRectangle()

        private Rectangle GetSourceRectangle() {
            return new Rectangle(0, 0, Texture.Width, Texture.Height);
        }// end GetSourceRectangle()
        
        // Empty function, important for the interface, do not delete 
        public virtual void Update() {}
    }// end Sprite


    public interface IAnimatedSprite : ISprite {
        void ResetSprite();
        int Rows{ get; set; }
        int Columns{ get; set; }
    }// end IAnimatedSprite



    public class AnimatedSprite : Sprite, IAnimatedSprite {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public override int Width{ get { return Texture.Width / Columns; } }
        public override int Height{ get { return Texture.Height / Rows; } }

        public override Rectangle SourceRectangle{ get { return GetSourceRectangle(); } }
        private int _currentFrame;
        private int _totalFrames;

        public AnimatedSprite(Texture2D texture, int rows, int columns) : base(texture) {
            Rows = rows;
            Columns = columns;
            _currentFrame = 0;
            _totalFrames = Rows * Columns;
        }// end constructor()

        public override void Update() {
            _currentFrame++;
            if (_currentFrame >= _totalFrames)
                _currentFrame = 0;
        }// end Update()

        private Rectangle GetSourceRectangle() {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = _currentFrame / Columns;
            int column = _currentFrame % Columns;
            return new Rectangle(width * column, height * row, width, height);
        }// end GetSourceRectangle()

        public override Rectangle DestinationRectangle(Vector2 location) {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            return new Rectangle((int)location.X, (int)location.Y, width, height);
        }// end DestinationRectangle()

        public void ResetSprite() {
            _currentFrame = 0;
        }// end ResetSprite()
    }// end AnimatedSprite class

    public interface ISimpleMovingSprite : IAnimatedSprite {
        void MoveRight();
        void MoveLeft();
        void Stop();
    }// end IComplexAnimatedSprite

    // public class SimpleMovingSprite : ISimpleMovingSprite {
        
    // }



}// end namespace