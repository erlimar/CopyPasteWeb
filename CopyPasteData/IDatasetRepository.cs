namespace CopyPasteData
{
    public interface IDatasetRepository
    {
        string NewID();
        string SetValue(string sessionId, string newValue);
        string GetValue(string sessionID);
    }
}
