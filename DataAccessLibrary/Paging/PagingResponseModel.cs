using DataAccessLibrary.Models;

namespace DataAccessLibrary.Paging
{
    public class PagingResponseModel<T> : ServiceResponseModel<T>
    {
        public PageModel MetaData { get; set; }
    }
}
