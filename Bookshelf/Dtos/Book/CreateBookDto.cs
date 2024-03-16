using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Dtos.Comment;
using Bookshelf.Models;

namespace Bookshelf.Dtos.Book
{
    public class CreateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}