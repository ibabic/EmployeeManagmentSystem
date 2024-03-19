using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class OvertimeRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<OvertimeDTO>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Overtimes.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.Overtimes.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<OvertimeDTO>> GetAll()
        {
            var overtimeDTOs = await appDbContext.Overtimes
                .AsNoTracking()
                .Include(t => t.OvertimeType)
                .Join(
                    appDbContext.Employees,
                    overtime => overtime.EmployeeId,
                    employee => employee.Id,
                    (overtime, employee) => new OvertimeDTO
                    {
                        Id = overtime.Id,
                        EmployeeId = overtime.EmployeeId,
                        StartDate = overtime.StartDate,
                        EndDate = overtime.EndDate,
                        OvertimeType = overtime.OvertimeType,
                        EmployeeFullName = employee.Name
                    }
                )
                .ToListAsync();

            return overtimeDTOs;
        }

        public async Task<OvertimeDTO> GetById(int id)
        {
            var overtimeDTO = await appDbContext.Overtimes
                .AsNoTracking()
                .Where(overtime => overtime.EmployeeId == id)
                .Select(overtime => new OvertimeDTO
                {
                    Id = overtime.Id,
                    EndDate = overtime.EndDate,
                    StartDate = overtime.StartDate,
                    OvertimeType = overtime.OvertimeType
                })
                .FirstOrDefaultAsync();

            return overtimeDTO;
        }

        public async Task<GeneralResponse> Insert(OvertimeDTO item)
        {
            var overtime = new Overtime
            {
                EmployeeId = item.EmployeeId,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                OvertimeType = item.OvertimeType,
                OvertimeTypeId = item.OvertimeTypeId
            };

            appDbContext.Overtimes.Add(overtime);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(OvertimeDTO item)
        {
            var overtime = await appDbContext.Overtimes.FirstOrDefaultAsync(eid => eid.Id == item.Id);
            if (overtime == null)
            {
                return NotFound();
            }

            overtime.StartDate = item.StartDate;
            overtime.EndDate = item.EndDate;
            overtime.OvertimeTypeId = item.OvertimeTypeId;

            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry data not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
