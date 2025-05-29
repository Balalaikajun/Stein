using Application.DTOs.Group;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<Group, GroupGetDto>()
            .ForMember(dest => dest.TeacherFullname,
                opt =>
                    opt.MapFrom(src =>
                        src.Teacher.Surname + " " +
                        src.Teacher.Name.Substring(0, 1) + "." +
                        src.Teacher.Patronymic.Substring(0, 1) + "."))
            .ForMember(dest => dest.StudentCount,
                opt => opt.MapFrom(src => src.Students.Count))
            .ForMember(d => d.Id,
                o => o.MapFrom(
                    src => new GroupKeyDto(src.SpecializationId, src.Year, src.Index)));

        CreateMap<GroupPostDto, Group>();
    }
}