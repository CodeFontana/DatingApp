namespace DataAccessLibrary.Entities;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }

    [MaxLength(500)]
    public string Filename { get; set; } = string.Empty;

    [Required]
    public bool IsMain { get; set; }

    [ForeignKey(nameof(AppUserId))]
    public AppUser AppUser { get; set; } = null!;

    public int AppUserId { get; set; }
}