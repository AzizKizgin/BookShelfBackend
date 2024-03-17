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
        public static Book BookFromCreateBookDto(this CreateBookDto createBookDto)
        {
            return new Book
            {
                Title = createBookDto.Title,
                Comments = new List<Comment>
                {
                    new Comment
                    {
                        Content = createBookDto.Comment
                    }
                }
            };
        }

        public static Book BookFromUpdateBookDto(this UpdateBookDto createBookDto)
        {
            return new Book
            {
                Title = createBookDto.Title
            };
        }

        public static BookListDto BookListDtoFromBook(this Book book)
        {
            return new BookListDto
            {
                Id = book.Id,
                Title = book.Title,
                CommentCount = book.Comments.Count
            };
        }
    }
}