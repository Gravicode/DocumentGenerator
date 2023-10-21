namespace DocumentGenerator.Web.Data
{
    public class AppConstants
    {
        public static string UploadUrlPrefix = "https://storagemurahaje.blob.core.windows.net/FileGue";

        public static string SQLConn = "";
        public static string ChatModel = "gpt-3.5-turbo";
        public const int TokenLimit = 4000;


        public static string OpenAIApiKey = "[OPEN AI KEY HERE]";//"-- Open AI Key --";
        public static string OrgID = "[ORG ID OPEN AI HERE]";//"-- ORG ID --";
        public static string Model = "text-davinci-003";
        public static (string model, string apiKey, string orgId) GetSettings()
        {
            LoadSettings();
            return (Model, OpenAIApiKey, OrgID);
        }
        public static void SaveSetting()
        {
            Preferences.Set("OpenAIApiKey", OpenAIApiKey);
            Preferences.Set("OrgID", OrgID);
            Preferences.Set("Model", Model);

        }

        public static void LoadSettings()
        {
            Model = Preferences.Get("Model", Model);
            OrgID = Preferences.Get("OrgID", OrgID);
            OpenAIApiKey = Preferences.Get("OpenAIApiKey", OpenAIApiKey);
        }

        public static long MaxAllowedFileSize = 10 * 1024000;
        public static long DefaultStorageSize = 21474836480;

    }
}
