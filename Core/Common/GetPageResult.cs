using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Core.Common
{
    public  class GetPageResult<T>
    {
        public int pageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public int TotalItems { get; set; } 
        public List<T> PageResult { get; set; } = new List<T>();


        public static async Task<GetPageResult<T>>GetPageAsync<T>(IQueryable<T>query,int pageNumber,int pageSize)
        {
            var count= await query.CountAsync();

            var queryResult =await  query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new GetPageResult<T>
            {
                pageSize = pageSize,
                TotalItems = count,
                CurrentPage = pageNumber,
                PageResult = queryResult
            };
        }

    }
}
