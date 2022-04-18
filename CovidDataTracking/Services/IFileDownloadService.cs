namespace Services
{
    public interface IFileDownloadService
    {
        bool DownloadFile(string url, string storageLocation);
    }
}