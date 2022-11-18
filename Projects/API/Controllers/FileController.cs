using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.BusinessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileLogic _logic;

        public FileController(FileLogic logic)
        {
            _logic = logic;
        }

        [HttpPost]
        [Route("Upload")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery(Name = "name")] string name, [FromQuery(Name = "category")] string category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var errors = new List<string>();

            var fileContents = ReadFormFile(file);

            if (fileContents.Length == 0)
                errors.Add("The attached file is empty.");

            if (string.IsNullOrWhiteSpace(name))
                errors.Add("Please provide a name for the instrument");

            if (errors.Any())
                return BadRequest(string.Join(',', errors));

            try
            {
                await _logic.UploadFile(fileContents, file.ContentType, file.Length, name, category);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to add the data to the database:\r\n{e}");
            }

            return Ok("Instrument successfully added to the database");
        }

        [HttpGet]
        [Route("GetAll")]
        [ProducesResponseType(typeof(List<Models.File>), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _logic.GetAllFiles();
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to get the data from the database:\r\n{e}");
            }
        }

        [HttpGet]
        [Route("Get/{name}")]
        [ProducesResponseType(typeof(Models.File), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetOne([FromQuery] string contentType, [FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Please provide a name for the instrument");

            try
            {
                var result = await _logic.GetFile(contentType, name);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to get {name} from the database:\r\n{e}");
            }
        }

        private static byte[] ReadFormFile(IFormFile file)
        {
            if (file is null) return new byte[] { };
            using var reader = new StreamReader(file.OpenReadStream());
            using var ms = new MemoryStream();
            reader.BaseStream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
