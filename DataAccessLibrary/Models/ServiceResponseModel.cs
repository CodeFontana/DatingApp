using DataAccessLibrary.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class ServiceResponseModel<T>
    {
        public bool Success { get; set; } = true;
        public T Data { get; set; }
        public string Message { get; set; } = "";
    }
}
