using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Dtos.Book;
using Bookshelf.Models;

namespace Bookshelf.Mappers
{
    public static class BookMapper
    {
        public static Book BookFromDto(this BookDto bookDto)
        {
            return new Book
            {
                Title = bookDto.Title
            };
        }
    }
}