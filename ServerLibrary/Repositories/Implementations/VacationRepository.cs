using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class VacationRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<VacationDTO>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Vacations.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.Vacations.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<VacationDTO>> GetAll()
        {
            var vacationDTOs = await appDbContext.Vacations
                .AsNoTracking()
                .Include(t => t.VacationType)
                .Join(
                    appDbContext.Employees,
                    vacation => vacation.EmployeeId,
                    employee => employee.Id,
                    (vacation, employee) => new VacationDTO
                    {
                        Id = vacation.Id,
                        EmployeeId = vacation.EmployeeId,
                        StartDate = vacation.StartDate,
                        EndDate = vacation.EndDate,
                        VacationTypeId = vacation.VacationTypeId,
                        VacationType = vacation.VacationType,
                        EmployeeFullName = employee.Name
                    }
                )
                .ToListAsync();

            return vacationDTOs;
        }

        public async Task<VacationDTO> GetById(int id)
        {
            var vacationDTO = await appDbContext.Vacations
                .AsNoTracking()
                .Where(vacation => vacation.EmployeeId == id)
                .Select(vacation => new VacationDTO
                {
                    Id = vacation.Id,
                    StartDate = vacation.StartDate,
                    EndDate = vacation.EndDate,
                    VacationType = vacation.VacationType,
                    VacationTypeId = vacation.VacationTypeId
                })
                .FirstOrDefaultAsync();

            return vacationDTO;
        }

        public async Task<GeneralResponse> Insert(VacationDTO item)
        {
            var vacation = new Vacation
            {
                EmployeeId = item.EmployeeId,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                VacationType = item.VacationType,
                VacationTypeId = item.VacationTypeId
            };

            appDbContext.Vacations.Add(vacation);

            await Commit();

            return Success();
        }

        public async Task<GeneralResponse> Update(VacationDTO item)
        {
            var obj = await appDbContext.Vacations
                .FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();
            obj.StartDate = item.StartDate;
            obj.EndDate = item.EndDate;
            obj.VacationTypeId = item.VacationTypeId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry data not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
