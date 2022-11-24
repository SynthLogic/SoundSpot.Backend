using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.BusinessLogic;
using API.Models;
using AutoMapper;
using System.Text.RegularExpressions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserLogic _logic;
        private readonly IMapper _mapper;

        public UserController(UserLogic logic, IMapper mapper)
        {
            _logic = logic;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Register([FromForm] string username, [FromForm] string email, [FromForm] string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
                errors.Add("Please provide a username");

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Please provide an email");

            if (!IsValid(email))
                errors.Add("Please provide a valid email");

            if (string.IsNullOrWhiteSpace(password))
                errors.Add("Please provide a password");

            if (errors.Any())
                return BadRequest(string.Join("\r\n", errors));

            try
            {
                var result = await _logic.CreateUser(username, email, password);
                return result is null
                    ? Ok("User successfully added to the database")
                    : StatusCode(409, result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to add user to the database:\r\n{e}");
            }            
        }

        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(string), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Please provide an email");

            if (!IsValid(email))
                errors.Add("Please provide a valid email");

            if (string.IsNullOrWhiteSpace(password))
                errors.Add("Please provide a password");

            if (errors.Any())
                return BadRequest(string.Join("\r\n", errors));

            try
            {
                var user = await _logic.GetUser(email, password);
                return user is null
                    ? StatusCode(204, "Unable to find user in the database")
                    : Ok(_mapper.Map<User, UserDto>(user));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to find user in the database:\r\n{e}");
            }
        }

        [HttpPatch]
        [Route("Update/{email}/{username}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> UpdateUser([FromRoute] string email, [FromRoute] string username, [FromBody] User updatedData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Please provide an email");

            if (!IsValid(email))
                errors.Add("Please provide a valid email");

            if (string.IsNullOrWhiteSpace(username))
                errors.Add("Please provide a username");

            if (updatedData is null)
                errors.Add("Please provide data to update");

            if (errors.Any())
                return BadRequest(string.Join("\r\n", errors));

            try
            {
                var result = await _logic.UpdateUser(email, username, updatedData);
                return !result
                    ? StatusCode(204,"Unable to find user in the database")
                    : Ok("Successfully updated user");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Unable to find user in the database:\r\n{e}");
            }
        }

        private static bool IsValid(string email)
        {
            try
            {
                var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var match = regex.Match(email ?? "");
                return match.Success;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }
    }
}
