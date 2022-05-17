using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Collections.Generic;
using TestingTactics;
using Containers;


namespace Controllers {

    public interface ICharacterControl {
        void Update(GameTime gameTime);
    }// end ICharacterControl interface


    // public class NPCTurnController : IController, IDisposable {
    //     private MasterAssetContainer _masterContainer;
    //     private HashSet<ICharacterAssetContainer> _allNPCs;

    //     public NPCTurnController(MasterAssetContainer container) {
    //         _masterContainer = container;
    //         _allNPCs = container.NonPlayerCharacters;
    //     }// end NPCTurnController()



    // }// end NPCTurnController class



    public class PlayerController {
        private Input.GameKeyboard _keyboard = Input.GameKeyboard.Instance;
        private TestingTactics.Game1 _game;
        private ICharacterAssetContainer _currentPlayer;
        private HashSet<ICharacterAssetContainer> _players;
        private HashSet<ICharacterAssetContainer> _npcs;
        private HashSet<ICharacterAssetContainer> _allCharacters;
        public bool TurnEnded{ get; private set; }

        public PlayerController(TestingTactics.Game1 game, Containers.MasterAssetContainer masterContainer) {
            _game = game;
            _players = masterContainer.PlayerCharacters;
            _npcs = masterContainer.NonPlayerCharacters;
            _allCharacters = masterContainer.AllCharacters;
        }// end PlayerController()

        // If a point is found on a character return that character, else return null
        private ICharacterAssetContainer HasClickedCharacter(Vector2 location) {
            foreach(var character in _allCharacters) {
                if(character.DestinationRectangle.Contains(location.X, location.Y))
                    return character;
            }
            return null;
        }// end HasClickedCharacter()

        public void Update(GameTime gameTime) {
            var mouse = Mouse.GetState();
            
        }// end Update()

    }// end PlayerController class

    public class EnemyController {
        private TestingTactics.Game1 _game;
        private HashSet<ICharacterAssetContainer> _players;
        private HashSet<ICharacterAssetContainer> _npcs;
        private HashSet<ICharacterAssetContainer> _allCharacters;
        private HashSet<IBaseAssetContainer> _allAssets;


        public EnemyController(TestingTactics.Game1 game, Containers.MasterAssetContainer masterContainer) {
            _game = game;
            _players = masterContainer.PlayerCharacters;
            _npcs = masterContainer.NonPlayerCharacters;
            _allCharacters = masterContainer.AllCharacters;
        }// end EnemyController constructor



    }

    public class TurnController {
        private enum TurnState { PLAYER, NPC }
        private TestingTactics.Game1 _game;
        private PlayerController _playerControl;
        private EnemyController _enemyControl;
        private TurnState _controllerState;


        public TurnController(TestingTactics.Game1 game, Containers.MasterAssetContainer masterContainer) {
            _game = game;
            _playerControl = new PlayerController(_game, masterContainer);
            _enemyControl = new EnemyController(_game, masterContainer);
            _controllerState = TurnState.PLAYER;
        }// end TurnController()
        
        public void Update(GameTime gameTime) {
            if(_controllerState == TurnState.PLAYER) {
                _playerControl.Update(gameTime);
            }
        }// end Update()


    }// end TurnController class


    public class AssetController : IController, IDisposable {


        // Controller also needs to account for collision
        // Assets trying to move into objects need to have their locations changed to the outer bounds of said object
        // need to work out a physics system
        // Every asset should have a unique controller object for remembering momentum and direction?
        // I'll leave out momentum for now and just focus on getting the asset order done right first


        // All assets are stored in an array, the array will be sorted after every update

        // Assets need to be sorted based off location (y direction)
        // Assets with lower y values need to be drawn first
        private MasterAssetContainer _masterContainer;
        private List<IBaseAssetContainer> _allAssets;
        private HashSet<ICharacterAssetContainer> _allCharacters;


        public bool IsDisposed { get; private set; }

        public AssetController(Game1 game, Containers.MasterAssetContainer masterContainer) {
            _masterContainer = masterContainer;
            _allAssets = _masterContainer.AllAssetContainers;
            _allCharacters = _masterContainer.AllCharacters;

            IsDisposed = false;
        }// end constructor

        public void Dispose() {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
            // Dispose all assets
            foreach (var asset in _masterContainer.AllAssetContainers) {
                asset.Dispose();
            }
            IsDisposed = true;
        }// end Dispose()

        public void Update(GameTime gameTime) {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
            // Update all assets in the container

        }// end Update()

        public void Draw() {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
        }// end Draw() 

        public void SaveContent(string fileName) {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
        }// end SaveContent()

        public void LoadContent(string fileName) {
            if(IsDisposed)
                throw new ObjectDisposedException("Controller has already been disposed");
        }// end LoadContent()
        

    }// end CharacterController class

}// end Controllers namespace