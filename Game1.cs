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
        // private MovingAsset<Graphics.Sprites.AnimatedSprite> _asset2;
        private Testing.CharacterContainerTest _characterTest;
        private RockGuy _rockGuy;
        private RockGuy _rockGuy2;
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
            _characterTest = new Testing.CharacterContainerTest(this);
            _characterTest.FindDuplicatesTest1();
            _characterTest.FindDuplicateTest2();
            _characterTest.FindDuplicateTest3();
            _characterTest.FindDuplicateTest4();
            _characterTest.FindDuplicateTest5();
            //var baseTest = new Testing.BaseAssetContainerTest(this);
            //baseTest.TestIllegalStaticAsset();
            var masterTest = new Testing.MasterContainerTest(this);

            base.Initialize();
            
        }
        
        protected override void LoadContent()
        {
            // Doing some Tests
            // Loading Character container tests
            


            _sprites = new SpriteBunch(this);
            _screen = new Graphics.Screen(this, 3840, 2160);
            
            var factory = new Factory.CharacterFactory();
            _rockGuy = factory.BuildRockGuy(this, new Vector2(500, 700));
            _rockGuy2 = factory.BuildRockGuy(this, new Vector2(800, 900));

            // Pretty terrible tile initialization system
            // Going to have make a proper "Map Factory"
            int rows = 40;
            int columns = 60;
            var tiles = new Graphics.Tiles.Tile[rows,columns];
            for(int i = 0; i < rows; i++) {
                for(int j = 0; j < columns; j++) {
                    tiles[i,j] = new Graphics.Tiles.Tile(Content.Load<Texture2D>("./Tiles/TileDarkGreen"));
                }
            }// end for loop

            var texture = Content.Load<Texture2D>("tile");
            var background = new Background.TileBackground(tiles, Vector2.Zero, Vector2.Zero, texture.Width, texture.Height);

            
            _backgroundController = new Controllers.BackgroundController(this, _sprites, "TestFile");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime) {
            // TODO: Add your update logic here
            var mouse = Mouse.GetState();
            GameKeyboard kBoard = GameKeyboard.Instance;
            kBoard.Update();
            if (kBoard.IsKeyClicked(Keys.Escape)) {
                Exit();
            }
            if (kBoard.IsKeyClicked(Keys.P))
                _backgroundController.SaveContent("TestFile");

            Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);
            Vector2 p1 = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            Vector2 p2 = Vector2.Zero;

            if(kBoard.IsKeyClicked(Keys.K))
                _rockGuy.Kill();
            temp++;
            if(kBoard.IsKeyClicked(Keys.J))
                _rockGuy.BringBackFromDead();
            
            // mousePosition.Y = _screen.Height - mousePosition.Y;
            // _rockGuy.MoveToLocation(mousePosition, gameTime);
            bool _isMoving = false;
            if(kBoard.IsKeyDown(Keys.A)) {
                _rockGuy.MoveLeft(gameTime);
                _isMoving = true;
            }
            if(kBoard.IsKeyDown(Keys.D)) {
                _rockGuy.MoveRight(gameTime);
                _isMoving = true;
            }
            if(kBoard.IsKeyDown(Keys.W)) {
                _rockGuy.MoveUp(gameTime);
                _isMoving = true;
            }
            if(kBoard.IsKeyDown(Keys.S)) {
                _rockGuy.MoveDown(gameTime);
                _isMoving = true;

            }
            if(!_isMoving)
                _rockGuy.Stop();
            //_asset1.MoveToLocation(mousePosition, gameTime);
            if(temp % 7 == 0) {
                //_asset1.AssetSprite.Update();
                // _asset2.AssetSprite.Update();
                _rockGuy.Update();
                _rockGuy2.Update();
            }

            
            //_asset1.MoveToLocation(mousePosition, gameTime);
            // if(mousePosition == _asset1.LocationOnMap.Location) 
            //     _asset1.AssetSprite.EndAnimation();
            //_asset2.MoveToLocation(mousePosition, gameTime);
            base.Update(gameTime);
        }// end Update()

        protected override void Draw(GameTime gameTime) {
            _screen.Set();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _sprites.Begin(true);
            _backgroundController.Draw();
            // _sprites.Draw(_asset2, Color.AliceBlue);
            //_sprites.Draw(_asset1, Color.AliceBlue);
            _sprites.Draw(_rockGuy.Texture, _rockGuy.SourceRectangle, _rockGuy.DestinationRectangle, Color.AliceBlue);
            _sprites.Draw(_rockGuy2.Texture, _rockGuy2.SourceRectangle, _rockGuy2.DestinationRectangle, Color.AliceBlue);
            _sprites.End();
            
            _screen.UnSet();
            //_screen.Present(backgroundSprites);
            _screen.Present(_sprites);
            base.Draw(gameTime);
        }

    }
}
