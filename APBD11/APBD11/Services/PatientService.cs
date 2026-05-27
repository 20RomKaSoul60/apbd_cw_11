using APBD11.Data;
using APBD11.DTOs.GET;
using APBD11.DTOs.POST;
using APBD11.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD11.Services;

public class PatientService : IPatientService
{
    private readonly Apbd11HwContext _dbContext;

    public PatientService(Apbd11HwContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PatientDTO>> GetPatients(string? search)
    {
        var query = _dbContext.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(patient =>
                EF.Functions.Like(patient.FirstName, $"%{search}%") ||
                EF.Functions.Like(patient.LastName, $"%{search}%"));
        }

        return await query
            .Select(patient => new PatientDTO
            {
                Pesel = patient.Pesel,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Age = patient.Age,
                Sex = patient.Sex ? "Male" : "Female",

                Admissions = patient.Admissions
                    .Select(admission => new AdmissionDTO
                    {
                        Id = admission.Id,
                        AdmissionDate = admission.AdmissionDate,
                        DischargeDate = admission.DischargeDate,
                        Ward = new WardDTO
                        {
                            Id = admission.Ward.Id,
                            Name = admission.Ward.Name,
                            Description = admission.Ward.Description
                        }
                    })
                    .ToList(),

                BedAssignments = patient.BedAssignments
                    .Select(bedAssignment => new BedAssignmentDTO
                    {
                        Id = bedAssignment.Id,
                        From = bedAssignment.From,
                        To = bedAssignment.To,
                        Bed = new BedDTO
                        {
                            Id = bedAssignment.Bed.Id,
                            BedType = new BedTypeDTO
                            {
                                Id = bedAssignment.Bed.BedType.Id,
                                Name = bedAssignment.Bed.BedType.Name,
                                Description = bedAssignment.Bed.BedType.Description
                            },
                            Room = new RoomDTO
                            {
                                Id = bedAssignment.Bed.Room.Id,
                                HasTv = bedAssignment.Bed.Room.HasTv,
                                Ward = new WardDTO
                                {
                                    Id = bedAssignment.Bed.Room.Ward.Id,
                                    Name = bedAssignment.Bed.Room.Ward.Name,
                                    Description = bedAssignment.Bed.Room.Ward.Description
                                }
                            }
                        }
                    })
                    .ToList()
            })
            .ToListAsync();
    }
    
    
    public async Task AssignBedToPatient(string pesel, CreateBedAssignmentDTO request)
    {
        if (request.To.HasValue && request.To.Value <= request.From)
        {
            throw new ArgumentException("End date must be later than start date.");
        }

        var patientExists = await _dbContext.Patients
            .AnyAsync(patient => patient.Pesel == pesel);

        if (!patientExists)
        {
            throw new KeyNotFoundException("Patient with given PESEL does not exist.");
        }

        var bedTypeExists = await _dbContext.BedTypes
            .AnyAsync(bedType => bedType.Name == request.BedType);

        if (!bedTypeExists)
        {
            throw new KeyNotFoundException("Bed type with given name does not exist.");
        }

        var wardExists = await _dbContext.Wards
            .AnyAsync(ward => ward.Name == request.Ward);

        if (!wardExists)
        {
            throw new KeyNotFoundException("Ward with given name does not exist.");
        }

        var requestedTo = request.To ?? DateTime.MaxValue;

        var availableBed = await _dbContext.Beds
            .Where(bed =>
                bed.BedType.Name == request.BedType &&
                bed.Room.Ward.Name == request.Ward &&
                !bed.BedAssignments.Any(assignment =>
                    assignment.From < requestedTo &&
                    (assignment.To ?? DateTime.MaxValue) > request.From))
            .FirstOrDefaultAsync();

        if (availableBed == null)
        {
            throw new KeyNotFoundException("No available bed matching given bed type, ward and time period.");
        }

        var bedAssignment = new BedAssignment
        {
            PatientPesel = pesel,
            BedId = availableBed.Id,
            From = request.From,
            To = request.To
        };

        _dbContext.BedAssignments.Add(bedAssignment);
        await _dbContext.SaveChangesAsync();
    }
    
}

