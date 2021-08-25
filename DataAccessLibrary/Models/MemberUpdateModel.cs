using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class MemberUpdateModel
    {
        [MaxLength(1000)]
        public string Introduction { get; set; }

        [MaxLength(1000)]
        public string LookingFor { get; set; }

        [MaxLength(1000)]
        public string Interests { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }
    }
}
