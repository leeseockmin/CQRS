using BackEnd.Application.Interfaces.Employee;
using DB.Data.AccountDB;
using BackEnd.Infrastructure.DataBase;
using Dapper;

namespace BackEnd.Infrastructure.Repositories
{
    public class EmployeeCommandRepository : IEmployeeCommandRepository
    {
        private readonly DataBaseManager _dbManager;
        private readonly ILogger<EmployeeCommandRepository> _logger;

        public EmployeeCommandRepository(DataBaseManager dbManager, ILogger<EmployeeCommandRepository> logger)
        {
            _dbManager = dbManager;
            _logger = logger;
        }

        public async Task<int> InsertAsync(Employee employee)
        {
            return await _dbManager.ExecuteAsync(DataBaseManager.DBType.Write, async connection =>
            {
                const string sql = $@"
                    INSERT INTO Employee ({nameof(Employee.name)}, {nameof(Employee.email)}, {nameof(Employee.tel)}, {nameof(Employee.joined)}, {nameof(Employee.createdAt)})
                    VALUES (@{nameof(Employee.name)}, @{nameof(Employee.email)}, @{nameof(Employee.tel)}, @{nameof(Employee.joined)}, @{nameof(Employee.createdAt)});
                    SELECT LAST_INSERT_ID();
                    ";

                return await connection.QuerySingleAsync<int>(sql, new
                {
                    employee.name,
                    employee.email,
                    employee.tel,
                    employee.joined,
                    employee.createdAt
                });
            });
        }
    }
}
