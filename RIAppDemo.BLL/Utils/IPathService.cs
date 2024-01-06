namespace RIAppDemo.BLL.Utils
{
    public interface IPathService
    {
        string AppRoot { get; }
        string DataDirectory { get; }
        string ConfigFolder { get; }
    }
}