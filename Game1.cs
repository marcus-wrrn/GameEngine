using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Input;
using Graphics.Assets;
using Graphics.Rendering;

namespace TestingTactics
{
    public class Game1 : Game
    {
        int temp = 0;
        GameKeyboard keyboard = GameKeyboard.Instance;
        private GraphicsDeviceManager _graphics;
        //private SpriteBatch _spriteBatch;
        private MovingAsset<Graphics.Sprites.ControlledAnimatedSprite> _asset1;
        private MovingAsset<Graphics.Sprites.AnimatedSprite> _asset2;
        private SpriteBunch _sprites;
        private Graphics.Screen _screen;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // _graphics.PreferredBackBufferWidth = 400;
            // _graphics.PreferredBackBufferHeight = 800;
            // _graphics.ApplyChanges();
            

            base.Initialize();
            
        }

        protected override void LoadContent()
        {
            _sprites = new SpriteBunch(this);
            _screen = new Graphics.Screen(this, 3840, 2160);
            //var sprite = new Graphics.Sprites.AnimatedSprite(Content.Load<Texture2D>("./C"));
            var rockGuy = new Graphics.Sprites.AnimatedSprite(Content.Load<Texture2D>("./Characters/BlueBandanaAnimRight"), 1, 18);
            var tempRockGuy = new Graphics.Sprites.ControlledAnimatedSprite(rockGuy, 6, 10);
            _asset1 = new MovingAsset<Graphics.Sprites.ControlledAnimatedSprite>(tempRockGuy, new Vector2(1000f, 1000f), 1000f, 10);
            _asset2 = new MovingAsset<Graphics.Sprites.AnimatedSprite>(rockGuy, new Vector2(1920, 1080), 1000f, 100f);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            var mouse = Mouse.GetState();
            GameKeyboard kBoard = GameKeyboard.Instance;
            kBoard.Update();
            if (kBoard.IsKeyClicked(Keys.Escape)) {
                Exit();
            }

            Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);
            Vector2 p1 = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            Vector2 p2 = Vector2.Zero;
            
            mousePosition.Y = _screen.Height - mousePosition.Y;
            if(temp % 10 == 0)
                _asset2.AssetSprite.Update();
            temp++;
            //Console.WriteLine("Width: " + _graphics.PreferredBackBufferWidth);
            // Console.WriteLine("Mouse Location: " + mousePosition);
            //_asset2.MoveDown(gameTime);
            _asset2.MoveToLocation(mousePosition, gameTime);
            if(mousePosition == _asset1.LocationOnMap.Location) 
                _asset1.AssetSprite.EndAnimation();
            //_asset2.MoveToLocation(mousePosition, gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _screen.Set();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _sprites.Begin(true);
            _sprites.Draw(_asset2, Color.AliceBlue);
            _sprites.Draw(_asset1, Color.AliceBlue);
            _sprites.End();
            
            _screen.UnSet();
            _screen.Present(_sprites);
            base.Draw(gameTime);
        }
    }
}
