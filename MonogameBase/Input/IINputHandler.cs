namespace MonoGameBase.Input
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
        Option,
        Editor_DrawLine,
        Editor_ShowTiles,
        Jump,
        Drop,
        Attack,
        Editor_Fill,
        Editor_Undo,
        Editor_SwapHeading,
        Editor_ToggleCameraMove,
        Editor_ToggleEditor,
        Editor_ToggleDebugDraw,
        Editor_PlaceTile,
        Editor_ToggleMouse,
        Editor_LoadLevel,
        Editor_ReloadLevel,
        Editor_SaveLevelAs,
        Editor_SaveLevel,
    }
    public interface IINputHandler : IMouseHandler
    {
        int GetX();
        int GetY();
        int GetCursorX();
        int GetCursorY();
        bool IsKeyPressed(KeyName key);
        bool IsKeyDown(KeyName keys);
    }

}
