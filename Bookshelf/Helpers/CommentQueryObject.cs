using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookshelf.Helpers
{
    public class CommentQueryObject
    {
        public string? BookTitle { get; set; }
        public string? Content { get; set; }
        public bool OrderByFavorite { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}