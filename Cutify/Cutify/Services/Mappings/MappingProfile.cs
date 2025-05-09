using AutoMapper;
using Cutify.Models;
using Cutify.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cutify.Services.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterVM, AppUser>();
            CreateMap<ReservationVM,Reservation>();
        }
    }
}
