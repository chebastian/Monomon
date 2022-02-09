namespace MonoGameBase.Content
{
    public interface IContentProvider
    {
        T GetContent<T>(string key) where T : class;
    }
}
