using ChargingStation.Common.Models.Depots.Requests;
using ChargingStation.Common.Models.Depots.Responses;
using ChargingStation.Common.Models.General.Requests;
using ChargingStation.Common.Models.General;
using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Repositories;
using Depots.Application.Services;
using Depots.Application.Specifications;
using TimeZone = ChargingStation.Domain.Entities.TimeZone;
using ChargingStation.Common.Enums;
using Depots.Application.Models.Requests;
using ChargingStation.Common.Models.TimeZone;
using ChargingStation.InternalCommunication.Services.UserManagement;

namespace Depots.UnitTests;

public class DepotServiceUnitTests
{
    private readonly Mock<IRepository<Depot>> _depotRepositoryMock;
    private readonly Mock<IRepository<TimeZone>> _timeZoneRepositoryMock;
    private readonly Mock<IUserHttpService> _userHttpServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DepotService _depotService;

    public DepotServiceUnitTests()
    {
        _depotRepositoryMock = new Mock<IRepository<Depot>>();
        _timeZoneRepositoryMock = new Mock<IRepository<TimeZone>>();
        _userHttpServiceMock = new Mock<IUserHttpService>();
        _mapperMock = new Mock<IMapper>();
        _depotService = new DepotService(_depotRepositoryMock.Object, _timeZoneRepositoryMock.Object, _mapperMock.Object, _userHttpServiceMock.Object);
    }

    private Depot CreateDepot()
    {
        return new Depot
        {
            Name = "Depot 1",
            Country = "Country 1",
            City = "City 1",
            Street = "Street 1",
            Building = "Building 1",
            TimeZoneId = Guid.NewGuid(),
            Status = DepotStatus.Available,
            CreatedAt = DateTime.UtcNow
        };
    }

    private DepotResponse CreateDepotResponse()
    {
        return new DepotResponse
        {
            Name = "Depot 1",
            Country = "Country 1",
            City = "City 1",
            Street = "Street 1",
            Building = "Building 1",
            BaseUtcOffset = TimeSpan.Zero,
            IanaId = "IanaId",
            Status = DepotStatus.Available,
            CreatedAt = DateTime.UtcNow
        };
    }

    private TimeZoneResponse CreateTimeZoneResponse()
    {
        return new TimeZoneResponse
        {
            DisplayName = "TimeZone 1",
            BaseUtcOffset = TimeSpan.Zero,
            IanaId = "IanaId",
            WindowsId = "WindowsId"
        };
    }

    [Fact]
    public async Task GetAsync_ShouldReturnPagedCollection_WhenDepotsExist()
    {
        // Arrange
        var request = new GetDepotsRequest { PagePredicate = new PagePredicate { Page = 1, PageSize = 10 } };
        var depots = new PagedCollection<Depot>(new List<Depot> { CreateDepot() }, 1, 10, 1);
        var depotsResponse = new PagedCollection<DepotResponse>(new List<DepotResponse> { CreateDepotResponse() }, 1, 10, 1);

        _depotRepositoryMock.Setup(repo => repo.GetPagedCollectionAsync(It.IsAny<GetDepotsSpecification>(), It.IsAny<int?>(), It.IsAny<int?>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(depots);

        _mapperMock.Setup(mapper => mapper.Map<IPagedCollection<DepotResponse>>(depots))
            .Returns(depotsResponse);

        // Act
        var result = await _depotService.GetAsync(request, Guid.Empty, false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(depotsResponse, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDepotResponse_WhenDepotExists()
    {
        // Arrange
        var depotId = Guid.NewGuid();
        var depot = CreateDepot();
        var depotResponse = CreateDepotResponse();

        _depotRepositoryMock.Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<GetDepotSpecification>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(depot);

        _mapperMock.Setup(mapper => mapper.Map<DepotResponse>(depot))
            .Returns(depotResponse);

        // Act
        var result = await _depotService.GetByIdAsync(depotId, Guid.Empty, false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(depotResponse, result);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDepotResponse_WhenDepotCreated()
    {
        // Arrange
        var request = new CreateDepotRequest
        {
            Name = "New Depot",
            Country = "Country 1",
            City = "City 1",
            Street = "Street 1",
            Building = "Building 1",
            TimeZoneId = Guid.NewGuid()
        };
        var depot = CreateDepot();
        var depotResponse = CreateDepotResponse();

        _depotRepositoryMock.Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<GetDepotSpecification>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Depot)null);

        _mapperMock.Setup(mapper => mapper.Map<Depot>(request))
            .Returns(depot);

        _depotRepositoryMock.Setup(repo => repo.AddAsync(depot, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _depotRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _mapperMock.Setup(mapper => mapper.Map<DepotResponse>(depot))
            .Returns(depotResponse);

        // Act
        var result = await _depotService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(depotResponse, result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnDepotResponse_WhenDepotUpdated()
    {
        // Arrange
        var request = new UpdateDepotRequest
        {
            Id = Guid.NewGuid(),
            Name = "Updated Depot",
            Country = "Country 1",
            City = "City 1",
            Street = "Street 1",
            Building = "Building 1",
            TimeZoneId = Guid.NewGuid()
        };
        var depot = CreateDepot();
        var depotResponse = CreateDepotResponse();

        _depotRepositoryMock.Setup(repo => repo.GetByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(depot);

        _mapperMock.Setup(mapper => mapper.Map(request, depot)).Returns(depot);

        _depotRepositoryMock.Setup(repo => repo.Update(depot));
        _depotRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _mapperMock.Setup(mapper => mapper.Map<DepotResponse>(depot)).Returns(depotResponse);

        // Act
        var result = await _depotService.UpdateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(depotResponse, result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRemove_WhenDepotExists()
    {
        // Arrange
        var depotId = Guid.NewGuid();
        var depot = CreateDepot();

        _depotRepositoryMock.Setup(repo => repo.GetByIdAsync(depotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(depot);

        _depotRepositoryMock.Setup(repo => repo.Remove(depot));
        _depotRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _depotService.DeleteAsync(depotId);

        // Assert
        _depotRepositoryMock.Verify(repo => repo.Remove(depot), Times.Once);
        _depotRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTimeZonesAsync_ShouldReturnPagedCollection_WhenTimeZonesExist()
    {
        // Arrange
        var request = new GetTimeZoneRequest { PagePredicate = new PagePredicate { Page = 1, PageSize = 10 } };
        var timeZones = new PagedCollection<TimeZone>(new List<TimeZone>
            {
                new TimeZone
                {
                    DisplayName = "TimeZone 1",
                    BaseUtcOffset = TimeSpan.Zero,
                    IanaId = "IanaId",
                    WindowsId = "WindowsId"
                }
            }, 1, 10, 1);
        var timeZonesResponse = new PagedCollection<TimeZoneResponse>(new List<TimeZoneResponse>
            {
                CreateTimeZoneResponse()
            }, 1, 10, 1);

        _timeZoneRepositoryMock.Setup(repo => repo.GetPagedCollectionAsync(It.IsAny<GetTimeZonesSpecification>(), It.IsAny<int?>(), It.IsAny<int?>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeZones);

        _mapperMock.Setup(mapper => mapper.Map<IPagedCollection<TimeZoneResponse>>(timeZones))
            .Returns(timeZonesResponse);

        // Act
        var result = await _depotService.GetTimeZonesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(timeZonesResponse, result);
    }
}