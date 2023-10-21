
namespace DocumentGenerator.Web.Data
{
    public class FileBlobHelper
    {
        public string DocFolder { get; set; }
        public FileBlobHelper()
        {
            try
            {
                Setup();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }
        async void Setup()
        {

            DocFolder = AppContext.BaseDirectory + @"wwwroot/dms";
            if (!Directory.Exists(DocFolder))
            {
                Directory.CreateDirectory(DocFolder);
            }
        }

        public async Task<byte[]> DownloadFile(string fileName)
        {
            try
            {
                
                if (!string.IsNullOrEmpty(DocFolder))
                {
                    var targetFile = $"{DocFolder}/{fileName}";
                    var res = await File.ReadAllBytesAsync(targetFile);
                    return res;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }
            return default;
        }

        public string GetFPath(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(DocFolder))
                {
                    var targetFile = $"{DocFolder}/{fileName}";
                    return targetFile;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }
            return default;
        }
        public async Task<bool> UploadFile(string fileName, byte[] Data)
        {
            try
            {
                var res = false;
                if (!string.IsNullOrEmpty(DocFolder))
                {
                    var targetFile = $"{DocFolder}/{fileName}";
                    File.WriteAllBytes(targetFile, Data);
                    res = true;
                }
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
