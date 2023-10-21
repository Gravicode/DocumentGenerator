using DocumentGenerator.Web.Data;
using Microsoft.SemanticKernel;
using Microsoft.SemanticMemory.MemoryStorage.Qdrant;
using Microsoft.SemanticMemory;
using DocumentGenerator.Models;
using Microsoft.SemanticMemory.MemoryStorage.DevTools;
using DocumentGenerator.Web.Helpers;

namespace DocumentGenerator.Web.Services
{
    public class RAGService
    {
        public string SkillName { get; set; } = "RAGSkill";
        public string FunctionName { set; get; } = "RAG";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;
        string systemMessage;
        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();
        Memory DocMemory;
        public bool FallBackHandling { get; set; } = true;
        public bool IsConfigured { get; set; } = false;
        public RAGService()
        {

          
            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();
            var config = new OpenAIConfig() { EmbeddingModel = "text-embedding-ada-002", APIKey = apiKey, OrgId = orgId, TextModel = model, MaxRetries = 3 };
            var configVector = new SimpleVectorDbConfig() { StorageType = SimpleVectorDbConfig.StorageTypes.TextFile, Directory = FileIndexer.VectorDir };
            DocMemory = new Microsoft.SemanticMemory.MemoryClientBuilder()
            .WithSimpleVectorDb(configVector)
            .WithOpenAIDefaults(apiKey, orgId)
            .BuildServerlessClient();
            IsConfigured = true;
            
        }

        public void Reset()
        {
            Items.Clear();
        }

        public List<RAGItem> Items { get; set; } = new();

        public async Task<string> Chat(string userMessage)
        {
            if (!IsConfigured) return string.Empty;

            string Result = string.Empty;
            //if (IsProcessing) return Result;

            try
            {
                //IsProcessing = true;
                var answer = await DocMemory.AskAsync(userMessage);
                Console.WriteLine("Sources:\n");
                var newItem = new RAGItem() { Answer = answer.Result, CreatedDate = DateHelper.GetLocalTimeNow(), Question = userMessage };
                foreach (var x in answer.RelevantSources)
                {
                    newItem.Sources.Add(new SourceItem() { Link = x.Link, Source = x.SourceName });
                    Console.WriteLine($"  - {x.SourceName}  - {x.Link} [{x.Partitions.First().LastUpdate:D}]");
                }
                Console.WriteLine($"Question: {userMessage}\n\nAnswer: {answer.Result}");
                Items.Add(newItem);
                Result = answer.Result;
                return Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                //IsProcessing = false;
            }
            return Result;
        }

    }
}
