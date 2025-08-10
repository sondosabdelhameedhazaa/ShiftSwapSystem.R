using AutoMapper;
using ShiftSwap.R.DAL.Models;
using ShiftSwap.R.PL.Dtos;

namespace ShiftSwap.R.PL.Mapping
{
    public class AgentProfile : Profile
    {
        public AgentProfile()
        {
            // ✅ From Agent -> AgentReadDto (للعرض فقط)
            CreateMap<Agent, AgentReadDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.TeamLeaderName, opt => opt.MapFrom(src => src.TeamLeader != null ? src.TeamLeader.Name : null))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<Agent, AgentDetailsDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.TeamLeaderName, opt => opt.MapFrom(src => src.TeamLeader != null ? src.TeamLeader.Name : string.Empty));

            // ✅ From CreateAgentDto -> Agent (لإنشاء Agent جديد)
            CreateMap<CreateAgentDto, Agent>();

            // ✅ From EditAgentDto -> Agent (للتعديل على Agent موجود)
            CreateMap<EditAgentDto, Agent>();

            // ✅ From Agent -> EditAgentDto (لتحميل بيانات Agent في فورم التعديل)
            CreateMap<Agent, EditAgentDto>();
        }
    }
}

