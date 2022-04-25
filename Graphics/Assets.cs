using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Windows;
using Graphics.Sprites;

namespace Graphics.Assets {
    // =============================================================== Base Asset ===============================================================
    public interface IAsset<T> : IDisposable where T: ISprite {
        T AssetSprite{ get; }
        Utility.TextureLocation<T> LocationOnMap{ get; }
        void ChangeSprite(T sprite);
    }

    // Base class used for all Assets
    public class Asset<T> : IAsset<T> where T: ISprite {
        public T AssetSprite{ get; private set; }   // texture of the asset
        public Utility.TextureLocation<T> LocationOnMap{ get; private set; }    // location of the asset
        public bool IsDisposed{ get; private set; }

        public Asset(T sprite, Vector2 loc) {
            AssetSprite = sprite;
            LocationOnMap = new Utility.TextureLocation<T>(sprite, loc);
            IsDisposed = false;
        }// end constructor()

        // Additional Constructors
        public Asset(T text, GraphicsDeviceManager graphics) : this(text, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)) {}
        public Asset(T text) : this(text, new Vector2(0, 0)) {}

        // TODO make it so it depends on Asset class
        public void ChangeSprite(T sprite) {
            AssetSprite = sprite;
        }

        // Immediatly move to a new location
        // TODO Make this depend on the map screen/other objects
        protected void SetLocation(float x, float y) {
            LocationOnMap.ChangeLocation(new Vector2(x, y));
        }// end ChangeLocation()

        /*
        Immediatly move to a new location
        */ 
        protected void SetLocation(Vector2 nextLocation) {
            LocationOnMap.ChangeLocation(nextLocation);
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
            Vector2 location = LocationOnMap.Location;
            location.Y += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            LocationOnMap.ChangeLocation(location);
        }// end MoveUp()

        public virtual void MoveDown(GameTime gameTime) {
            Vector2 location = LocationOnMap.Location;
            location.Y -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(location);
        }// end MoveDown()

        public virtual void MoveRight(GameTime gameTime) {
            Vector2 location = LocationOnMap.Location;
            location.X += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(location);
        }// end MoveRight()

        public virtual void MoveLeft(GameTime gameTime) {
            Vector2 location = LocationOnMap.Location;
            location.X -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(location);
        }// end MoveLeft()

        public virtual void MoveDirection(float x, float y, GameTime gameTime) {
            this.MoveDirection(new Vector2(x, y), gameTime);
        }// end MoveDirection()

        public virtual void MoveDirection(Vector2 nextLocation, GameTime gameTime) {
            Vector2 currLocation = LocationOnMap.Location;
            // Make sure the new location is immutable
            Vector2 tempLocation = nextLocation;
            tempLocation.Normalize();
            // Find new location
            Console.WriteLine("Location 1: " + currLocation);
            currLocation += tempLocation * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(currLocation);
        }// end MoveDirection()

        protected bool HasReachedDestination(Vector2 destination) {
            var location = LocationOnMap.Location;
            Console.WriteLine("Width " + AssetSprite.Width);
            if(location.X > destination.X - AssetSprite.Width / 2 && location.X < destination.X + AssetSprite.Width / 2)
                if(location.Y > destination.Y - AssetSprite.Height / 2 && location.Y < destination.Y + AssetSprite.Width / 2)
                    return true;
            return false;
        }// end HasReachedDestination()

        public virtual void MoveToLocation(Vector2 destination, GameTime gameTime) {
            if(!HasReachedDestination(destination))
                MoveDirection(destination - LocationOnMap.Location, gameTime);
            // Means its close enough to set loc            Console.WriteLine("Also made it here");ation to specific spot
            else {
                SetLocation(destination);
            }
        }// end MoveToLocation()

    }// end MovingAsset class

    public interface IHorizontalMovingAsset<T> : IMovingAsset<T> where T: ISimpleMovingSprite {
        void Stop();
    }

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
            else if (destination.X - LocationOnMap.X > 0)
                AssetSprite.MoveRight();
            else
                AssetSprite.MoveLeft();
        }// end MoveToLocation()

    }// end HorizontleMovingAsset class


}// end namespace