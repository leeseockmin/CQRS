
namespace BackEnd.Application.Interfaces.Employee
{
    public interface IEmployeeCommandRepository
    {
        Task<int> InsertAsync(DB.Data.AccountDB.Employee employee);
    }
}
