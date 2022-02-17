namespace TalkiPlay.Shared
{
    public interface IConfig
    {
        string DeviceNamePrefix { get; }
        string GetAssetDownloadUrl(int id);

        string ApiKey { get; }
    }
}