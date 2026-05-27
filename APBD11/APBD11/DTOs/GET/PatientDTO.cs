namespace APBD11.DTOs.GET;

public class PatientDTO
{
    public string Pesel { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Sex { get; set; } = null!;
    public List<AdmissionDTO> Admissions { get; set; } = new();
    public List<BedAssignmentDTO> BedAssignments { get; set; } = new();
}