using System.Web;
using GemBox.Document;
using DocumentGenerator.Models;
using DocumentGenerator.Web.Data;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ImageGeneration;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;

using Microsoft.SemanticMemory;

namespace DocumentGenerator.Web.Services
{
    public class FunctionInput
    {
        public DocFunction MyFunction { get; set; }
        public Dictionary<string, string> Input { get; set; } = new();

    }
    public class DocGenService
    {
        const int ImageDefaultSize = 512;
        public event StatusChangedEventHandler StatusChanged;
        public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
        public class StatusChangedEventArgs : EventArgs
        {
            public int Progress { get; set; }
            public string Message { get; set; }
        }
        public string SkillName { get; set; } = "DocGenSkill";

        public bool IsProcessing { get; set; } = false;
        public DocTemplate MyDoc { get; set; }
        public DocFunctionService DocFunctionSvc { get; set; }
        public DocTemplateService DocTemplateSvc { get; set; }
        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();
        public List<FunctionInput> MyFunctions { set; get; } = new();
        IKernel kernel { set; get; }
        FileBlobHelper FileBlobHelper { set; get; }
        bool HasIndexed = false;
        public List<RAGItem> Items { get; set; } = new();
        Memory DocMemory;
        string TempFolder;
        public DocGenService(DocFunctionService functionsvc, DocTemplateService templatesvc, FileBlobHelper FileBlobHelper)
        {
            this.FileBlobHelper = FileBlobHelper;
            this.DocTemplateSvc = templatesvc;
            this.DocFunctionSvc = functionsvc;
            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel = new KernelBuilder()
       .WithOpenAITextCompletionService(modelId: model, apiKey: apiKey, orgId: orgId, serviceId: "davinci")
       .WithOpenAIImageGenerationService(apiKey, orgId)
       .Build();
            var config = new OpenAIConfig() { EmbeddingModel = "text-embedding-ada-002", APIKey = apiKey, OrgId = orgId, TextModel = model, MaxRetries = 3 };

            DocMemory = new Microsoft.SemanticMemory.MemoryClientBuilder()
        .WithOpenAIDefaults(apiKey, orgId)
        .BuildServerlessClient();
            //SetupSkill();
        }

        public void SetupSkill(long IdTemplate)
        {
            MyFunctions.Clear();
            var selDoc = DocTemplateSvc.GetDataById(IdTemplate);
            MyDoc = selDoc;
            var listFun = selDoc.DocFunctions;
            foreach (var fun in listFun)
            {
                var selFun = DocFunctionSvc.GetDataById(fun);
                var newInput = new FunctionInput() { MyFunction = selFun };
                foreach (var param in selFun.Parameters)
                {
                    newInput.Input.Add(param, "");
                }
                MyFunctions.Add(newInput);
            }

        }
        const int MaxTokens = 2000;
        HttpClient client = new();

