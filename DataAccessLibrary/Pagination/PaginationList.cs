using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibrary.Pagination
{
    public class PaginationList<T> : List<T>
    {
        public PaginationList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new PaginationModel()
            {
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                PageSize = pageSize,
                TotalCount = count
            };
            
            AddRange(items);
        }

        public PaginationModel MetaData { get; set; }

        public static async Task<PaginationList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = await source.CountAsync();
            
            List<T> items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return new PaginationList<T>(items, count, pageNumber, pageSize);
        }
    }
}
