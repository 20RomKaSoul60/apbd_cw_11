namespace APBD11.DTOs.GET;

public class BedAssignmentDTO
{
    public int Id { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
    public BedDTO Bed { get; set; } = null!;
}