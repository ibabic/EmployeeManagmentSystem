using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class GeneralDepartmentRepository(AppDbContext appDbContext, ILogger<GeneralDepartmentRepository> logger) : IGenericRepositoryInterface<GeneralDepartment>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var methodName = nameof(DeleteById);
            logger.LogInformation($"[{methodName}] Attempting to delete general department with ID: {id}");

            var department = await appDbContext.GeneralDepartments.FindAsync(id);
            if (department is null)
            {
                logger.LogError($"[{methodName}] General department with ID {id} not found");
                return NotFound();
            }

            appDbContext.GeneralDepartments.Remove(department);
            await Commit();

            logger.LogInformation($"[{methodName}] General department with ID {id} successfully deleted");
            return Success();
        }

        public async Task<List<GeneralDepartment>> GetAll()
        {
            var methodName = nameof(GetAll);
            logger.LogInformation($"[{methodName}] Attempting to retrieve all general departments");

            var generalDepartments = await appDbContext.GeneralDepartments.ToListAsync();

            logger.LogInformation($"[{methodName}] Successfully retrieved all general departments");
            return generalDepartments;
        }

        public async Task<GeneralDepartment> GetById(int id)
        {
            var methodName = nameof(GetById);
            logger.LogInformation($"[{methodName}] Attempting to retrieve general department with ID: {id}");

            var generalDepartment = await appDbContext.GeneralDepartments.FindAsync(id);

            if (generalDepartment == null)
            {
                logger.LogError($"[{methodName}] General department with ID {id} not found");
            }
            else
            {
                logger.LogInformation($"[{methodName}] Successfully retrieved general department with ID: {id}");
            }

            return generalDepartment;
        }

        public async Task<GeneralResponse> Insert(GeneralDepartment item)
        {
            var methodName = nameof(Insert);
            logger.LogInformation($"[{methodName}] Attempting to insert general department: {item.Name}");

            if (!await CheckName(item.Name!))
            {
                logger.LogError($"[{methodName}] Department with name '{item.Name}' already added");
                return new GeneralResponse(false, "Department already added");
            }

            appDbContext.GeneralDepartments.Add(item);
            await Commit();

            logger.LogInformation($"[{methodName}] General department '{item.Name}' successfully inserted");
            return Success();
        }

        public async Task<GeneralResponse> Update(GeneralDepartment item)
        {
            var methodName = nameof(Update);
            logger.LogInformation($"[{methodName}] Attempting to update general department with ID: {item.Id}");

            var department = await appDbContext.GeneralDepartments.FindAsync(item.Id);
            if (department is null)
            {
                logger.LogError($"[{methodName}] General department with ID {item.Id} not found");
                return NotFound();
            }

            department.Name = item.Name;
            await Commit();

            logger.LogInformation($"[{methodName}] General department with ID {item.Id} successfully updated");
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry department not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.GeneralDepartments.FirstOrDefaultAsync(d => d.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
