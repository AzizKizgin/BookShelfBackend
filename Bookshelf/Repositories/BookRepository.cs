using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Data;
using Bookshelf.Dtos.Book;
using Bookshelf.Helpers;
using Bookshelf.Interfaces;
using Bookshelf.Mappers;
using Bookshelf.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Bookshelf.Repositories
{
    public class BookRepository: IBookRepository
    {
        private readonly ApplicationDBContext _context;

        public BookRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Book> AddBook(Book book)
        {
            var result = await _context.Books.AddAsync(book);
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

        public async Task<Book?> GetBook(string title)
        {
            return await _context.Books
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => b.Title == title);
        }

        public async Task<List<Book>> GetBooks(BookListQueryObject bookQuery)
        {
            var books = _context
                .Books
                .Include(b => b.Comments)
                .AsQueryable();
            if (bookQuery.Title != null)
            {
                books = books.Where(b => b.Title.Contains(bookQuery.Title));
            }
            if (bookQuery.FromDate != null)
            {
                books = books.Where(b => b.Comments.Any(c => c.CreatedAt > DateTime.UnixEpoch.AddDays(bookQuery.FromDate.Value - 1)));
            }
            var skip = (bookQuery.Page - 1) * bookQuery.PageSize;
            return await books.Skip(skip).Take(bookQuery.PageSize).ToListAsync();
        }

        public async Task<Book?> UpdateBook(string title, UpdateBookDto book)
        {
            var existingBook = await _context.Books.FindAsync(title);
            if (existingBook == null)
            {
                return null;
            }
            if (!existingBook.Comments.IsNullOrEmpty())
            {
                throw new InvalidOperationException("Cannot update a book with comments");
            }
            existingBook.Title = book.Title;
            await _context.SaveChangesAsync();
            return existingBook;
        }
    }
}