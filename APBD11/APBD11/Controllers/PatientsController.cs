using APBD11.DTOs.GET;
using APBD11.DTOs.POST;
using APBD11.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD11.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PatientDTO>>> GetPatients([FromQuery] string? search)
    {
        var patients = await _patientService.GetPatients(search);
        return Ok(patients);
    }
    
    
    [HttpPost("{pesel}/bedassignments")]
    public async Task<IActionResult> AssignBedToPatient(
        [FromRoute] string pesel,
        [FromBody] CreateBedAssignmentDTO request)
    {
        try
        {
            await _patientService.AssignBedToPatient(pesel, request);

            return Created($"/api/patients/{pesel}/bedassignments", new
            {
                message = "Bed was successfully assigned to patient."
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }
    
    
}