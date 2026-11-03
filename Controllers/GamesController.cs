using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using VideoGames_API.Data;
using VideoGames_API.Models;

namespace VideoGames_API.Controllers;

[ApiController]
[Route("api/games")]
public sealed class GamesController(IGameRepository repository, ILogger<GamesController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GameResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GameResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await repository.GetAllAsync(cancellationToken));
        }
        catch (SqlException exception)
        {
            return HandleDatabaseError(exception);
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var game = await repository.GetByIdAsync(id, cancellationToken);
            return game is null ? NotFound() : Ok(game);
        }
        catch (SqlException exception)
        {
            return HandleDatabaseError(exception);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameResponse>> Create(GameRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = await repository.CreateAsync(request, cancellationToken);
            var game = await repository.GetByIdAsync(gameId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = gameId }, game);
        }
        catch (SqlException exception)
        {
            return HandleDatabaseError(exception);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameResponse>> Update(int id, GameRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!await repository.UpdateAsync(id, request, cancellationToken))
            {
                return NotFound();
            }

            return Ok(await repository.GetByIdAsync(id, cancellationToken));
        }
        catch (SqlException exception)
        {
            return HandleDatabaseError(exception);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
        }
        catch (SqlException exception)
        {
            return HandleDatabaseError(exception);
        }
    }

    private ObjectResult HandleDatabaseError(SqlException exception)
    {
        logger.LogError(exception, "A database error occurred while processing a games request.");
        return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
        {
            Title = "Database error",
            Detail = "The request could not be completed because of a database error."
        });
    }
}
