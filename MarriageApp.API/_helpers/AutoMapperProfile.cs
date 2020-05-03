using System.Linq;
using AutoMapper;
using MarriageApp.API.Dtos;
using MarriageApp.API.Models;

namespace MarriageApp.API._helpers
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User,UserForListDto>().
            ForMember(dest=>dest.PhotoUrl,opt=>opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url)).
            ForMember(dest=>dest.Age,opt=>opt.MapFrom(src=>src.DateofBirth.Age()));
            CreateMap<User,UserForDetailedDto>().
            ForMember(dest=>dest.PhotoUrl,opt=>opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url)).
            ForMember(dest=>dest.Age,opt=>opt.MapFrom(src=>src.DateofBirth.Age()));
            CreateMap<Photo,PhotoDetailedDto>();
            CreateMap<UserForUpdateDto,User>();
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto,Photo>();
            CreateMap<UserForRegisterDto,User>();
            CreateMap<MessageForCreationDto,Message>().ReverseMap();
            CreateMap<Message,MessageToReturnDto>()
            .ForMember(m => m.SenderPhotoUrl,opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p=>p.IsMain).Url))
            .ForMember(m => m.RecipientPhotoUrl,opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p=>p.IsMain).Url));
        }
        
    }
}