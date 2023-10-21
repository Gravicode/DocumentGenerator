
using DocumentGenerator.Models;
using DocumentGenerator.Web.Data;
using Microsoft.SemanticMemory;
using Microsoft.SemanticMemory.MemoryStorage.DevTools;
using System.Reflection;

namespace DocumentGenerator.Web.Helpers;
public class FileIndexer
{
    public EventHandler<FileItem> ItemIndexed;

    static string _CrawledDir;
    public static string CrawledDir
    {
        get
        {
            if (string.IsNullOrEmpty(_CrawledDir))
            {
                var fPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                _CrawledDir = Path.Combine(fPath, "Crawled");
            }
            return _CrawledDir;
        }
        set
        {
            _CrawledDir = value;
        }
    }

    static string _VectorDir;
    public static string VectorDir
    {
        get
        {
            if (string.IsNullOrEmpty(_VectorDir))
            {
                var fPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                _VectorDir = Path.Combine(fPath, "VectorData");
            }
            return _VectorDir;
        }
    }
    public bool IsProcessing { set; get; } = false;
    public int Counter { get; set; } = 0;

    public FileIndexer()
    {
        if (!Directory.Exists(CrawledDir))
        {
            Directory.CreateDirectory(CrawledDir);
        }
        SetupIndexer();
    }
    Memory DocMemory;
    void SetupIndexer()
    {
        var folderText = FileIndexer.CrawledDir;
        //string model, apiKey, orgId;
        var (model, apiKey, orgId) = AppConstants.GetSettings();
        var configVector = new SimpleVectorDbConfig() { StorageType = SimpleVectorDbConfig.StorageTypes.TextFile, Directory = VectorDir };
        DocMemory = new Microsoft.SemanticMemory.MemoryClientBuilder()
        .WithSimpleVectorDb(configVector)
        .WithOpenAIDefaults(apiKey, orgId)
        .BuildServerlessClient();

    }

    readonly string[] extensions = new string[] { ".txt", ".pdf", ".docx" };
    public async Task<List<FileItem>> GetAllFiles()
    {
        var items = new List<FileItem>();
        try
        {
            var Counter = 1;
            DirectoryInfo dir = new DirectoryInfo(CrawledDir);
            var files = dir.GetFilesByExtensions(extensions);
            foreach (var CurrentFile in files)
            {
                items.Add(new FileItem() { FileName = CurrentFile.Name, Ext = CurrentFile.Extension, No = Counter++, Size = CurrentFile.Length,FilePath = CurrentFile.FullName });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("get files failed:" + ex);
        }
        return items;
    }
    public async Task DoIndexing()
    {
        if (IsProcessing) return;
        IsProcessing = true;
        try
        {
            var Counter = 1;
            DirectoryInfo dir = new DirectoryInfo(CrawledDir);
            var files = dir.GetFilesByExtensions(extensions);
            foreach (var CurrentFile in files)
            {
                var tagcollection = new TagCollection();
                tagcollection.Add("user", "user1");
                tagcollection.Add("filename", CurrentFile.Name);
                var bytes = File.ReadAllBytes(CurrentFile.FullName);
                var ms = new MemoryStream(bytes);
                await DocMemory.ImportDocumentAsync(ms, CurrentFile.Name, documentId: CurrentFile.Name, tagcollection);
                System.Console.WriteLine($"file {CurrentFile.Name} has been indexed.");
                ItemIndexed?.Invoke(this, new FileItem() { FileName = CurrentFile.Name, Ext = CurrentFile.Extension, No = Counter++, Size = CurrentFile.Length, FilePath = CurrentFile.FullName });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("index fail:" + ex);
        }
        finally
        {
            IsProcessing = false;
        }

    }
   


}