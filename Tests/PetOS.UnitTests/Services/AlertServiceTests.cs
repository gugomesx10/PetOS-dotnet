using Moq;
using PetOS.Dto.Alert;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services;

namespace PetOS.UnitTests.Services;

public class AlertServiceTests
{
    [Fact]
    public async Task GetByIdAsync_AlertExists_ReturnsAlertResponseDto()
    {
        // Arrange
        var repositoryMock = new Mock<IAlertRepository>();

        var alert = new Alert
        {
            Id = 1,
            PetId = 1,
            Message = "Vacina próxima do vencimento",
            AlertDate = DateTime.UtcNow,
            IsRead = 0
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(alert);

        var service = new AlertService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.PetId);
        Assert.Equal("Vacina próxima do vencimento", result.Message);

        repositoryMock.Verify(
            repository => repository.GetByIdAsync(1),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_AlertDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repositoryMock = new Mock<IAlertRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(999))
            .ReturnsAsync((Alert?)null);

        var service = new AlertService(repositoryMock.Object);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidAlert_ReturnsCreatedAlertAsUnread()
    {
        // Arrange
        var repositoryMock = new Mock<IAlertRepository>();

        var dto = new AlertCreateDto
        {
            PetId = 1,
            VaccineId = null,
            Message = "Hora do medicamento",
            AlertDate = DateTime.UtcNow
        };

        repositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Alert>()))
            .Callback<Alert>(alert => alert.Id = 10)
            .Returns(Task.CompletedTask);

        var service = new AlertService(repositoryMock.Object);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Equal(10, result.Id);
        Assert.Equal(1, result.PetId);
        Assert.Equal("Hora do medicamento", result.Message);
        Assert.Equal(0, result.IsRead);

        repositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Alert>(alert =>
                    alert.Message == "Hora do medicamento" &&
                    alert.IsRead == 0
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetUnreadAsync_UnreadAlertsExist_ReturnsUnreadAlerts()
    {
        // Arrange
        var repositoryMock = new Mock<IAlertRepository>();

        var alerts = new List<Alert>
        {
            new Alert
            {
                Id = 1,
                PetId = 1,
                Message = "Alerta pendente",
                AlertDate = DateTime.UtcNow,
                IsRead = 0
            }
        };

        repositoryMock
            .Setup(repository => repository.GetUnreadAsync())
            .ReturnsAsync(alerts);

        var service = new AlertService(repositoryMock.Object);

        // Act
        var result = await service.GetUnreadAsync();
        var resultList = result.ToList();

        // Assert
        Assert.Single(resultList);
        Assert.Equal(0, resultList[0].IsRead);
        Assert.Equal("Alerta pendente", resultList[0].Message);

        repositoryMock.Verify(
            repository => repository.GetUnreadAsync(),
            Times.Once
        );
    }
}