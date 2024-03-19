using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class HealthRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<MedicalLeaveDTO>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.MedicalLeaves.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.MedicalLeaves.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<MedicalLeaveDTO>> GetAll()
        {
            var medicalLeaveDTOs = await appDbContext.MedicalLeaves
                .AsNoTracking()
                .Join(
                    appDbContext.Employees,
                    medicalLeave => medicalLeave.EmployeeId,
                    employee => employee.Id,
                    (medicalLeave, employee) => new MedicalLeaveDTO
                    {
                        Id = medicalLeave.Id,
                        EmployeeId = medicalLeave.EmployeeId,
                        MedicalDiagnose = medicalLeave.MedicalDiagnose,
                        MedicalRecomendation = medicalLeave.MedicalRecomendation,
                        EmployeeFullName = employee.Name
                    }
                )
                .ToListAsync();

            return medicalLeaveDTOs;
        }

        public async Task<MedicalLeaveDTO> GetById(int id)
        {
            var medicalLeave = await appDbContext.MedicalLeaves
                .AsNoTracking()
                .Where(ml => ml.EmployeeId == id)
                .Select(ml => new MedicalLeaveDTO
                {
                    Id = ml.Id,
                    Date = ml.Date,
                    MedicalDiagnose = ml.MedicalDiagnose,
                    MedicalRecomendation = ml.MedicalRecomendation
                })
                .FirstOrDefaultAsync();

            return medicalLeave;
        }

        public async Task<GeneralResponse> Insert(MedicalLeaveDTO item)
        {
            var medicalLeave = new MedicalLeave
            {
                EmployeeId = item.EmployeeId,
                Date = item.Date,
                MedicalDiagnose = item.MedicalDiagnose,
                MedicalRecomendation = item.MedicalRecomendation,
            };

            appDbContext.MedicalLeaves.Add(medicalLeave);

            await Commit();

            return Success();
        }

        public async Task<GeneralResponse> Update(MedicalLeaveDTO item)
        {
            var medicalLeave = await appDbContext.MedicalLeaves
                .FirstOrDefaultAsync(e => e.Id == item.Id);

            if (medicalLeave == null)
                return NotFound();

            medicalLeave.MedicalRecomendation = item.MedicalRecomendation;
            medicalLeave.MedicalDiagnose = item.MedicalDiagnose;
            medicalLeave.Date = item.Date;

            await Commit();

            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry data not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}

