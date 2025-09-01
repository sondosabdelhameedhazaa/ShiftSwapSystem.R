using AutoMapper;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;

namespace ShiftSwap.R.PL.Mapping
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            //  Project → ProjectReadDto 
            CreateMap<Project, ProjectReadDto>()
                .ForMember(dest => dest.AgentCount, opt => opt.MapFrom(src => src.Agents.Count));

            //  Project → ProjectDetailsDto 
            CreateMap<Project, ProjectDetailsDto>()
                .ForMember(dest => dest.Agents, opt => opt.MapFrom(src => src.Agents));

            //  Agent → AgentInProjectDto 
            CreateMap<Agent, AgentInProjectDto>();

            //  CreateProjectDto → Project 
            CreateMap<CreateProjectDto, Project>();

            //  EditProjectDto ↔ Project 
            CreateMap<Project, EditProjectDto>().ReverseMap();
        }
    }
}
