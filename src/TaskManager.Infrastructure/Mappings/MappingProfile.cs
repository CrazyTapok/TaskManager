using AutoMapper;

namespace TaskManager.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Entities.Company, Core.Models.Company>().ReverseMap();
            CreateMap<Entities.Employee, Core.Models.Employee>().ReverseMap();
            CreateMap<Entities.Project, Core.Models.Project>().ReverseMap();
            CreateMap<Entities.Task, Core.Models.Task>().ReverseMap();
        }
    }
}
