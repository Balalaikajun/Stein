namespace Application.DTOs.Group;

public class GroupGetDto
{
    public GroupKeyDto Id { get; set; }
    public string Acronym { get; set; }
    public string TeacherFullname { get; set; }
    public int StudentCount { get; set; }
    public bool IsActive { get; set; }
}
