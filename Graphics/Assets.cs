using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Graphics.Sprites;

namespace Graphics.Assets {
    // =============================================================== Base Asset ===============================================================
    public interface IAsset : IDisposable {
        Texture2D Texture{ get; }
        Rectangle SourceRectangle { get; }
        Rectangle DestinationRectangle{ get; }
        Vector2 Location{ get; }
        Vector2 DrawingLocation{ get; }
    }// end IAsset interface

    // Base class used for all Assets
    public class Asset<T> : IAsset where T: ISprite {
        public T AssetSprite{ get; private set; }   // texture of the asset
        protected Utility.TextureLocation<T> _locationOnMap;   // location of the asset
        public bool IsDisposed{ get; private set; }
        public Vector2 Location { get { return _locationOnMap.Location; } }
        public Vector2 DrawingLocation { get { return _locationOnMap.GetLocationToDraw(); } }
        public Rectangle SourceRectangle{ get { return AssetSprite.SourceRectangle; } }
        public Rectangle DestinationRectangle{ get { return AssetSprite.DestinationRectangle(DrawingLocation); } }
        public Texture2D Texture{ get { return AssetSprite.Texture; } }
        

        public Asset(T sprite, Vector2 loc, bool hasCollision = true) {
            AssetSprite = sprite;
            _locationOnMap = new Utility.TextureLocation<T>(sprite, loc);
            IsDisposed = false;
        }// end constructor()

        // Additional Constructors
        public Asset(T text, GraphicsDeviceManager graphics) : this(text, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)) {}
        public Asset(T text) : this(text, new Vector2(0, 0)) {}

        // Immediatly move to a new location
        // TODO Make this depend on the map screen/other objects
        protected void SetLocation(float x, float y) {
            _locationOnMap.ChangeLocation(new Vector2(x, y));
        }// end ChangeLocation()

        /*
        Immediatly move to a new location
        */ 
        protected void SetLocation(Vector2 nextLocation) {
            _locationOnMap.ChangeLocation(nextLocation);
        }// end ChangeLocation()

        public void Dispose() {
            if(IsDisposed)
                return;
            AssetSprite.Dispose();
            IsDisposed = true;
        }// end Dispose()

    }// end Asset

    // ======================================================== Moving Asset ===============================================================
    public interface IMovingAsset<T> where T: ISprite {
        float Speed{ get; }
        void ChangeSpeed(float speed);
        void MoveUp(GameTime gameTime);
        void MoveDown(GameTime gameTime);
        void MoveLeft(GameTime gameTime);
        void MoveRight(GameTime gameTime);
        void MoveDirection(Vector2 vector, GameTime gameTime);
        void MoveDirection(float x, float y, GameTime gameTime);
        void MoveToLocation(Vector2 vector, GameTime gameTime);
    }

    // An Asset that moves a specific direction
    public class MovingAsset<T> : Asset<T>, IMovingAsset<T> where T: ISprite {
        public float Speed{ get; private set; }

        public MovingAsset(T sprite, Vector2 location, float maxSpeed, float acceleration) : base(sprite, location) {
            Speed = maxSpeed;
        }// end MovingAsset()

        public MovingAsset(T sprite, GraphicsDeviceManager graphicsManager, float maxSpeed) : base(sprite, graphicsManager) {
            Speed = maxSpeed;
        }// end MovingAsset()

        public MovingAsset(T sprite, Vector2 location) : base(sprite, location) {
            Speed = 0.0f;
        }// end MovingAsset()

        public MovingAsset(T sprite) : base(sprite) {
            Speed = 0.0f;
        }// end MovingAsset()

        public void ChangeSpeed(float speed) {
            Speed = speed;
        }// end ChangeSpeed()


        public void MoveUp(GameTime gameTime) {
            // Change speed
            Vector2 location = Location;
            location.Y += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _locationOnMap.ChangeLocation(location);
        }// end MoveUp()

        public virtual void MoveDown(GameTime gameTime) {
            Vector2 location = Location;
            location.Y -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(Location);
        }// end MoveDown()

        public virtual void MoveRight(GameTime gameTime) {
            Vector2 location = Location;
            location.X += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(location);
        }// end MoveRight()

        public virtual void MoveLeft(GameTime gameTime) {
            Vector2 location = Location;
            location.X -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(location);
        }// end MoveLeft()

        public virtual void MoveDirection(float x, float y, GameTime gameTime) {
            this.MoveDirection(new Vector2(x, y), gameTime);
        }// end MoveDirection()

        public virtual void MoveDirection(Vector2 nextLocation, GameTime gameTime) {
            Vector2 currLocation = Location;
            // Make sure the new location is immutable
            Vector2 tempLocation = nextLocation;
            tempLocation.Normalize();
            // Find new location
            currLocation += tempLocation * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(currLocation);
        }// end MoveDirection()

        protected bool HasReachedDestination(Vector2 destination) {
            var location = Location;
            if(location.X > destination.X - AssetSprite.Width / 2 && location.X < destination.X + AssetSprite.Width / 2)
                if(location.Y > destination.Y - AssetSprite.Height / 2 && location.Y < destination.Y + AssetSprite.Width / 2)
                    return true;
            return false;
        }// end HasReachedDestination()

        public virtual void MoveToLocation(Vector2 destination, GameTime gameTime) {
            if(!HasReachedDestination(destination))
                MoveDirection(destination - Location, gameTime);
            // Means its close enough to set location to specific spot
            else {
                SetLocation(destination);
            }
        }// end MoveToLocation()

    }// end MovingAsset class

    public interface IHorizontalMovingAsset<T> : IMovingAsset<T> where T: ISimpleMovingSprite {
        void Stop();
    }// end IHorizontal

    public class HorizontalMovingAsset<T> : MovingAsset<T>, IHorizontalMovingAsset<T> where T: ISimpleMovingSprite {
        // (T sprite, Vector2 location, float maxSpeed, float acceleration) : base(sprite, location)
        public HorizontalMovingAsset(T sprite, Vector2 location, float maxSpeed, float acceleration) 
            : base (sprite, location, maxSpeed, acceleration) {}

        public override void MoveRight(GameTime gameTime) {
            base.MoveRight(gameTime);
            AssetSprite.MoveRight();
        }

        public override void MoveLeft(GameTime gameTime) {
            base.MoveLeft(gameTime);
            AssetSprite.MoveLeft();
        }

        public void Stop() {
            AssetSprite.Stop();
        }// end Stop()

        public override void MoveToLocation(Vector2 destination, GameTime gameTime) {
            base.MoveToLocation(destination, gameTime);
            if(HasReachedDestination(destination)) {
                AssetSprite.Stop();
            }
            else if (destination.X - _locationOnMap.X > 0)
                AssetSprite.MoveRight();
            else
                AssetSprite.MoveLeft();
        }// end MoveToLocation()

    }// end HorizontleMovingAsset class

}// end namespace