        public async Task<byte[]> GenerateDoc()
        {  
            string Result = string.Empty;
            if (IsProcessing) return default;
            IsProcessing = true;
            await Task.Delay(1);
            if (!HasIndexed)
            {
                //StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 1, Message = "indexing DMS for RAG first..." });
                //await Task.Delay(1);
                //await LoadIndex();
            }
            // Get AI service instance used to generate images
            var dallE = kernel.GetService<IImageGeneration>();
            try
            {
                try
                {

                    //List<byte[]> DocParts = new List<byte[]>();
                    StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 10, Message = "Start generating document..." });
                    await Task.Delay(1);

                    var queryString = HttpUtility.ParseQueryString(MyDoc.DocUrl);
                    var fname = queryString[0];
                    var bytes = await FileBlobHelper.DownloadFile(fname);
                    // Load Word document from file's path.
                    var document = DocumentModel.Load(new MemoryStream(bytes));
                    var Progress = 0;
                    var pctProgress = 100 / MyFunctions.Count;
                    foreach (var fun in MyFunctions)
                    {
                        var FunctionName = fun.MyFunction.FunctionName;
                        var Temperature = fun.MyFunction.Temp;
                        var TopP = fun.MyFunction.TopP;

                        string skPrompt = fun.MyFunction.Prompt;
                        if (!ListFunctions.ContainsKey(FunctionName))
                        {
                            var promptConfig = new PromptTemplateConfig
                            {
                            
                            };

                            var promptTemplate = new PromptTemplate(
                    skPrompt,                        // Prompt template defined in natural language
                    promptConfig,                    // Prompt configuration
                    kernel                           // SK instance
                );


                            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

                            var currentFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
                            ListFunctions.Add(FunctionName, currentFunction);
                        }
                        var context = new ContextVariables();
                        foreach (var param in fun.Input)
                        {
                            context.Set(param.Key, param.Value);
                        }
                        switch (fun.MyFunction.FunctionType)
                        {
                            case FunctionTypes.Text:
                                var result = await kernel.RunAsync(context, ListFunctions[FunctionName]);
                                document.Content.Replace(fun.MyFunction.ResultTag, result.GetValue<string>());
                                break;
                            case FunctionTypes.Image:
                                var imgPrompt = skPrompt;
                                foreach (var param in fun.Input)
                                {
                                    var key = "{{$" + param.Key + "}}";
                                    imgPrompt = imgPrompt.Replace(key, param.Value);
                                }
                                var imageUrl = await dallE.GenerateImageAsync(imgPrompt.Trim(), ImageDefaultSize, ImageDefaultSize);

                                try
                                {
                                    var content = document.Content.Find(fun.MyFunction.ResultTag).First();
                                    var bytesImage = await client.GetByteArrayAsync(imageUrl);
                                    var ms = new MemoryStream(bytesImage);
                                    Picture InsertedPic = new Picture(document, ms);
                                    FloatingLayout PicLayout = new FloatingLayout(
                                          new HorizontalPosition(HorizontalPositionType.Left, HorizontalPositionAnchor.Page),
                                          new VerticalPosition(2, GemBox.Document.LengthUnit.Inch, VerticalPositionAnchor.Page),
                                          InsertedPic.Layout.Size);
                                    PicLayout.WrappingStyle = TextWrappingStyle.InFrontOfText;
                                    InsertedPic.Layout = PicLayout;
                                    content.Set(InsertedPic.Content);

                                }
                                catch (Exception imgex)
                                {

                                    Console.WriteLine("failed to download image:" + imgex);
                                }

                                break;
                            case FunctionTypes.RAG:
                                var ragPrompt = skPrompt;
                                foreach (var param in fun.Input)
                                {
                                    var key = "{{$" + param.Key + "}}";
                                    ragPrompt = ragPrompt.Replace(key, param.Value);
                                }
                                MemoryAnswer answer = null;
                                if (!string.IsNullOrEmpty(fun.MyFunction.TagFilter))
                                {
                                     answer = await DocMemory.AskAsync(question: ragPrompt, filter: new MemoryFilter().ByTag("folder", fun.MyFunction.TagFilter), cancellationToken: new CancellationToken());
                                }
                                else
                                {
                                     answer = await DocMemory.AskAsync(ragPrompt);
                                }
                              
                                document.Content.Replace(fun.MyFunction.ResultTag, answer?.Result);
                                break;
                        }
                        Progress += pctProgress;
                        StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = Progress, Message = $"generate function {FunctionName}..." });
                        await Task.Delay(1);

                    }
                    var pdfSaveOptions = new GemBox.Document.DocxSaveOptions() { };
                    var dataBytes = new byte[0];
                    using (var pdfStream = new MemoryStream())
                    {
                        document.Save(pdfStream, pdfSaveOptions);
                        dataBytes = pdfStream.ToArray();
                    }
                    StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 100, Message = $"finished..." });
                    await Task.Delay(1);

                    return await Task.FromResult(dataBytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return default;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return default;
            }
            finally
            {
                IsProcessing = false;
            }
        }
       
    }
}

