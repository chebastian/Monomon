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
    using MonoGameBase.Input;
    using MonoGameProj.Input;

    public class BufferInputHandler : IINputHandler
    {
        private Keys[] _wasPressed;
        private Keys[] _keys;
        private GamePadState _padState;
        private List<KeyName> _padDown;
        private IEnumerable<Keys> _pressed;
        private IEnumerable<Keys> _removed;
        BufferedMouse _mouse = new BufferedMouse();
        private IEnumerable<KeyName> _padWasPressed;
        private IEnumerable<KeyName> _padPressed;
        private float _cursorX;
        private float _cursorY;
        private bool _mouseMove;

        private Keys[] GetPressedKeys()
        {
            var state = Keyboard.GetState();
            var pad = GamePad.GetState(0);
            var padKeys = new List<Keys>();

            return state.GetPressedKeys().Union(padKeys).ToArray();
        }

        public void Update()
        {
            _mouse.Update();
            var pressed = GetPressedKeys();
            var newKeys = _wasPressed == null ? pressed : pressed.Except(_wasPressed);
            _keys = pressed;

            var inputstate = GamePad.GetState(0);
            _padState = inputstate;

            var pressedKeyNames = new List<KeyName>();
            foreach(var key in Enum.GetValues(typeof(KeyName)))
            {
                if(GetPadKeys(_padState,(KeyName)key))
                    pressedKeyNames.Add((KeyName)key);
            }


            _padDown = pressedKeyNames;
            var newPadPressees = _padWasPressed == null ? pressedKeyNames : pressedKeyNames.Except(_padWasPressed);
            _padPressed = newPadPressees;


            _pressed = newKeys;

            _removed = _wasPressed == null ? new List<Keys>() : _wasPressed.Except(pressed);

            _wasPressed = pressed;
            _padWasPressed = pressedKeyNames;
            if(IsKeyPressed(KeyName.Editor_ToggleMouse))
            {
                _mouseMove = !_mouseMove;
            }
            if(_mouseMove)
            {
                _cursorX += _padState.ThumbSticks.Left.X * 4;
                _cursorY -= _padState.ThumbSticks.Left.Y * 5;
            }
            else
            {
                _cursorX = Mouse.GetState().Position.X;
                _cursorY = Mouse.GetState().Position.Y;
            }
        }


        private bool GetPadKeys(GamePadState inputstate, KeyName keyname)
        {
            return GamePadMap(keyname).Any(x => x == ButtonState.Pressed);
        }

        public int GetX()
        {
            if (IsKeyDown(KeyName.Right))
                return 1;
            else if (IsKeyDown(KeyName.Left))
                return -1;

            return 0;
        }

        public int GetY()
        {
            if (IsKeyDown(KeyName.Down))
                return 1;
            else if (IsKeyDown(KeyName.Up))
                return -1;

            return 0;
        }

        (BufferedMouseState left, BufferedMouseState right) IMouseHandler.MouseButtonState()
        {
            return _mouse.MouseButtonState();
        }

        public bool IsKeyPressed(KeyName key)
        {
            return _pressed.Any(pressed => KeyMap(key).Contains(pressed)) || _padPressed.Contains(key) || MouseMap().Contains(key);
        }

        private KeyName[] MouseMap()
        {
            var keys = new List<KeyName>();
            if (_mouse.MouseButtonState().left == BufferedMouseState.Down)
            {
                keys.Add(KeyName.Select);
                keys.Add(KeyName.Editor_PlaceTile);
            }
            if(_mouse.MouseButtonState().right == BufferedMouseState.Down)
            {
                keys.Add(KeyName.Editor_DrawLine);
            }

            return keys.ToArray();
        }

        public Keys[] KeyMap(KeyName key) => key switch {
            KeyName.Left => new[] { Keys.Left, Keys.J },
            KeyName.Right => new[] { Keys.Right, Keys.K },
            KeyName.Up => new[] { Keys.Up, Keys.K },
            KeyName.Down => new[] { Keys.Down, Keys.J },
            KeyName.Select => new[] { Keys.A },
            KeyName.Back => new[] { Keys.S },
            KeyName.Quit => new[] { Keys.C },
            KeyName.Option => new[] { Keys.O },
            KeyName.Jump => new[] { Keys.Space },
            KeyName.Drop => new[] { Keys.Down, Keys.LeftControl },
            KeyName.Attack => new[] { Keys.A, Keys.S },

            KeyName.Editor_DrawLine => new[] { Keys.LeftShift },
            KeyName.Editor_Fill => new[] { Keys.F },
            KeyName.Editor_Undo => new[] { Keys.U },
            KeyName.Editor_ShowTiles => new[] { Keys.T },
            KeyName.Editor_ToggleCameraMove => new[] { Keys.C },
            KeyName.Editor_SwapHeading => new[] { Keys.R },
            KeyName.Editor_ToggleDebugDraw => new[] { Keys.G },
            KeyName.Editor_ToggleEditor => new[] { Keys.E },

            KeyName.Editor_PlaceTile => new[] { Keys.E },
            KeyName.Editor_ToggleMouse => new[] { Keys.M },

            KeyName.Editor_SaveLevel => new[] { Keys.S },
            KeyName.Editor_LoadLevel => new[] { Keys.L },
            KeyName.Editor_ReloadLevel => new[] { Keys.K,Keys.RightControl },
            KeyName.Editor_SaveLevelAs => new[] { Keys.S,Keys.RightControl },

        };

        private ButtonState[] GamePadMap(KeyName keyname)
        { 
            var dpad = _padState.DPad;
            var buttons = _padState.Buttons;
            var shoulder = _padState.Triggers;

            return keyname switch
            {
                KeyName.Left => new[] {dpad.Left},
                KeyName.Right => new[] {dpad.Right},
                KeyName.Down => new[] {dpad.Down},
                KeyName.Up => new[] {dpad.Up},
                KeyName.Jump => new[] {buttons.A},
                KeyName.Attack => new[] {buttons.X},
                KeyName.Drop => new[] { dpad.Down },

                KeyName.Select => new[] {buttons.A },
                KeyName.Editor_ToggleEditor => new[] {buttons.Start },
                KeyName.Editor_ToggleDebugDraw => new[] {buttons.LeftStick },
                KeyName.Editor_ToggleMouse => new[] {buttons.LeftStick },
                KeyName.Editor_PlaceTile => new[] {buttons.A },
                KeyName.Editor_DrawLine => new[] {buttons.RightShoulder },
                KeyName.Editor_ToggleCameraMove => new[] {buttons.RightShoulder },
                KeyName.Editor_ShowTiles => new[] {buttons.LeftShoulder },
                KeyName.Editor_SwapHeading => new[] {buttons.RightStick },
                KeyName.Editor_Undo => new[] {buttons.B },

                //KeyName.Editor_SaveLevel => new[] {},
                //KeyName.Editor_SaveLevelAs => new[] {},
                //KeyName.Editor_LoadLevel => new[] {},
                //KeyName.Editor_LoadLevelAs => new[] {},


                _ => new[] { ButtonState.Released }
            }; 
        }

        public bool IsKeyDown(KeyName keys)
        {
            return KeyMap(keys).Any(key => _keys.Contains(key)) || _padDown.Contains(keys) || MouseMap().Contains(keys);
        }

        public int GetCursorX()
        {
            return (int)_cursorX;
        }

        public int GetCursorY()
        {
            return (int)_cursorY;
        }
    }
}
