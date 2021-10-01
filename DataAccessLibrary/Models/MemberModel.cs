using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class MemberModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string MainPhotoFilename { get; set; }

        public int Age { get; set; }

        [MaxLength(50)]
        public string KnownAs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        [MaxLength(25)]
        public string Gender { get; set; }

        [MaxLength(1000)]
        public string Introduction { get; set; }

        [MaxLength(1000)]
        public string LookingFor { get; set; }

        [MaxLength(1000)]
        public string Interests { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        public IList<PhotoModel> Photos { get; set; }

        public DateTime CacheTime { get; set; }
    }
}
