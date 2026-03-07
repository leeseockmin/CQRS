using BackEnd.Application.DTOs.Employee;
using BackEnd.Application.Interfaces.Employee;
using BackEnd.Application.Queries.Employee;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BackEnd.Tests.Queries
{
    /// <summary>
    /// GetEmployeeByNameQueryHandler 단위 테스트
    /// </summary>
    public class GetEmployeeByNameQueryHandlerTests
    {
        private readonly Mock<IEmployeeQueryRepository> _mockQueryRepository;
        private readonly Mock<ILogger<GetEmployeeByNameQueryHandler>> _mockLogger;
        private readonly GetEmployeeByNameQueryHandler _handler;

        public GetEmployeeByNameQueryHandlerTests()
        {
            _mockQueryRepository = new Mock<IEmployeeQueryRepository>();
            _mockLogger = new Mock<ILogger<GetEmployeeByNameQueryHandler>>();
            _handler = new GetEmployeeByNameQueryHandler(
                _mockQueryRepository.Object,
                _mockLogger.Object);
        }

        // =============================================
        // 성공 케이스
        // =============================================

        /// <summary>
        /// [성공] 이름 검색 — 일치하는 직원 리스트를 반환한다.
        /// </summary>
        [Fact]
        public async Task Handle_ExistingName_ReturnsEmployeeDtoList()
        {
            // Arrange
            var name = "홍길동";
            var expectedDto = new EmployeeDto(
                EmployeeId: 1,
                Name: "홍길동",
                Email: "hong@example.com",
                Tel: "01012345678",
                Joined: new DateTime(2024, 1, 1));

            var query = new GetEmployeeByNameQuery(name);

            _mockQueryRepository
                .Setup(r => r.GetByNameAsync(name))
                .ReturnsAsync(new List<EmployeeDto> { expectedDto });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].EmployeeId);
            Assert.Equal("홍길동", result[0].Name);
            Assert.Equal("hong@example.com", result[0].Email);
            Assert.Equal("01012345678", result[0].Tel);
        }

        /// <summary>
        /// [성공] 동명이인 검색 — 이름이 같은 직원이 여러 명이면 모두 반환된다.
        /// </summary>
        [Fact]
        public async Task Handle_DuplicateNames_ReturnsAllMatching()
        {
            // Arrange
            var name = "홍길동";
            var query = new GetEmployeeByNameQuery(name);

            var expectedList = new List<EmployeeDto>
            {
                new EmployeeDto(EmployeeId: 1, Name: "홍길동", Email: "hong1@example.com", Tel: "01011111111", Joined: new DateTime(2023, 1, 1)),
                new EmployeeDto(EmployeeId: 2, Name: "홍길동", Email: "hong2@example.com", Tel: "01022222222", Joined: new DateTime(2024, 1, 1))
            };

            _mockQueryRepository
                .Setup(r => r.GetByNameAsync(name))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].EmployeeId);
            Assert.Equal(2, result[1].EmployeeId);
        }

        /// <summary>
        /// [성공] 부분 이름 검색 — 검색어를 포함하는 직원이 여러 명이면 모두 반환된다.
        /// </summary>
        [Fact]
        public async Task Handle_PartialName_ReturnsAllMatching()
        {
            // Arrange
            var name = "홍";
            var query = new GetEmployeeByNameQuery(name);

            var expectedList = new List<EmployeeDto>
            {
                new EmployeeDto(EmployeeId: 1, Name: "홍길동", Email: "hong1@example.com", Tel: "01011111111", Joined: new DateTime(2023, 1, 1)),
                new EmployeeDto(EmployeeId: 3, Name: "홍범도", Email: "hong3@example.com", Tel: "01033333333", Joined: new DateTime(2022, 5, 10))
            };

            _mockQueryRepository
                .Setup(r => r.GetByNameAsync(name))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Name == "홍길동");
            Assert.Contains(result, e => e.Name == "홍범도");
        }

        /// <summary>
        /// [성공] 존재하지 않는 이름 검색 — 예외 없이 빈 리스트를 반환한다.
        /// </summary>
        [Fact]
        public async Task Handle_NonExistentName_ReturnsEmptyList()
        {
            // Arrange
            var name = "존재하지않는사람";
            var query = new GetEmployeeByNameQuery(name);

            _mockQueryRepository
                .Setup(r => r.GetByNameAsync(name))
                .ReturnsAsync(new List<EmployeeDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// [성공] 이름 조회 시 파라미터 전달 검증 — 요청한 이름과 동일한 값이 GetByNameAsync에 전달된다.
        /// </summary>
        [Fact]
        public async Task Handle_NameQuery_PassesSameNameToGetByNameAsync()
        {
            // Arrange
            var name = "이순신";
            var query = new GetEmployeeByNameQuery(name);

            _mockQueryRepository
                .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<EmployeeDto>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert — 요청한 이름이 Repository에 정확히 전달되어야 합니다.
            _mockQueryRepository.Verify(
                r => r.GetByNameAsync(name),
                Times.Once);
        }

        /// <summary>
        /// [성공] 조회 성공 시 호출 횟수 검증 — GetByNameAsync가 정확히 1회만 호출된다.
        /// </summary>
        [Fact]
        public async Task Handle_SuccessfulQuery_CallsGetByNameAsyncOnce()
        {
            // Arrange
            var name = "강감찬";
            var query = new GetEmployeeByNameQuery(name);

            _mockQueryRepository
                .Setup(r => r.GetByNameAsync(name))
                .ReturnsAsync(new List<EmployeeDto>
                {
                    new EmployeeDto(
                        EmployeeId: 5,
                        Name: "강감찬",
                        Email: "kang@example.com",
                        Tel: "01099999999",
                        Joined: new DateTime(2023, 6, 15))
                });

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockQueryRepository.Verify(
                r => r.GetByNameAsync(It.IsAny<string>()),
                Times.Once);
        }

        /// <summary>
        /// [성공] 반환된 EmployeeDto 리스트의 필드 정확성 검증 — 모든 필드가 정확히 반환된다.
        /// </summary>
        [Fact]
        public async Task Handle_ReturnedEmployeeDtoList_AllFieldsReturnedAccurately()
        {
            // Arrange
            var joined = new DateTime(2025, 3, 1);
            var expectedDto = new EmployeeDto(
                EmployeeId: 42,
                Name: "테스트직원",
                Email: "test@company.co.kr",
                Tel: "01055556666",
                Joined: joined);

            var query = new GetEmployeeByNameQuery("테스트직원");

            _mockQueryRepository
                .Setup(r => r.GetByNameAsync("테스트직원"))
                .ReturnsAsync(new List<EmployeeDto> { expectedDto });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(42, result[0].EmployeeId);
            Assert.Equal("테스트직원", result[0].Name);
            Assert.Equal("test@company.co.kr", result[0].Email);
            Assert.Equal("01055556666", result[0].Tel);
            Assert.Equal(joined, result[0].Joined);
        }

        // =============================================
        // 실패 케이스
        // =============================================

        /// <summary>
        /// [실패] name이 null — ArgumentException이 발생한다.
        /// </summary>
        [Fact]
        public async Task Handle_NameIsNull_ThrowsArgumentException()
        {
            // Arrange
            var query = new GetEmployeeByNameQuery(null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("name은 필수 입력값입니다.", ex.Message);
            _mockQueryRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// [실패] name이 빈 문자열 — ArgumentException이 발생한다.
        /// </summary>
        [Fact]
        public async Task Handle_NameIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            var query = new GetEmployeeByNameQuery(string.Empty);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("name은 필수 입력값입니다.", ex.Message);
            _mockQueryRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// [실패] name이 공백 문자열 — ArgumentException이 발생한다.
        /// </summary>
        [Fact]
        public async Task Handle_NameIsWhitespace_ThrowsArgumentException()
        {
            // Arrange
            var query = new GetEmployeeByNameQuery("   ");

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("name은 필수 입력값입니다.", ex.Message);
            _mockQueryRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// [실패] name이 최대 길이(64자) 초과 — ArgumentException이 발생한다.
        /// </summary>
        [Fact]
        public async Task Handle_NameExceedsMaxLength_ThrowsArgumentException()
        {
            // Arrange
            var longName = new string('가', 65);
            var query = new GetEmployeeByNameQuery(longName);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("name은 최대 64자까지 허용됩니다.", ex.Message);
            _mockQueryRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// [성공] name이 정확히 최대 길이(64자) — 예외 없이 정상 처리된다.
        /// </summary>
        [Fact]
        public async Task Handle_NameExactlyMaxLength_ProcessesSuccessfully()
        {
            // Arrange
            var maxName = new string('가', 64);
            var query = new GetEmployeeByNameQuery(maxName);

            _mockQueryRepository
                .Setup(r => r.GetByNameAsync(maxName))
                .ReturnsAsync(new List<EmployeeDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockQueryRepository.Verify(r => r.GetByNameAsync(maxName), Times.Once);
        }
    }
}
