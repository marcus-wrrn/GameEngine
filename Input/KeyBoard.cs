using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Input {
    public sealed class GameKeyboard {
        private static readonly Lazy<GameKeyboard> Lazy = new Lazy<GameKeyboard>(() => new GameKeyboard());

        public static GameKeyboard Instance {
            get { return Lazy.Value; }
        }

        private KeyboardState prevKeyboardState;
        private KeyboardState currKeyboardState;

        public GameKeyboard() {
            prevKeyboardState = Keyboard.GetState();
            currKeyboardState = prevKeyboardState;
        }

        public void Update() {
            this.prevKeyboardState = this.currKeyboardState;
            this.currKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key) {
            return this.currKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyClicked(Keys key) {
            return this.currKeyboardState.IsKeyDown(key) && !this.prevKeyboardState.IsKeyDown(key);
        }
    }
}