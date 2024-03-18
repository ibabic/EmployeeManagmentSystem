using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class DepartmentRepository(AppDbContext appDbContext, ILogger<DepartmentRepository> logger) : IGenericRepositoryInterface<Department>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var methodName = nameof(DeleteById);
            logger.LogInformation($"[{methodName}] Attempting to delete department with ID: {id}");

            var department = await appDbContext.Departments.FindAsync(id);
            if (department is null)
            {
                logger.LogError($"[{methodName}] Department with ID {id} not found");
                return NotFound();
            }

            appDbContext.Departments.Remove(department);
            await Commit();

            logger.LogInformation($"[{methodName}] Department with ID {id} successfully deleted");
            return Success();
        }

        public async Task<List<Department>> GetAll()
        {
            var methodName = nameof(GetAll);
            logger.LogInformation($"[{methodName}] Attempting to retrieve all departments");

            var departments = await appDbContext.Departments
                .AsNoTracking()
                .Include(gd => gd.GeneralDepartment)
                .ToListAsync();

            logger.LogInformation($"[{methodName}] Successfully retrieved all departments");

            return departments;
        }

        public async Task<Department> GetById(int id)
        {
            var methodName = nameof(GetById);
            logger.LogInformation($"[{methodName}] Attempting to retrieve department with ID: {id}");

            var department = await appDbContext.Departments.FindAsync(id);

            if (department == null)
            {
                logger.LogError($"[{methodName}] Department with ID {id} not found");
            }
            else
            {
                logger.LogInformation($"[{methodName}] Successfully retrieved department with ID: {id}");
            }

            return department;
        }

        public async Task<GeneralResponse> Insert(Department item)
        {
            var methodName = nameof(Insert);
            logger.LogInformation($"[{methodName}] Attempting to insert department with name: {item.Name}");

            if (!await CheckName(item.Name!))
            {
                logger.LogError($"[{methodName}] Department with name {item.Name} already exists");
                return new GeneralResponse(false, "Department already added");
            }

            appDbContext.Departments.Add(item);
            await Commit();

            logger.LogInformation($"[{methodName}] Department with name {item.Name} successfully inserted");
            return Success();
        }

        public async Task<GeneralResponse> Update(Department item)
        {
            var methodName = nameof(Update);
            logger.LogInformation($"[{methodName}] Attempting to update department with ID: {item.Id}");

            var department = await appDbContext.Departments.FindAsync(item.Id);
            if (department is null)
            {
                logger.LogError($"[{methodName}] Department with ID {item.Id} not found");
                return NotFound();
            }

            department.GeneralDepartmentId = item.GeneralDepartmentId;
            department.Name = item.Name;

            await Commit();

            logger.LogInformation($"[{methodName}] Department with ID {item.Id} successfully updated");
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry department not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Departments.FirstOrDefaultAsync(d => d.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
