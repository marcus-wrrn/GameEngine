using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Utilities;
namespace Graphics.Sprites {

    ////////////////////////////////////////////////////////////// Base Sprite /////////////////////////////////////////////////////////////
    public interface ISprite : IDisposable {
        Texture2D Texture{ get; }
        Rectangle SourceRectangle{ get; }
        int Width{ get; }
        int Height{ get; }
        Rectangle DestinationRectangle(Vector2 loc);
        void Update();
    }// end ISprite

    // Static Sprite (Just a Texture)
    public class Sprite : ISprite {
        public virtual Texture2D Texture{ get; }
        public virtual int Width{ get { return Texture.Width; } }
        public virtual int Height{ get { return Texture.Height; } }
        public virtual Rectangle SourceRectangle{ get { return GetSourceRectangle(); } }
        public bool IsDisposed{ get; private set; }
        public Sprite(Texture2D texture) {
            Texture = texture;
            IsDisposed = false;
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

        public void Dispose() {
            if(IsDisposed)
                return;
            Texture.Dispose();
            IsDisposed = true;
        }// end Dispose()

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

        public AnimatedSprite(Texture2D texture, int columns) : this(texture, 1, columns) {}

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

    /////////////////////////////////////////////////////////////// NoRepeatSprite //////////////////////////////////////////////////////////////////////////////////

    public interface INoRepeatSprite : IAnimatedSprite, ISprite {
        bool HasStopped{ get; }
    }// end INoRepeatSprite interface

    public class NoRepeatSprite : AnimatedSprite, INoRepeatSprite {
        public bool HasStopped{ get; private set; }

        public NoRepeatSprite(AnimatedSprite sprite) : base(sprite) {
            HasStopped = false;
        }// end NoRepeatSprite constructor

        public override void Update() {  
            if(HasStopped)
                return;
            _currentFrame++;
            if(_currentFrame >= _totalFrames) {
                _currentFrame -= 1;
                HasStopped = true;
            }
        }// end Update()

        public override void ResetSprite() {
            _currentFrame = 0;
            HasStopped = false;
        }// end ResetSprite

    }// end NoRepeatSprite class

    ////////////////////////////////////////////////////////////// Controlled Animated Sprite /////////////////////////////////////////////////////////////////////////

    // Repeating Sprite with a Start, Middle and End
    public interface IControlledAnimatedSprite : IAnimatedSprite, ISprite {
        void EndAnimation();
        bool HasStopped{ get; }
    }// end IControlledAnimatedSprite

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
        public bool IsDisposed{ get; private set; }
        public bool HasEnded{ get { return CheckIfEnded(); } }

        private enum State { IDLE, MOVING_RIGHT, MOVING_LEFT, STOPPING_RIGHT, STOPPING_LEFT };
        private AnimatedSprite _idleSprite;
        private ControlledAnimatedSprite _movingSpriteRight;
        private ControlledAnimatedSprite _movingSpriteLeft;
        private State _state;
        private bool _isMoving;

        public SimpleMovingSprite(AnimatedSprite standingSprite, ControlledAnimatedSprite movingSpriteRight, ControlledAnimatedSprite movingSpriteLeft) {
            _movingSpriteLeft = movingSpriteLeft;
            _movingSpriteRight = movingSpriteRight;
            _idleSprite = standingSprite;
            _state = State.IDLE;
            IsDisposed = false;
        }// end constructor

        public void MoveRight() {
            if(_movingSpriteLeft.HasStopped || _movingSpriteRight.HasStopped) {
                _movingSpriteLeft.ResetSprite();
                _movingSpriteRight.ResetSprite();
            }
            if(!_isMoving)
                _isMoving = true;
            _state = State.MOVING_RIGHT;
        }// end MoveRight()

        public void MoveLeft() {
            if(_movingSpriteLeft.HasStopped || _movingSpriteRight.HasStopped) {
                _movingSpriteLeft.ResetSprite();
                _movingSpriteRight.ResetSprite();
            }
            if(!_isMoving)
                _isMoving = true;
            _state = State.MOVING_LEFT;
        }// end MoveLeft()

        public void Stop() {
            if(_movingSpriteLeft.HasStopped || _movingSpriteRight.HasStopped)
                StopMoving();
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                _state = State.STOPPING_RIGHT;
            else if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                _state = State.STOPPING_LEFT;
            if(_isMoving) {
                _movingSpriteLeft.EndAnimation();
                _movingSpriteRight.EndAnimation();
            }
        }// end Stop()

        public void Update() {
            if(_state == State.IDLE)
                _idleSprite.Update();
            else if(_state == State.STOPPING_RIGHT || _state == State.STOPPING_LEFT) {
                _movingSpriteRight.Update();
                _movingSpriteLeft.Update();
                if(_movingSpriteLeft.HasStopped || _movingSpriteRight.HasStopped)
                    StopMoving();
            }
            else {
                _movingSpriteRight.Update();
                _movingSpriteLeft.Update();
            }
        }// end Update()

        private void StopMoving() {
            _isMoving = false;
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
        }// end GetCurrentTextureWidth()


        private Rectangle GetSourceRectangle() {
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                return _movingSpriteRight.SourceRectangle;
            if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                return _movingSpriteLeft.SourceRectangle;
            return _idleSprite.SourceRectangle;
        }// end GetSourceRectangle()

        public Rectangle DestinationRectangle(Vector2 location) {
            if(_state == State.MOVING_RIGHT || _state == State.STOPPING_RIGHT)
                return _movingSpriteRight.DestinationRectangle(location);
            if(_state == State.MOVING_LEFT || _state == State.STOPPING_LEFT)
                return _movingSpriteLeft.DestinationRectangle(location);
            return _idleSprite.DestinationRectangle(location);
        }// end GetDestinationRectangle()

        public void Dispose() {
            if(IsDisposed)
                return;
            _idleSprite.Dispose();
            _movingSpriteLeft.Dispose();
            _movingSpriteRight.Dispose();
            IsDisposed = true;
        }// end Dispose()

        private bool CheckIfEnded() {
            if(_isMoving) {
                if(_movingSpriteLeft.HasStopped || _movingSpriteRight.HasStopped)
                    return true;
                return false;
            }
            // If the sprite is idle than the animation is considered to have ended
            return true;
        }

    }// end SimpleMovingSprite class

