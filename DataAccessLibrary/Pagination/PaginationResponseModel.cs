using DataAccessLibrary.Models;

namespace DataAccessLibrary.Pagination
{
    public class PaginationResponseModel<T> : ServiceResponseModel<T>
    {
        public PaginationModel MetaData { get; set; }
    }
}
