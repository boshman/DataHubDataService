using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return new string[] { "file01.pdf", "file02.pdf", "file03.pdf" };
        }

        // GET api/values/5
        [HttpGet("{agencyID}/{filePath}")]
        public ActionResult<IEnumerable<DataHubFile>> Get(string agencyID, string filePath)
        {
            List<DataHubFile> files = new List<DataHubFile>();
            filePath = System.Net.WebUtility.UrlDecode(filePath);

            files = DbAccess.GetFileList(agencyID, filePath);
            System.Console.WriteLine($"files.Count = {files.Count()}");
            return files;
        }

        // POST data/
        [HttpPost()]
        public ActionResult<string> Post(DataHubFile dataHubFile)
        {
            string result = string.Empty;

            DbAccess.UploadFile(dataHubFile.AgencyID, dataHubFile.Path, dataHubFile.UploadedBy);
            result = "S3 Upload successful";

            return result;
        }
    }
}
