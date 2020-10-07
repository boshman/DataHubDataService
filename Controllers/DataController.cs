using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataHubDataService.Models;
using DataHubFileService;

namespace DataHubDataService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            System.Console.Write("called GET /data");
            return new string[] { "file01.pdf", "file02.pdf", "file03.pdf" };
        }

        // GET api/values/1001/<encoded_file_path>
        [HttpGet("{agencyID}/{filePath}")]
        public async Task<ActionResult<IEnumerable<DataHubFile>>> Get(string agencyID, string filePath)
        {
            List<DataHubFile> files = new List<DataHubFile>();
            filePath = System.Net.WebUtility.UrlDecode(filePath);
            filePath = filePath.Replace(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("/")), "/");

            files = await DbAccess.GetFileList(agencyID, filePath);
            System.Console.WriteLine($"files.Count = {files.Count()}");
            return files;
        }

        // POST data/
        [HttpPost()]
        public async Task<ActionResult<string>> Post(DataHubFile dataHubFile)
        {
            string result = string.Empty;

            result = await DbAccess.UploadFile(dataHubFile.AgencyID, dataHubFile.Path, dataHubFile.UploadedBy);

            return new JsonResult(result);
        }

        // Delete api/values/1001/<encoded_file_path>
        [HttpDelete("{agencyID}/{filePath}")]
        public async Task<ActionResult<string>> Delete(string agencyID, string filePath)
        {
            string result = string.Empty;
            filePath = System.Net.WebUtility.UrlDecode(filePath);
            filePath = filePath.Replace(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("/")), "/");

            result = await DbAccess.DeleteFile(agencyID, filePath);
            return new JsonResult(result);
        }
    }
}
