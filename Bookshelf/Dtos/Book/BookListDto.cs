using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookshelf.Dtos.Book
{
    public class BookListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CommentCount { get; set; }
    }
}