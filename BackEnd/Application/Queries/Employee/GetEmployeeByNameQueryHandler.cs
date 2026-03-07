using BackEnd.Application.Constants;
using BackEnd.Application.DTOs.Employee;
using BackEnd.Application.Interfaces.Employee;
using MediatR;

namespace BackEnd.Application.Queries.Employee
{
    public class GetEmployeeByNameQueryHandler : IRequestHandler<GetEmployeeByNameQuery, List<EmployeeDto>>
    {
        private readonly IEmployeeQueryRepository _queryRepository;
        private readonly ILogger<GetEmployeeByNameQueryHandler> _logger;

        public GetEmployeeByNameQueryHandler(
            IEmployeeQueryRepository queryRepository,
            ILogger<GetEmployeeByNameQueryHandler> logger)
        {
            _queryRepository = queryRepository;
            _logger = logger;
        }

        public async Task<List<EmployeeDto>> Handle(GetEmployeeByNameQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogError($"name이 비어있습니다.");
                throw new ArgumentException("name은 필수 입력값입니다.");
            }

            if (request.Name.Length > EmployeeConstants.NameMaxLength)
            {
                _logger.LogError($"name 길이 초과. 입력값 길이: {request.Name.Length}");
                throw new ArgumentException($"name은 최대 {EmployeeConstants.NameMaxLength}자까지 허용됩니다.");
            }

            _logger.LogInformation($"직원 이름 조회 쿼리. Name: {request.Name}");
            return await _queryRepository.GetByNameAsync(request.Name);
        }
    }
}
