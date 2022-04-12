using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
namespace Graphics.Sprites {

    ////////////////////////////////////////////////////////////// Base Sprite /////////////////////////////////////////////////////////////
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

    ////////////////////////////////////////////////////////////// Animated Sprite /////////////////////////////////////////////////////////////

    public interface IAnimatedSprite : ISprite {
        void ResetSprite();
        int Rows{ get; }
        int Columns{ get; }
    }// end IAnimatedSprite



    public class AnimatedSprite : Sprite, IAnimatedSprite {
        public int Rows { get; }
        public int Columns { get; }
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
            int row = _currentFrame / Columns;
            int column = _currentFrame % Columns;
            return new Rectangle(Width * column, Height * row, Width, Height);
        }// end GetSourceRectangle()

        public override Rectangle DestinationRectangle(Vector2 location) {
            return new Rectangle((int)location.X, (int)location.Y, Width, Height);
        }// end DestinationRectangle()

        public virtual void ResetSprite() {
            _currentFrame = 0;
        }// end ResetSprite()

    }// end AnimatedSprite class

    ////////////////////////////////////////////////////////////// Controlled Animated Sprite /////////////////////////////////////////////////////////////

    public interface IControlledAnimatedSprite : IAnimatedSprite, ISprite {
        void EndAnimation();
        bool HasStopped{ get; }
    }

    public class ControlledAnimatedSprite : AnimatedSprite, IControlledAnimatedSprite {
        public bool HasStopped{ get { return _state == State.ENDED; } }
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

    ////////////////////////////////////////////////////////////// Simple Moving Sprite /////////////////////////////////////////////////////////////

    public interface ISimpleMovingSprite : IAnimatedSprite {
        void MoveRight();
        void MoveLeft();
        void Stop();
    }// end IComplexAnimatedSprite

    public class SimpleMovingSprite : ISimpleMovingSprite {
        public int Width{ get { return GetCurrentTextureWidth(); } }
        public int Height{ get { return GetCurrentTextureHeight(); } }
        public int Rows{ get { return GetCurrentRowNumber(); } }
        public int Columns{ get { return GetCurrentColNumber(); } }
        public Texture2D Texture{ get { return GetCurrentTexture(); } }
        public Rectangle SourceRectangle{ get { return GetSourceRectangle(); } }

        private enum State { IDLE, MOVING_RIGHT, MOVING_LEFT, STOPPING_RIGHT, STOPPING_LEFT };
        private AnimatedSprite _idleSprite;
        private ControlledAnimatedSprite _movingSpriteRight;
        private ControlledAnimatedSprite _movingSpriteLeft;
        private State _state;

        public SimpleMovingSprite(AnimatedSprite standingSprite, ControlledAnimatedSprite movingSpriteRight, ControlledAnimatedSprite movingSpriteLeft) {
            _movingSpriteLeft = movingSpriteLeft;
            _movingSpriteRight = movingSpriteRight;
            _idleSprite = standingSprite;
            _state = State.IDLE;
        }// end constructor

        public void MoveRight() {
            _state = State.MOVING_RIGHT;
        }// end MoveRight()

        public void MoveLeft() {
            _state = State.MOVING_LEFT;
        }// end MoveLeft()

        public void Stop() {
            if(_movingSpriteLeft.HasStopped || _movingSpriteRight.HasStopped)
                return;
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                _state = State.STOPPING_RIGHT;
            else if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                _state = State.STOPPING_LEFT;
            _movingSpriteLeft.EndAnimation();
            _movingSpriteRight.EndAnimation();
        }// end Stop()

        public void Update() {
            if(_state == State.IDLE)
                _idleSprite.Update();
            else if(_state == State.STOPPING_RIGHT || _state == State.STOPPING_LEFT) {
                if(_movingSpriteLeft.HasStopped || _movingSpriteRight.HasStopped)
                    StopMoving();
            }
            else {
                _movingSpriteRight.Update();
                _movingSpriteLeft.Update();
            }
        }// end Update()

        private void StopMoving() {
            _movingSpriteLeft.ResetSprite();
            _movingSpriteRight.ResetSprite();
            _idleSprite.ResetSprite();
            _state = State.IDLE;
        }// end HasStopped

        public void ResetSprite() {
            // Set state to idle
            _state = State.IDLE;
            // Reset all animation states
            _idleSprite.ResetSprite();
            _movingSpriteRight.ResetSprite();
            _movingSpriteLeft.ResetSprite();
        }// end ResetSprite()

        private int GetCurrentRowNumber() {
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                return _movingSpriteRight.Rows;
            if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                return _movingSpriteLeft.Rows;
            return _idleSprite.Rows;
        }// end GetCurrentRowNumber()

        private int GetCurrentColNumber() {
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                return _movingSpriteRight.Columns;
            if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                return _movingSpriteLeft.Columns;
            return _idleSprite.Columns;
        }// end GetCurrentColNumber()

        private Texture2D GetCurrentTexture() {
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                return _movingSpriteRight.Texture;
            if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                return _movingSpriteLeft.Texture;
            return _idleSprite.Texture;
        }// end GetCurrentTexture()

        private int GetCurrentTextureHeight() {
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                return _movingSpriteRight.Height;
            if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                return _movingSpriteLeft.Height;
            return _idleSprite.Height;
        }// end GetCurrentTextureHeight()

        private int GetCurrentTextureWidth() {
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                return _movingSpriteRight.Width;
            if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                return _movingSpriteLeft.Width;
            return _idleSprite.Width;
        }


        private Rectangle GetSourceRectangle() {
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                return _movingSpriteRight.SourceRectangle;
            if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                return _movingSpriteLeft.SourceRectangle;
            return _idleSprite.SourceRectangle;
        }

        public Rectangle DestinationRectangle(Vector2 location) {
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                return _movingSpriteRight.DestinationRectangle(location);
            if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                return _movingSpriteLeft.DestinationRectangle(location);
            return _idleSprite.DestinationRectangle(location);
        }// end GetDestinationRectangle()

    }// end SimpleMovingSprite class

}// end namespace