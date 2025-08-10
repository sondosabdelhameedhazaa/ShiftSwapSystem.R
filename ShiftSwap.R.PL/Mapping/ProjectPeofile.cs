using AutoMapper;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;

namespace ShiftSwap.R.PL.Mapping
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            // ✅ Project → ProjectReadDto (لعرض قائمة المشاريع)
            CreateMap<Project, ProjectReadDto>()
                .ForMember(dest => dest.AgentCount, opt => opt.MapFrom(src => src.Agents.Count));

            // ✅ Project → ProjectDetailsDto (لعرض تفاصيل المشروع)
            CreateMap<Project, ProjectDetailsDto>()
                .ForMember(dest => dest.Agents, opt => opt.MapFrom(src => src.Agents));

            // ✅ Agent → AgentInProjectDto (لربط العملاء داخل التفاصيل)
            CreateMap<Agent, AgentInProjectDto>();

            // ✅ CreateProjectDto → Project (لإنشاء مشروع جديد)
            CreateMap<CreateProjectDto, Project>();

            // ✅ EditProjectDto ↔ Project (للتعديل)
            CreateMap<Project, EditProjectDto>().ReverseMap();
        }
    }
}
