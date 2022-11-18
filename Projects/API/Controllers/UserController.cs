using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.BusinessLogic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserLogic _logic;

        public UserController(UserLogic logic)
        {
            _logic = logic;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CreateUser([FromForm] string username, [FromForm] string email, [FromForm] string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
                errors.Add("Please provide a username");

            if (string.IsNullOrWhiteSpace(email) && !IsValid(email))
                errors.Add("Please provide a valid email");

            if (string.IsNullOrWhiteSpace(password))
                errors.Add("Please provide a password");

            if (errors.Any())
                return BadRequest(string.Join(',', errors));

            try
            {
                await _logic.CreateUser(username, email, password);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to add instrument to the database:\r\n{e}");
            }

            return Ok("User successfully added to the database");
        }

        private static bool IsValid(string email)
        {
            try
            {
                var m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
