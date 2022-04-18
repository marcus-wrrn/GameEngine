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
        private Controllers.BackgroundController _backgroundController;
        //private SpriteBatch _spriteBatch;
        private HorizontalMovingAsset<Graphics.Sprites.SimpleMovingSprite> _asset1;
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
            var movingRight = new Graphics.Sprites.ControlledAnimatedSprite(rockGuy, 6, 10);
            rockGuy = new Graphics.Sprites.AnimatedSprite(Content.Load<Texture2D>("./Characters/BlueBandanaAnimLeft"), 1, 18);
            var movingLeft = new Graphics.Sprites.ControlledAnimatedSprite(rockGuy, 6, 10);
            rockGuy = new Graphics.Sprites.AnimatedSprite(Content.Load<Texture2D>("./Characters/BandanGuyStandingAnim"), 1, 6);
            var rockGuy2 = new Graphics.Sprites.AnimatedSprite(Content.Load<Texture2D>("./Characters/BandanGuyStandingAnim"), 1, 6);
            var cool = new Graphics.Sprites.SimpleMovingSprite(rockGuy, movingRight, movingLeft);
            _asset1 = new HorizontalMovingAsset<Graphics.Sprites.SimpleMovingSprite>(cool, new Vector2(1000f, 1000f), 1000f, 10);
            _asset2 = new MovingAsset<Graphics.Sprites.AnimatedSprite>(rockGuy2, new Vector2(1920, 1080), 1000f, 100f);

            // Pretty terrible tile initialization system
            var tiles = new Graphics.Tiles.Tile[40,40];
            for(int i = 0; i < 40; i++) {
                for(int j = 0; j < 40; j++) {
                    tiles[i,j] = new Graphics.Tiles.Tile(Content.Load<Texture2D>("tile"));
                }
            }// end for loop

            var texture = Content.Load<Texture2D>("tile");
            var background = new Background.TileBackground(tiles, Vector2.Zero, Vector2.Zero, texture.Width, texture.Height);

            
            _backgroundController = new Controllers.BackgroundController(this, "TestFile");
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
            if (kBoard.IsKeyClicked(Keys.P))
                _backgroundController.SaveContent("TestFile");
            if (kBoard.IsKeyDown(Keys.W))
                _asset1.MoveUp(gameTime);
            if (kBoard.IsKeyDown(Keys.D))
                _asset1.MoveRight(gameTime);
            if (kBoard.IsKeyDown(Keys.S))
                _asset1.MoveDown(gameTime);
            if (kBoard.IsKeyDown(Keys.A))
                _asset1.MoveLeft(gameTime);



            Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);
            Vector2 p1 = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            Vector2 p2 = Vector2.Zero;
            
            mousePosition.Y = _screen.Height - mousePosition.Y;
            if(temp % 10 == 0) {
                _asset1.AssetSprite.Update();
                _asset2.AssetSprite.Update();
            }
                
            temp++;
            //_asset1.MoveToLocation(mousePosition, gameTime);
            // if(mousePosition == _asset1.LocationOnMap.Location) 
            //     _asset1.AssetSprite.EndAnimation();
            //_asset2.MoveToLocation(mousePosition, gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _screen.Set();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _sprites.Begin(true);
            var backgroundSprites = _backgroundController.Draw();
            _sprites.Draw(_asset2, Color.AliceBlue);
            _sprites.Draw(_asset1, Color.AliceBlue);
            _sprites.End();
            
            _screen.UnSet();
            _screen.Present(backgroundSprites);
            _screen.Present(_sprites);
            base.Draw(gameTime);
        }

        public int GetScreenHeight() {
            return _screen.Height;
        }

        public int GetScreenWidth() {
            return _screen.Width;
        }

    }
}
