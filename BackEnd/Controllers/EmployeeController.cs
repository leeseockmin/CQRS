using BackEnd.Application.Commands.Employee;
using BackEnd.Application.Constants;
using BackEnd.Application.DTOs.Common;
using BackEnd.Application.DTOs.Employee;
using BackEnd.Application.Queries.Employee;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IMediator mediator, ILogger<EmployeeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>이름으로 직원 상세 연락정보를 조회합니다.</summary>
        /// <remarks>GET /api/employee/search?name={name}</remarks>
        [HttpGet("search")]
        [ProducesResponseType(typeof(GetEmployeeByNameResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeByNameAsync([FromQuery] GetEmployeeByNameRequest request)
        {
            try
            {
                var result = await _mediator.Send(new GetEmployeeByNameQuery(request.Name));
                return Ok(new GetEmployeeByNameResult(result));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"직원 이름 조회 유효성 검사 실패. Message: {ex.Message}");
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>직원 목록을 페이지 단위로 조회합니다.</summary>
        /// <remarks>GET /api/employee/list?page={page}&amp;pageSize={pageSize}</remarks>
        [HttpGet("list")]
        [ProducesResponseType(typeof(EmployeeListResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEmployeeListAsync([FromQuery] GetEmployeeListRequest request)
        {
            if (request.Page < EmployeeConstants.PageMinValue || request.PageSize < EmployeeConstants.PageMinValue)
            {
                _logger.LogError($"잘못된 페이지 파라미터. Page: {request.Page}, PageSize: {request.PageSize}");
                return BadRequest(new ErrorResponse("page와 pageSize는 1 이상이어야 합니다."));
            }

            var result = await _mediator.Send(new GetEmployeeListQuery(request.Page, request.PageSize));
            return Ok(result);
        }

        /// <summary>직원 목록을 일괄 등록합니다.</summary>
        /// <remarks>POST /api/employee/create</remarks>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEmployeeAsync([FromBody] List<CreateEmployeeRequest> request)
        {
            try
            {
                await _mediator.Send(new CreateEmployeeCommand(request));
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"직원 일괄 등록 유효성 검사 실패. Message: {ex.Message}");
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }
    }
}
