using System;
using System.Collections.Generic;
using System.Text;

namespace API.Models
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public int HighestScore { get; set; }
        public int LatestScore { get; set; }
    }
}
