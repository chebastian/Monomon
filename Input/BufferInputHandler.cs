using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Input
{
    public enum KeyName
    {
        Up,
        Down,
        Left,
        Right,
        Select,
        Back,
        Quit,
    }

    public interface IINputHandler : IMouseHandler
    {
        int GetX();
        int GetY();
        bool IsKeyPressed(Keys key);
        bool IsKeyPressed(KeyName key);
        bool IsKeyReleased(Keys key);
        bool IsKeyDown(Keys keys);
    }

    public class BufferInputHandler : IINputHandler
    {
        private Keys[] _wasPressed;
        private Keys[] _keys;
        private IEnumerable<Keys> _pressed;
        private IEnumerable<Keys> _removed;
        BufferedMouse _mouse = new BufferedMouse();

        public bool IsKeyPressed(Keys key)
        {
            return _pressed.Contains(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return _removed.Contains(key);
        }

        private Keys[] GetPressedKeys()
        {
            var state = Keyboard.GetState();
            var pad = GamePad.GetState(0);
            var padKeys = new List<Keys>();

            if (state.IsKeyDown(Keys.K))
                padKeys.Add(Keys.Right);
            if (state.IsKeyDown(Keys.J))
                padKeys.Add(Keys.Left);
            if (pad.Buttons.A == ButtonState.Pressed)
                padKeys.Add(Keys.Space);
            if (pad.Buttons.X == ButtonState.Pressed)
                padKeys.Add(Keys.A);
            if (pad.DPad.Left == ButtonState.Pressed)
                padKeys.Add(Keys.Left);
            if (pad.DPad.Right == ButtonState.Pressed)
                padKeys.Add(Keys.Right);
            if (pad.DPad.Up == ButtonState.Pressed)
                padKeys.Add(Keys.Up);
            if (pad.DPad.Down == ButtonState.Pressed)
            {
                padKeys.Add(Keys.Down);
                padKeys.Add(Keys.LeftControl);
            }
            if (pad.Buttons.Start == ButtonState.Pressed)
                padKeys.Add(Keys.L);
            if (pad.Buttons.RightShoulder == ButtonState.Pressed)
                padKeys.Add(Keys.T);

            return state.GetPressedKeys().Union(padKeys).ToArray();
        }

        public void Update()
        {
            _mouse.Update();
            var pressed = GetPressedKeys();
            var newKeys = _wasPressed == null ? pressed : pressed.Except(_wasPressed);
            _keys = pressed;

            var inputstate = GamePad.GetState(0);
            if(inputstate.IsButtonDown(Buttons.A))
            { 
            }

            _pressed = newKeys;

            _removed = _wasPressed == null ? new List<Keys>() : _wasPressed.Except(pressed);

            _wasPressed = pressed;
        }

        public bool IsKeyDown(Keys keys)
        {
            return _keys.Contains(keys);
        }

        public int GetX()
        {
            if (IsKeyDown(Keys.Right))
                return 1;
            else if (IsKeyDown(Keys.Left))
                return -1;

            return 0; 
        }

        public int GetY()
        {
            if (IsKeyDown(Keys.Down))
                return 1;
            else if (IsKeyDown(Keys.Up))
                return -1;

            return 0;
        }

        (BufferedMouseState left, BufferedMouseState right) IMouseHandler.MouseButtonState()
        {
            return _mouse.MouseButtonState();
        }

        public bool IsKeyPressed(KeyName key)
        {
            var kk = key switch
            {
                KeyName.Left => new[]   { Keys.Left },
                KeyName.Right => new[]  { Keys.Right },
                KeyName.Up => new[]     { Keys.Up,Keys.K },
                KeyName.Down => new[]   { Keys.Down,Keys.J },
                KeyName.Select => new[] { Keys.Space },
                KeyName.Back => new[]   { Keys.A },
                KeyName.Quit => new[]   { Keys.C },
                _ => throw new ArgumentOutOfRangeException(),
            };

            return _pressed.Any(pressed => kk.Contains(pressed));
        }
    }
}
