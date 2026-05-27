using APBD11.DTOs.GET;
using APBD11.DTOs.POST;

namespace APBD11.Services;

public interface IPatientService
{
    Task<List<PatientDTO>> GetPatients(string? search);
    Task AssignBedToPatient(string pesel, CreateBedAssignmentDTO request);
}