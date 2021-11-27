using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Input
{
    public enum BufferedMouseState
    {
        Clicked,
        Released,
        Down,
        Up
    };

    public enum MouseButton
    {
        Left, Mid, Right
    }

    public interface IMouseHandler
    {
        (BufferedMouseState left, BufferedMouseState right) MouseButtonState();
    }

    public class BufferedMouse : IMouseHandler
    {
        private MouseState _oldState;
        private bool _clicked;
        private bool _released;
        (BufferedMouseState left, BufferedMouseState right) _state;

        public (BufferedMouseState left, BufferedMouseState right) MouseButtonState()
        {
            return _state;
        }

        public void Update(GameTime gt)
        {
            BufferedMouseState GetState(ButtonState oldState, ButtonState current)
            {
                var clicked = (current== ButtonState.Pressed && oldState== ButtonState.Released);
                var released = (oldState== ButtonState.Pressed && current== ButtonState.Released);

                if (clicked)
                    return BufferedMouseState.Clicked;
                else if (current == ButtonState.Pressed)
                    return BufferedMouseState.Down;
                else if (released)
                    return BufferedMouseState.Released;

                return BufferedMouseState.Up; 
            }

            var current = Mouse.GetState();
            var newLeft = GetState(_oldState.LeftButton, current.LeftButton);
            var newRight = GetState(_oldState.RightButton, current.RightButton);

            _oldState = current;
            _state = (newLeft, newRight);
        }
    }
}
