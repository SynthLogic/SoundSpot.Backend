using AutoMapper;

namespace API.Models
{
    public class ApiProfiles : Profile
    {
        public ApiProfiles()
        {
            CreateMap<User, UserDto>();
        }
    }
}
