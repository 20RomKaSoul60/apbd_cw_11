namespace APBD11.DTOs.GET;

public class AdmissionDTO
{
    public int Id { get; set; }
    public DateTime AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public WardDTO Ward { get; set; } = null!;
}