using Api.Models.Entites;
using Api.Models.ViewModel.Account;
using AutoMapper;

namespace Api.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            //CreateMap<RegisterViewModel, User>().ForMember(dest=>dest.UserName,opt=>opt.MapFrom(src=>src.FirstName+src.LastName));
            CreateMap<RegisterViewModel, User>().ForMember(dest=>dest.UserName,opt=>opt.MapFrom(src=>src.Email));
           // CreateMap<UserViewModel, User>().ForMember(dest=>dest.);
        }
    }
}
