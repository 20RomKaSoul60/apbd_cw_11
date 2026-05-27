namespace APBD11.DTOs.GET;

public class RoomDTO
{
    public string Id { get; set; } = null!;
    public bool HasTv { get; set; }
    public WardDTO Ward { get; set; } = null!;
}