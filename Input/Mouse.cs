using Microsoft.Xna.Framework.Input;
using System;


namespace Input {
    public sealed class GameMouse {
        
        private MouseState _prevMouseState;
        private MouseState _currMouseState;
        private Graphics.Screen _screen;

        public int X { get { return _currMouseState.X; } }
        public int Y { get { return _screen.Height - _currMouseState.Y; } }

        public GameMouse(TestingTactics.Game1 game) {
            _currMouseState = Mouse.GetState();
            _screen  = game.Screen;
            if(_screen == null)
                throw new Exception("WTF");
        }// end constructor

        public void Update() {
            _prevMouseState = _currMouseState;
            _currMouseState = Mouse.GetState();
        }// end Update()

        public bool LeftButtonPressed() {
            return _currMouseState.LeftButton == ButtonState.Pressed;
        }// end LeftButtonPressed()

        public bool RightButtonPressed() {
            return _currMouseState.RightButton == ButtonState.Pressed;
        }// end RightButtonPressed()

        public bool MiddleButtonPressed() {
            return _currMouseState.MiddleButton == ButtonState.Pressed;
        }// end MiddleButtonPressed()

        public bool LeftButtonClicked() {
            return _currMouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed;
        }// end LeftButtonClicked()

        public bool RightButtonClicked() {
            return _currMouseState.RightButton == ButtonState.Released && _prevMouseState.RightButton == ButtonState.Pressed;
        }// end RightButtonClicked()

        public bool MiddleButtonClicked() {
            return _currMouseState.MiddleButton == ButtonState.Released && _prevMouseState.MiddleButton == ButtonState.Pressed;
        }// end MiddleButtonClicked()
        

    }// end GameMouse class

}// end namespace Input