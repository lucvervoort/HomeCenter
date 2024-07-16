namespace DiskCleanup;

class FileData
{
    public string SnapshotTime { get; set; }
    public string SystemName { get; set; }
    public string FileCreationTime { get; set; }
    public string Fingerprint { get; set; }
    public string FileName { get; set; }

    public void Print()
    {
        Console.WriteLine("\"" + SnapshotTime + "\", \"" + SystemName + "\", \"" + FileCreationTime + "\", " + Fingerprint + ", \"" + FileName + "\"");
    }
}

class Program
{
    static byte[] MD5Hash(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open,
                                                 FileAccess.Read,
                                                 FileShare.Read,
                                                 4096,
                                                 FileOptions.SequentialScan))
        {
            using (System.Security.Cryptography.SHA512 hash = System.Security.Cryptography.SHA512.Create())
            {
                System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                return hash.ComputeHash(stream);
            }
            //return MD5.Create().ComputeHash(stream);
        }
    }

    static void Main()
    {
        // Make sure directory exists before using this!
        var files = new List<string>(Directory.GetFiles("C:\\projects",
            "*.*",
            SearchOption.AllDirectories));
        Console.WriteLine("Calculating...");
        Method(files);
        Console.WriteLine("Collected: ");
        foreach(var d in _fileData.Values)
        {
            d.Print();
        }
    }

    private static Dictionary<string, FileData> _fileData = new();

    static void Method(List<string> files)
    {
        var takenOn = DateTime.Now.ToString();
        foreach (var e in from path in files
                          select new
                          {
                              Path = Path.GetFullPath(path),
                              CreationTime = new FileInfo(path).CreationTime,
                              Hash = BitConverter.ToString(MD5Hash(path))
                                                .ToLowerInvariant()
                                                .Replace("-", string.Empty)
                          })
        {
            var f = new FileData() { SnapshotTime = takenOn, SystemName = System.Environment.MachineName, FileCreationTime = e.CreationTime.ToString(), Fingerprint = e.Hash, FileName = e.Path };
            if (_fileData.ContainsKey(e.Hash))
            {
                var oldItem = _fileData[e.Hash];
                Console.WriteLine($"You can safely clean up: {f.FileName} or {oldItem.FileName}");
            }
            else
            {
                _fileData.Add(e.Hash, f);
            }
        }
    }
}