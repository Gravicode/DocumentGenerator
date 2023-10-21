using Microsoft.AspNetCore.Mvc;
using DocumentGenerator.Web.Data;
using System.IO;
using System;
using Microsoft.AspNetCore.StaticFiles;
using System.Threading.Tasks;
using DocumentGenerator.Web.Helpers;

namespace DocumentGenerator.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DmsController : Controller
    {
        FileBlobHelper blob;
        public DmsController(FileBlobHelper blob)
        {
            this.blob = blob;

        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetFile(string filename)
        {
            var file = await blob.DownloadFile(filename);
            if (file != null)
            {
              
                var mime = MimeTypeHelper.GetMimeType(Path.GetExtension(filename));
                return File(file, mime,filename);
            }
            return NotFound();
        }
        
    }
}
