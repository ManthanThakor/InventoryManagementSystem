using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels.common
{
    public class BaseResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class PaginatedResponseViewModel<T> : BaseResponseViewModel
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
