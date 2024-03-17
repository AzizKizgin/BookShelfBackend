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
        Task<Book> AddBook(Book book);
        Task<Book?> UpdateBook(string title, UpdateBookDto book);
        Task<Book?> DeleteBook(int id);
        Task<Book?> GetBook(string title);
    }
}