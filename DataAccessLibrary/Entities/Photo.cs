using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }

        [MaxLength(1000)]
        public string Url { get; set; }

        [Required]
        public bool IsMain { get; set; }

        [MaxLength(50)]
        public string PublicId { get; set; }

        public AppUser AppUser { get; set; }

        public int AppUserId { get; set; }
    }
}