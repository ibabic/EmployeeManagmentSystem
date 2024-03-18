using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class EmployeeRepository(AppDbContext appDbContext, ILogger<EmployeeRepository> logger) : IGenericRepositoryInterface<Employee>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var methodName = nameof(DeleteById);
            logger.LogInformation($"[{methodName}] Attempting to delete employee with ID: {id}");

            var employee = await appDbContext.Employees.FindAsync(id);
            if (employee is null)
            {
                logger.LogError($"[{methodName}] Employee with ID {id} not found");
                return NotFound();
            }

            appDbContext.Employees.Remove(employee);
            await Commit();

            logger.LogInformation($"[{methodName}] Employee with ID {id} successfully deleted");
            return Success();
        }

        public async Task<List<Employee>> GetAll()
        {
            var methodName = nameof(GetAll);
            logger.LogInformation($"[{methodName}] Attempting to retrieve all employees");

            var employees = await appDbContext.Employees
                .AsNoTracking()
                .Include(c => c.City)
                    .ThenInclude(co => co.Country)
                .Include(b => b.Branch)
                    .ThenInclude(d => d.Department)
                        .ThenInclude(gd => gd.GeneralDepartment)
                .ToListAsync();

            logger.LogInformation($"[{methodName}] Successfully retrieved all employees");
            return employees;
        }

        public async Task<GeneralResponse> Insert(Employee item)
        {
            var methodName = nameof(Insert);
            logger.LogInformation($"[{methodName}] Attempting to insert a new employee");

            if (!await CheckName(item.Name!))
            {
                logger.LogError($"[{methodName}] Employee with name {item.Name} already exists");
                return new GeneralResponse(false, "Employee already added");
            }

            var addedEmployee = appDbContext.Employees.Add(item);
            await Commit();

            logger.LogInformation($"[{methodName}] Employee {item.Name} successfully added");
            return Success();
        }

        public async Task<GeneralResponse> Update(Employee employee)
        {
            var methodName = nameof(Update);
            logger.LogInformation($"[{methodName}] Attempting to update employee with ID: {employee.Id}");

            var findUser = await appDbContext.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id);
            if (findUser is null)
            {
                logger.LogError($"[{methodName}] Employee with ID {employee.Id} does not exist");
                return new GeneralResponse(false, "Employee does not exist");
            }

            findUser.Name = employee.Name;
            findUser.Other = employee.Other;
            findUser.Address = employee.Address;
            findUser.TelephoneNumber = employee.TelephoneNumber;
            findUser.BranchId = employee.BranchId;
            findUser.CivilId = employee.CivilId;
            findUser.FileNumber = employee.FileNumber;
            findUser.JobName = employee.JobName;
            findUser.Photo = employee.Photo;

            await appDbContext.SaveChangesAsync();
            await Commit();

            logger.LogInformation($"[{methodName}] Employee with ID {employee.Id} successfully updated");
            return Success();
        }

        public async Task<Employee> GetById(int id)
        {
            var methodName = nameof(GetById);
            logger.LogInformation($"[{methodName}] Attempting to retrieve employee with ID: {id}");

            var employee = await appDbContext.Employees
                .AsNoTracking()
                .Include(c => c.City)
                    .ThenInclude(co => co.Country)
                .Include(b => b.Branch)
                    .ThenInclude(d => d.Department)
                        .ThenInclude(gd => gd.GeneralDepartment)
                .FirstOrDefaultAsync(ei => ei.Id == id);

            if (employee == null)
            {
                logger.LogError($"[{methodName}] Employee with ID {id} not found");
                return null;
            }

            logger.LogInformation($"[{methodName}] Successfully retrieved employee with ID: {id}");
            return employee;
        }


        private static GeneralResponse NotFound() => new(false, "Sorry Country not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Employees.FirstOrDefaultAsync(e => e.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
