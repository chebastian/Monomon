namespace MonoGameBase.Input
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

}
