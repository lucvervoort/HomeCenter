namespace FileInfoCollector
{
    public class FileInfoRecord
    {
        public string? FilePath { get; set; }
        public long Size { get; set; }
        public string? Checksum { get; set; }
        public bool Exists { get; set; }
        public DateTime? Created { get; set; }
        public Exception? Error { get; set; }
    }
}
