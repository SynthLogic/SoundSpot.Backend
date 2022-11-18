using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.BusinessLogic;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentController : ControllerBase
    {
        private readonly InstrumentLogic _logic;

        public InstrumentController(InstrumentLogic logic)
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
                await _logic.UploadFile(fileContents, name, category);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to add instrument to the database:\r\n{e}");
            }

            return Ok("Instrument successfully added to the database");
        }

        [HttpGet]
        [Route("GetAll")]
        [ProducesResponseType(typeof(List<Instrument>), 200)]
        public async Task<IActionResult> GetAll()
        {
            List<Instrument> result;

            try
            {
                result = await _logic.GetAllFiles();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to get instruments from the database:\r\n{e}");
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("GetOne/{name}")]
        [ProducesResponseType(typeof(Instrument), 200)]
        public async Task<IActionResult> GetOne([FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Please provide a name for the instrument");

            Instrument result;

            try
            {
                result = await _logic.GetFile(name);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to get {name} from the database:\r\n{e}");
            }

            return Ok(result);
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
