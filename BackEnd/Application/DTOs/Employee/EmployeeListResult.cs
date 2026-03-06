namespace BackEnd.Application.DTOs.Employee
{
    public record EmployeeListResult(
        List<EmployeeDto> Items,
        int TotalCount,
        int Page,
        int PageSize
    );
}
