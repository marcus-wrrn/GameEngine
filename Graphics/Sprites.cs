using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Graphics.Sprites {

    public interface ISprite {
        Texture2D Texture{ get; }
        Rectangle SourceRectangle{ get; }
        Rectangle DestinationRectangle(Vector2 loc);
    }

    public class Sprite : ISprite {
        public virtual Texture2D Texture{ get; }
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
    }// end Sprite


    public interface IAnimatedSprite {
        Texture2D Texture{ get; }
        void Update();
        void ResetSprite();
        Rectangle SourceRectangle{ get; }
        Rectangle DestinationRectangle(Vector2 loc);
        int Rows{ get; set; }
        int Columns{ get; set; }
    }// end interface



    public class AnimatedSprite : Sprite, IAnimatedSprite {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public override Rectangle SourceRectangle{ get { return GetSourceRectangle(); } }

        private int currentFrame;
        private int totalFrames;

        public AnimatedSprite(Texture2D texture, int rows, int columns) : base(texture) {
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
        }// end constructor()

        public virtual void Update() {
            currentFrame++;
            if (currentFrame >= totalFrames)
                currentFrame = 0;
        }// end Update()

        private Rectangle GetSourceRectangle() {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = currentFrame / Columns;
            int column = currentFrame % Columns;
            return new Rectangle(width * column, height * row, width, height);
        }// end GetSourceRectangle()

        public override Rectangle DestinationRectangle(Vector2 location) {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            return new Rectangle((int)location.X, (int)location.Y, width, height);
        }// end DestinationRectangle()

        public void ResetSprite() {
            currentFrame = 0;
        }// end ResetSprite()

        // public void Draw(SpriteBatch spriteBatch, Vector2 location) {
        //     int width = Texture.Width / Columns;
        //     int height = Texture.Height / Rows;
        //     int row = currentFrame / Columns;
        //     int column = currentFrame % Columns;

        //     Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
        //     Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

        //     spriteBatch.Begin();
        //     spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
        //     spriteBatch.End();
        // }// end Draw()

    }// end AnimatedSprite class

}