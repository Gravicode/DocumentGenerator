using Microsoft.AspNetCore.Components.Forms;

namespace DocumentGenerator.Web.Data
{
    public class FileModel
    {
        public string Name { get; set; }
        public IBrowserFile File { get; set; }
    }
}
