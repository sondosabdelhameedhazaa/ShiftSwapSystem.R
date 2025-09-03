using AutoMapper;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;

namespace ShiftSwap.R.PL.Mapping
{
    public class ShiftSwapProfile : Profile
    {
        public ShiftSwapProfile()
        {
            CreateMap<ShiftSwapRequest, ShiftSwapRequestReadDto>()
               .ForMember(dest => dest.RequestorName, opt => opt.MapFrom(src => src.RequestorAgent.Name))
               .ForMember(dest => dest.RequestorHRID, opt => opt.MapFrom(src => src.RequestorAgent.HRID)) 
               .ForMember(dest => dest.TargetName, opt => opt.MapFrom(src => src.TargetAgent.Name))
               .ForMember(dest => dest.TargetHRID, opt => opt.MapFrom(src => src.TargetAgent.HRID)) 
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
               .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedBy != null ? src.ApprovedBy.Name : null));

            CreateMap<ShiftSwapRequestCreateDto, ShiftSwapRequest>();
        }
    }
}
