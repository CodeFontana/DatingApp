using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class UserWithRolesModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}
