using DataAccessLibrary.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class PagingResponseModel<T> : ServiceResponseModel<T>
    {
        public PageData MetaData { get; set; }
    }
}
