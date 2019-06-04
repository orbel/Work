using AutoMapper;
using UserManagement.Models;
using UserManagement.Entities;

namespace UserManagement.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserEntity>();
            CreateMap<UserEntity, User>();
        }
    }
}
