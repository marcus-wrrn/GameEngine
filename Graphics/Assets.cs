using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Windows;

namespace Graphics.Assets {
    // =============================================================== Base Asset ===============================================================
    public interface IAsset {
        Texture2D Texture{ get; }
        Utility.TextureLocation LocationOnMap{ get; }
        void ChangeTexture(Texture2D texture);
    }

    // Base class used for all Assets
    public class Asset : IAsset {
        public Texture2D Texture{ get; private set; }   // texture of the asset
        public Utility.TextureLocation LocationOnMap{ get; private set; }    // location of the asset

        public Asset(Texture2D text, Vector2 loc) {
            Texture = text;
            LocationOnMap = new Utility.TextureLocation(text, loc);
        }// end constructor()

        // Additional Constructors
        public Asset(Texture2D text, GraphicsDeviceManager graphics) : this(text, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)) {}
        public Asset(Texture2D text) : this(text, new Vector2(0, 0)) {}

        // TODO make it so it depends on Asset class
        public virtual void ChangeTexture(Texture2D texture) {
            Texture = texture;
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
    }// end Asset

    // ======================================================== Moving Asset ===============================================================
    public interface IMovingAsset {
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
    public class MovingAsset : Asset, IMovingAsset {
        public float Speed{ get; private set; }

        public MovingAsset(Texture2D texture, Vector2 location, float maxSpeed, float acceleration) : base(texture, location) {
            Speed = maxSpeed;
        }

        public MovingAsset(Texture2D texture, GraphicsDeviceManager graphicsManager, float maxSpeed) : base(texture, graphicsManager) {
            Speed = maxSpeed;
        }

        public MovingAsset(Texture2D texture, Vector2 location) : base(texture, location) {
            Speed = 0.0f;
        }

        public MovingAsset(Texture2D texture) : base(texture) {
            Speed = 0.0f;
        }

        public void ChangeSpeed(float speed) {
            Speed = speed;
        }


        public void MoveUp(GameTime gameTime) {
            // Change speed
            Vector2 location = LocationOnMap.Location;
            location.Y += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            LocationOnMap.ChangeLocation(location);
        }// end MoveUp()

        public void MoveDown(GameTime gameTime) {
            Vector2 location = LocationOnMap.Location;
            location.Y -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(location);
        }// end MoveDown()

        public void MoveRight(GameTime gameTime) {
            Vector2 location = LocationOnMap.Location;
            location.X += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(location);
        }// end MoveRight()

        public void MoveLeft(GameTime gameTime) {
            Vector2 location = LocationOnMap.Location;
            location.X -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(location);
        }// end MoveLeft()

        public void MoveDirection(float x, float y, GameTime gameTime) {
            this.MoveDirection(new Vector2(x, y), gameTime);
        }

        public void MoveDirection(Vector2 nextLocation, GameTime gameTime) {
            Vector2 currLocation = LocationOnMap.Location;
            // Make sure the new location is immutable
            Vector2 tempLocation = nextLocation;
            tempLocation.Normalize();
            // Find new location
            currLocation += tempLocation * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.SetLocation(currLocation);
        }// end MoveDirection()

        private bool HasReachedDestination(Vector2 destination) {
            var location = LocationOnMap.Location;
            if(location.X > destination.X - Texture.Width / 2 && location.X < destination.X + Texture.Width / 2)
                if(location.Y > destination.Y - Texture.Height / 2 && location.Y < destination.Y + Texture.Width / 2)
                    return true;
            return false;
        }

        public void MoveToLocation(Vector2 destination, GameTime gameTime) {
            if(!HasReachedDestination(destination))
                MoveDirection(destination - LocationOnMap.Location, gameTime);
            // Means its close enough to set location to specific spot
            else {
                SetLocation(destination);
            }
        }
    }
}