using AutoMapper;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;

namespace ShiftSwap.R.PL.Mapping
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectReadDto>()
                .ForMember(dest => dest.AgentCount, opt => opt.MapFrom(src => src.Agents.Count));

            CreateMap<Project, ProjectDetailsDto>()
                .ForMember(dest => dest.Agents, opt => opt.MapFrom(src => src.Agents));

            CreateMap<Agent, AgentInProjectDto>();

            CreateMap<CreateProjectDto, Project>();

            CreateMap<Project, EditProjectDto>().ReverseMap();
        }
    }
}
