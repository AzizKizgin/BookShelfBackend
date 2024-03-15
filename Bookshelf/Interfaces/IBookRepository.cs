using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Dtos.Book;
using Bookshelf.Helpers;
using Bookshelf.Models;

namespace Bookshelf.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooks(BookQueryObject queryObject);
        Task<Book?> GetBook(int id);
        Task<Book> AddBook(BookDto book);
        Task<Book?> UpdateBook(Book book);
        Task<Book?> DeleteBook(int id);
    }
}