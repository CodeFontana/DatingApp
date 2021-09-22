using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string Filename { get; set; }

        [Required]
        public bool IsMain { get; set; }

        public AppUser AppUser { get; set; }

        public int AppUserId { get; set; }
    }
}