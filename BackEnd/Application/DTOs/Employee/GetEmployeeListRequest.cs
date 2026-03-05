namespace BackEnd.Application.DTOs.Employee
{
    public record GetEmployeeListRequest(int Page = 1, int PageSize = 20);
}
