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
        public virtual void Update() {
            var text = Texture;
        }
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
        protected int _currentFrame;
        protected int _totalFrames;

        public AnimatedSprite(Texture2D texture, int rows, int columns) : base(texture) {
            Rows = rows;
            Columns = columns;
            _currentFrame = 0;
            _totalFrames = Rows * Columns;
        }// end constructor()

        public AnimatedSprite(AnimatedSprite other) : base(other.Texture) {
            Rows = other.Rows;
            Columns = other.Columns;
            _currentFrame = 0;
            _totalFrames = Rows * Columns;
        }// end copy constructor

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

        public virtual void ResetSprite() {
            _currentFrame = 0;
        }// end ResetSprite()
    }// end AnimatedSprite class

    public interface IControlledAnimatedSprite : IAnimatedSprite, ISprite {
        void EndAnimation();
    }

    public class ControlledAnimatedSprite : AnimatedSprite, IControlledAnimatedSprite {
        private enum State{ START, MIDDLE, ENDING, ENDED }
        private int _lastStartFrame;
        private int _lastRepeatFrame;
        private State _state;


        public ControlledAnimatedSprite(Texture2D texture, int rows, int cols, int lastStartFrame, int lastRepeatFrame) : base(texture, rows, cols) {
            if(lastStartFrame < 0 || lastStartFrame > _totalFrames)
                throw new ArgumentException("Start frame cannot be less than zero or greater than the total number of frames");
            if(lastStartFrame >= lastRepeatFrame)
                throw new ArgumentException("the last start frame cannot be after the last repeat frame");
            if(lastRepeatFrame > _totalFrames)
                throw new ArgumentException("the last repeat frame cannot be greater than the total number of frames");
            _lastStartFrame = lastStartFrame;
            _lastRepeatFrame = lastRepeatFrame;
        }
        public ControlledAnimatedSprite(AnimatedSprite sprite, int lastStartFrame, int lastRepeatFrame) : 
            this(sprite.Texture, sprite.Rows, sprite.Columns, lastStartFrame, lastRepeatFrame) {}



        public override void Update() {
            if(_state == State.START) {
                UpdateBegining();
            }
            else if(_state == State.MIDDLE) {
                UpdateMiddle();
            } else if (_state == State.ENDING) {
                UpdateEnd();
            }
        }// end Update()

        private void UpdateBegining() {
            _currentFrame++;
            if(_currentFrame >= _lastStartFrame)
                _state = State.MIDDLE;
        }// end Update Begining

        private void UpdateMiddle() {
            _currentFrame++;
            if(_currentFrame >= _lastRepeatFrame)
                _currentFrame = _lastStartFrame + 1;
        }// end UpdateMiddle()

        private void UpdateEnd() {
            _currentFrame++;
            if(_currentFrame >= _totalFrames) {
                _currentFrame = _totalFrames - 1;
                _state = State.ENDED;
            }
            else if(_currentFrame <= _lastRepeatFrame) {
                _currentFrame = _lastRepeatFrame + 1;
            }
        }// end UpdateEnd()

        // Triggers the end state of the animation
        public void EndAnimation() {
            _state = State.ENDING;
        }// end EndAnimation()

        public override void ResetSprite() {
            _currentFrame = 0;
            _state = State.START;
        }// end ResetSprite()
    }





    public interface ISimpleMovingSprite : IAnimatedSprite {
        void MoveRight();
        void MoveLeft();
        void Stop();
    }// end IComplexAnimatedSprite

    // public class SimpleMovingSprite : ISimpleMovingSprite {
    //     private enum Direction { STANDING, MOVING_RIGHT, MOVING_LEFT, STOPPING_RIGHT, STOPPING_LEFT };
    //     private AnimatedSprite _movingSpriteLeft;
    //     private AnimatedSprite _movingSpriteRight;
    //     private AnimatedSprite _standingSprite;
    //     private Direction _direction;
    //     private bool _isStoping;

    //     public SimpleMovingSprite(AnimatedSprite standingSprite, AnimatedSprite movingSpriteRight, AnimatedSprite movingSpriteLeft) {
    //         _movingSpriteLeft = movingSpriteLeft;
    //         _movingSpriteRight = movingSpriteRight;
    //         _standingSprite = standingSprite;
    //         _direction = Direction.STANDING;
    //         _isStoping = false;
    //     }// end constructor

    //     public void MoveRight() {
    //         _direction = Direction.MOVING_RIGHT;
    //     }// end MoveRight()

    //     public void MoveLeft() {
    //         _direction = Direction.MOVING_LEFT;
    //     }// end MoveLeft()

    //     public void Stop() {
    //         if(_direction == Direction.MOVING_RIGHT || _direction == Direction.STOPPING_RIGHT)
    //             _direction = Direction.STOPPING_RIGHT;
    //         else if(_direction == Direction.MOVING_LEFT || _direction == Direction.STOPPING_LEFT)
    //             _direction = Direction.STOPPING_LEFT;
    //     }// end Stop()



    // }

}// end namespace