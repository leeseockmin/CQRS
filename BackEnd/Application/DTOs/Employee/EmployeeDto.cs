namespace BackEnd.Application.DTOs.Employee
{
    public record EmployeeDto(
        int EmployeeId,
        string Name,
        string Email,
        string Tel,
        DateTime Joined
    );
}
