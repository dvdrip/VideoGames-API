using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using VideoGames_API.Controllers;
using VideoGames_API.Data;
using VideoGames_API.Models;
using Xunit;

namespace VideoGames_API.Tests.Controllers;

public sealed class GamesControllerTests
{
    private static readonly GameResponse Minecraft = new()
    {
        GameId = 1,
        Title = "Minecraft",
        ReleaseYear = 2011,
        Genre = "Sandbox"
    };

    [Fact]
    public async Task GetAll_ReturnsOkWithGames()
    {
        var repository = CreateRepository();
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { Minecraft });
        var controller = CreateController(repository);

        var result = await controller.GetAll(CancellationToken.None);

        var response = Assert.IsType<OkObjectResult>(result.Result);
        var games = Assert.IsAssignableFrom<IReadOnlyList<GameResponse>>(response.Value);
        Assert.Single(games);
        Assert.Equal("Minecraft", games[0].Title);
    }

    [Fact]
    public async Task GetById_WhenGameExists_ReturnsOkWithGame()
    {
        var repository = CreateRepository();
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Minecraft);
        var controller = CreateController(repository);

        var result = await controller.GetById(1, CancellationToken.None);

        var response = Assert.IsType<OkObjectResult>(result.Result);
        var game = Assert.IsType<GameResponse>(response.Value);
        Assert.Equal(1, game.GameId);
    }

    [Fact]
    public async Task GetById_WhenGameDoesNotExist_ReturnsNotFound()
    {
        var repository = CreateRepository();
        repository.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GameResponse?)null);
        var controller = CreateController(repository);

        var result = await controller.GetById(99, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionWithCreatedGame()
    {
        var request = new GameRequest { Title = "Minecraft", ReleaseYear = 2011, Genre = "Sandbox" };
        var repository = CreateRepository();
        repository.Setup(x => x.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Minecraft);
        var controller = CreateController(repository);

        var result = await controller.Create(request, CancellationToken.None);

        var response = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(GamesController.GetById), response.ActionName);
        Assert.Same(Minecraft, response.Value);
    }

    [Fact]
    public async Task Update_WhenGameExists_ReturnsOkWithUpdatedGame()
    {
        var request = new GameRequest { Title = "Minecraft", ReleaseYear = 2011, Genre = "Sandbox" };
        var repository = CreateRepository();
        repository.Setup(x => x.UpdateAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Minecraft);
        var controller = CreateController(repository);

        var result = await controller.Update(1, request, CancellationToken.None);

        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(Minecraft, response.Value);
    }

    [Fact]
    public async Task Update_WhenGameDoesNotExist_ReturnsNotFound()
    {
        var request = new GameRequest { Title = "Minecraft" };
        var repository = CreateRepository();
        repository.Setup(x => x.UpdateAsync(99, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var controller = CreateController(repository);

        var result = await controller.Update(99, request, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_WhenGameExists_ReturnsNoContent()
    {
        var repository = CreateRepository();
        repository.Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var controller = CreateController(repository);

        var result = await controller.Delete(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenGameDoesNotExist_ReturnsNotFound()
    {
        var repository = CreateRepository();
        repository.Setup(x => x.DeleteAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var controller = CreateController(repository);

        var result = await controller.Delete(99, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    private static Mock<IGameRepository> CreateRepository() => new();

    private static GamesController CreateController(Mock<IGameRepository> repository) =>
        new(repository.Object, NullLogger<GamesController>.Instance);
}