    // ========================================================== Player Sprite ===================================================================================================

    public interface IPlayerSprite : ISimpleMovingSprite {
        void MoveUp();
        void StopMoving();   
    }// end PlayerSprite interface

    public class PlayerSprite<T> : IPlayerSprite where T: Enum {

        public Texture2D            Texture{ get { return GetTexture(); } }
        public Rectangle            SourceRectangle{ get { return GetSourceRectangle(); } }
        public int                  Rows{ get { return GetRows(); } }
        public int                  Columns{ get { return GetColumns(); } }
        public int                  Width{ get { return Texture.Width; } }
        public int                  Height{ get { return Texture.Height; } }
        public bool                 HasEnded{ get; }
        public bool                 IsDisposed{ get; private set; }
        private SimpleMovingSprite  _baseSprite;
        private NoRepeatSprite[]    _animationEvents;
        private T                   _currentState;
        private T[]                 _animationNames;
        private NoRepeatSprite      _currentSprite;
        private bool                _isAnimationPlaying;
        private bool                _isPlayingNonRepeatAnimation;

        public PlayerSprite(SimpleMovingSprite baseSprite, NoRepeatSprite[] animationEvents, T[] animationNames) {
            // Exception checking
            if(animationEvents.Length != animationEvents.GetLength(0))
                throw new RankException("Array must be 1 dimensional");
            if(animationNames.Length != animationNames.GetLength(0))
                throw new RankException("Array must be 1 dimensional");
            if(animationEvents.Length != animationNames.Length)
                throw new Exception("Length of names and events must be the same");
            if(animationEvents.Length == 0)
                throw new Exception("Array cannot have a length of 0");
            if(animationNames.Length == 0)
                throw new Exception("Array cannot have a length of 0");
            if(baseSprite == null)
                throw new Exception("Sprite cannot be null");
            // Assigns values
            _baseSprite = baseSprite;
            _animationEvents = animationEvents;
            _animationNames = animationNames;
            _isAnimationPlaying = false;
            _isPlayingNonRepeatAnimation = false;
        }// end PlayerSprite constructor

        public void Dispose() {
            if(!IsDisposed) {
                _baseSprite.Dispose();
                for(int i = 0; i < _animationEvents.Length; i++)
                    _animationEvents[i].Dispose();
            }
        }// end Dispose()

