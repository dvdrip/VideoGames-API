using System.ComponentModel.DataAnnotations;

namespace VideoGames_API.Models;

public sealed class GameRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1000, 9999)]
    public short? ReleaseYear { get; set; }

    [StringLength(100)]
    public string? Genre { get; set; }
}
