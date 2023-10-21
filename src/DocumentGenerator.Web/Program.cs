using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using DocumentGenerator.Web.Data;
using MudBlazor.Services;
using DocumentGenerator.Web.Services;
using DocumentGenerator.Web.Pages;
using DocumentGenerator.Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(); 
builder.Services.AddTransient<DocFunctionService>();
builder.Services.AddTransient<DocTemplateService>();
builder.Services.AddTransient<DocGenService>();
builder.Services.AddTransient<RAGService>();
builder.Services.AddTransient<FileIndexer>();
builder.Services.AddTransient<FileBlobHelper>();
builder.Services.AddSingleton<AppState>();


var app = builder.Build();
var configBuilder = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", optional: false);
IConfiguration Configuration = configBuilder.Build();
AppConstants.SQLConn = Configuration["ConnectionStrings:SqlConn"];
AppConstants.UploadUrlPrefix = Configuration["UploadUrlPrefix"];


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var db = new DocumentGeneratorDb();
db.Database.EnsureCreated();
//FileIndexer.CrawledDir = "You source folder for documents here";
app.Run();