        private Texture2D GetTexture() {
            if(_isAnimationPlaying)
                return _currentSprite.Texture;
            return _baseSprite.Texture;
        }// end GetTexture()

        private int GetRows() {
            if(_isAnimationPlaying)
                return _currentSprite.Rows;
            return _baseSprite.Rows;
        }// end GetRows()

        private int GetColumns() {
            if(_isAnimationPlaying)
                return _currentSprite.Columns;
            return _baseSprite.Columns;
        }// end GetColumns()

        private Rectangle GetSourceRectangle() {
            if(_isAnimationPlaying)
                return _currentSprite.SourceRectangle;
            return _baseSprite.SourceRectangle;
        }// end GetSourceRectangle()

        public Rectangle DestinationRectangle(Vector2 location) {
            if(_isAnimationPlaying)
                return _currentSprite.DestinationRectangle(location);
            return _baseSprite.DestinationRectangle(location);
        }// end DestinationRectangle()

        private AnimatedSprite FindAnimation(T state) {
            for(int i = 0; i < _animationNames.Length; i++) {
                if(_animationNames[i].Equals(state)) {
                    return _animationEvents[i];
                }
            }
            return null;
        }// end FindAnimation()

        // Sets the animation to play for a specific task
        private void SetNewAnimation(T animation) {
            for(int i = 0; i < _animationNames.Length; i++) {
                if(_animationNames[i].Equals(animation)) {
                    // If another sprite is playing end it
                    if(_isAnimationPlaying)
                        EndAnimation();
                    // Starts new animation
                    StartNewAnimation(i);
                    return;
                }
            }
            // Throws an Error if the animation value does not exist in the sprite
            throw new ArgumentException("Animation is not available for this sprite");
        }// end SetNewAnimation()

        // Plays an animation
        public void PlayAnimation(T animation) {
            SetNewAnimation(animation);
            _isAnimationPlaying = true;
            _isPlayingNonRepeatAnimation = false;
        }// end PlayAnimation()

        // Plays an animation that stops on its final frame
        public void PlayFinalAnimation(T animation) {
            SetNewAnimation(animation);
            _isAnimationPlaying = true;
            _isPlayingNonRepeatAnimation = true;
        }// end PlayFinalAnimation()

        private void EndAnimation() {
            _currentSprite.ResetSprite();
            _isAnimationPlaying = false;
            _isPlayingNonRepeatAnimation = false;
        }// end EndAnimation()

        private void StartNewAnimation(int index) {
            if(!_isAnimationPlaying)
                _baseSprite.ResetSprite();
            _currentSprite = _animationEvents[index];
            _currentState = _animationNames[index];
            _isAnimationPlaying = true;
        }// end StartNewAnimation()

        public void Update() {
            // If an animation is playing
            if(_isAnimationPlaying) {
                if(_currentSprite.HasStopped && !_isPlayingNonRepeatAnimation)
                    EndAnimation();
                else if(!_currentSprite.HasStopped)
                    _currentSprite.Update();
                else
                    return;
            }
            else
                _baseSprite.Update();
        }// end Update()

        public void ResetSprite() {
            if(_isAnimationPlaying)
                EndAnimation();
            _baseSprite.ResetSprite();
        }// end ResetSprite()

        public void Stop() {
            if(_isAnimationPlaying)
                EndAnimation();
            _baseSprite.Stop();
        }// end Stop()

        public void MoveUp() {
            // TODO Implement a move up function
            throw new Exception("This Function is currently unfinished");
        }// end MoveUp()

        public void MoveDown() {
            // TODO Implement a move up function
            throw new Exception("This Function is currently unfinished");
        }// end MoveDown()

        public void MoveRight() {
            if(_isAnimationPlaying)
                return;
            _baseSprite.MoveRight();
        }// end MoveRight()

        public void MoveLeft() {
            if(_isAnimationPlaying)
                return;
            _baseSprite.MoveLeft();
        }// end MoveLeft()

        public void StopMoving() {
            if(_isAnimationPlaying)
                return;
            _baseSprite.Stop();
        }// end StopMoving()

        private bool CheckIfEnded() {
            if(_isAnimationPlaying)
                return _currentSprite.HasStopped;
            return _baseSprite.HasEnded;
        }// CheckIfEnded()

    }// end PlayerSprite class

}// end namespace