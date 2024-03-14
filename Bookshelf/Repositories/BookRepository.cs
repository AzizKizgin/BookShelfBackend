using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Data;
using Bookshelf.Dtos.Book;
using Bookshelf.Interfaces;
using Bookshelf.Mappers;
using Bookshelf.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Repositories
{
    public class BookRepository: IBookRepository
    {
        private readonly ApplicationDBContext _context;

        public BookRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Book> AddBook(BookDto book)
        {
            var result = await _context.Books.AddAsync(book.BookFromDto());
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Book?> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }
            if (book.Comments != null)
            {
                throw new InvalidOperationException("Cannot delete a book with comments");
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> GetBook(int id)
        {
            var book = await _context.Books
            .Include(b => b.Comments)
            .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return null;
            }
            return book;
        }

        public async Task<List<Book>> GetBooks()
        {
            var books = await _context.Books.ToListAsync();
            return books;
        }

        public async Task<Book?> UpdateBook(Book book)
        {
            var existingBook = await _context.Books.FindAsync(book.Id);
            if (existingBook == null)
            {
                return null;
            }
            existingBook.Title = book.Title;
            await _context.SaveChangesAsync();
            return existingBook;
        }
    }
}