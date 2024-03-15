using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookshelf.Helpers
{
    public class BookQueryObject
    {
        public string? Title { get; set; }
        public int? FromDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}