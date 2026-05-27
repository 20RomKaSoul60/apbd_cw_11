namespace APBD11.DTOs.GET;

public class BedDTO
{
    public int Id { get; set; }
    public BedTypeDTO BedType { get; set; } = null!;
    public RoomDTO Room { get; set; } = null!;
}