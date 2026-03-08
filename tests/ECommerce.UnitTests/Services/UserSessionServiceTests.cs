using AutoMapper;
using ECommerce.Application.DTO.UserSession;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Core.Entities.User;
using ECommerce.Core.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace ECommerce.UnitTests.Services;

public class UserSessionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IUserSessionService _userSessionService;

    public UserSessionServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _userSessionService = new UserSessionService(
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetSessionsByUserIdAsync_ShouldReturnMappedSessions()
    {
        var sessions = new List<UserSession> { new() { Id = 1, UserId = "user1" } };
        var sessionDtos = new List<GetUserSessionDTO> { new() { Id = 1, UserId = "user1" } };

        _unitOfWorkMock.Setup(u => u.UserSessionRepository.GetSessionsByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetUserSessionDTO>>(sessions)).Returns(sessionDtos);

        var result = await _userSessionService.GetSessionsByUserIdAsync("user1");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteSessionAsync_WhenExists_ShouldDeleteSession()
    {
        var session = new UserSession { Id = 1 };

        _unitOfWorkMock.Setup(u => u.UserSessionRepository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _userSessionService.DeleteSessionAsync(1);

        _unitOfWorkMock.Verify(u => u.UserSessionRepository.DeleteAsync(session, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSessionAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.UserSessionRepository.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserSession?)null);

        var act = () => _userSessionService.DeleteSessionAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
