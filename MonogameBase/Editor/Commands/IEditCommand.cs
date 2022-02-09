namespace MonoGameBase.Editor.Commands
{
    public interface IEditCommand
    {
        void Execute();
        void Undo();
    }
}
