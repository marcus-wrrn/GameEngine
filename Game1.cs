using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Containers;
using Input;
using Graphics.Assets;
using Graphics.Rendering;
using Classifier;
using Graphics.Sprites;

namespace TestingTactics
{
    public class Game1 : Game
    {
        private const string _assetSaveFile = "AssetFile";
        public GameMouse MouseForGame { get; private set; }
        private GraphicsDeviceManager _graphics;
        private Controllers.BackgroundController _backgroundController;
        // private MovingAsset<Graphics.Sprites.AnimatedSprite> _asset2;
        //private Testing.CharacterContainerTest _characterTest;
        private Controllers.AssetController _controller;
        private Containers.MasterAssetContainer _masterAssetContainer;
        private SpriteBunch _sprites;
        public Graphics.Screen Screen { get; private set; }
        private Factory.CharacterFactory _characterFactory;
        private FileIO.FileSaveLoader _fileLoader;

        public Game1() {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }// end Game1 constructor

        protected override void Initialize() {
            _sprites = new SpriteBunch(this);
            Screen = new Graphics.Screen(this, 3840, 2160);
            // TODO: Add your initialization logic here
            // _graphics.PreferredBackBufferWidth = 400;
            // _graphics.PreferredBackBufferHeight = 800;
            // _graphics.ApplyChanges();
            // _characterTest = new Testing.CharacterContainerTest(this);
            // _characterTest.FindDuplicatesTest1();
            // _characterTest.FindDuplicateTest2();
            // _characterTest.FindDuplicateTest3();
            // _characterTest.FindDuplicateTest4();
            // _characterTest.FindDuplicateTest5();
            //var baseTest = new Testing.BaseAssetContainerTest(this);
            //baseTest.TestIllegalStaticAsset();
            MouseForGame = new GameMouse(this);
            _masterAssetContainer = new MasterAssetContainer();
            _characterFactory = new Factory.CharacterFactory(this, _masterAssetContainer);
            _fileLoader = new FileIO.FileSaveLoader(this, _masterAssetContainer, _assetSaveFile);
            // factory.CreateRockGuyCharacter(this, _masterAssetContainer, new Vector2(300f, 300f), Classifier.CharacterAllegiance.ENEMY);
            // factory.CreateRockGuyCharacter(this, _masterAssetContainer, new Vector2(900f, 600f), Classifier.CharacterAllegiance.ENEMY);
            // factory.CreateRockGuyCharacter(this, _masterAssetContainer, new Vector2(200f, 800f), Classifier.CharacterAllegiance.PLAYER);
            // factory.CreateRockGuyCharacter(this, _masterAssetContainer, new Vector2(600f, 800f), Classifier.CharacterAllegiance.PLAYER);
            //var masterTest = new Testing.MasterContainerTest(this);
            base.Initialize();
            
        }// end Initialize()


        protected override void LoadContent()
        {
            // Doing some Tests
            // Loading Character container tests
            

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
            _controller = new Controllers.AssetController(this, _masterAssetContainer);
            _backgroundController = new Controllers.BackgroundController(this, _sprites, "TestFile");
            CharacterClassifier classifier = new CharacterClassifier(CharacterAllegiance.PLAYER, AssetType.ROCK_GUY);
            _characterFactory.CreateRockGuyCharacter(new Vector2(320f, 400f), classifier);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime) {
            // TODO: Add your update logic here
            MouseForGame.Update();
            GameKeyboard kBoard = GameKeyboard.Instance;
            kBoard.Update();
            if (kBoard.IsKeyClicked(Keys.Escape)) {
                Exit();
            }
            if (kBoard.IsKeyClicked(Keys.P))
                _backgroundController.SaveContent("TestFile");
            if(kBoard.IsKeyClicked(Keys.A)) {
                _fileLoader.SaveObjectsToFile();
            }
            if(kBoard.IsKeyClicked(Keys.L)) {
                _fileLoader.LoadAssetsFromFile(_assetSaveFile);
            }

            // Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);
            // Vector2 p1 = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            // Vector2 p2 = Vector2.Zero;

            // if(kBoard.IsKeyClicked(Keys.K))
            //     _rockGuy.Kill();
            // temp++;
            // if(kBoard.IsKeyClicked(Keys.J))
            //     _rockGuy.BringBackFromDead();
            
            // // mousePosition.Y = _screen.Height - mousePosition.Y;
            // // _rockGuy.MoveToLocation(mousePosition, gameTime);
            // bool _isMoving = false;
            // if(kBoard.IsKeyDown(Keys.A)) {
            //     _rockGuy.MoveLeft(gameTime);
            //     _isMoving = true;
            // }
            // if(kBoard.IsKeyDown(Keys.D)) {
            //     _rockGuy.MoveRight(gameTime);
            //     _isMoving = true;
            // }
            // if(kBoard.IsKeyDown(Keys.W)) {
            //     _rockGuy.MoveUp(gameTime);
            //     _isMoving = true;
            // }
            // if(kBoard.IsKeyDown(Keys.S)) {
            //     _rockGuy.MoveDown(gameTime);
            //     _isMoving = true;

            // }
            // if(!_isMoving)
            //     _rockGuy.Stop();
            // //_asset1.MoveToLocation(mousePosition, gameTime);
            // if(temp % 7 == 0) {
            //     //_asset1.AssetSprite.Update();
            //     // _asset2.AssetSprite.Update();
            //     _rockGuy.Update();
            //     _rockGuy2.Update();
            // }

            _controller.Update(gameTime);

            //_asset1.MoveToLocation(mousePosition, gameTime);
            // if(mousePosition == _asset1.LocationOnMap.Location) 
            //     _asset1.AssetSprite.EndAnimation();
            //_asset2.MoveToLocation(mousePosition, gameTime);
            base.Update(gameTime);
        }// end Update()

        protected override void Draw(GameTime gameTime) {
            Screen.Set();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _sprites.Begin(true);
            _backgroundController.Draw();
            // _sprites.Draw(_asset2, Color.AliceBlue);
            //_sprites.Draw(_asset1, Color.AliceBlue);
            foreach(var asset in _masterAssetContainer.AllAssetContainers) {
                if(!asset.IsDisposed)
                    asset.Draw(_sprites);
            }
            _sprites.End();
            
            Screen.UnSet();
            //_screen.Present(backgroundSprites);
            Screen.Present(_sprites);
            base.Draw(gameTime);
        }

    }
}
