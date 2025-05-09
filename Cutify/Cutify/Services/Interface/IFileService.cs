namespace Cutify.Services.Interface
{
    public interface IFileService
    {
        Task<string> ReadFileAsync(string path);
    }
}
