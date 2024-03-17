using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using Bookshelf.Dtos.Book;
using Bookshelf.Dtos.Comment;
using Bookshelf.Helpers;
using Bookshelf.Interfaces;
using Bookshelf.Mappers;
using Bookshelf.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController: ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<AppUser> _userManger;

        public BookController(IBookRepository bookRepository, ICommentRepository commentRepository,UserManager<AppUser> userManager)
        {
            _bookRepository = bookRepository;
            _commentRepository = commentRepository;
            _userManger = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetBooks([FromQuery] BookQueryObject queryObject)
        {
            try
            {
                var books = await _bookRepository.GetBooks(queryObject);
                return Ok(books);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Book>> AddBook([FromBody] CreateBookDto book)
        {
            try {
                var username = User.Identity.Name;
                var user = await _userManger.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized();
                }
                var bookEntity = book.BookFromCreateBookDto();
                bookEntity.Comments[0].AppUserId = user.Id;
                bookEntity.Comments[0].AppUser = user;
                var newBook = await _bookRepository.AddBook(bookEntity);
                return Ok(newBook);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{title}")]
        [Authorize]
        public async Task<ActionResult<Book>> UpdateBook([FromRoute] string title, [FromBody] UpdateBookDto book)
        {
            try {
                var username = User.Identity.Name;
                var user = await _userManger.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized();
                }
                var updatedBook = await _bookRepository.UpdateBook(title, book);
                if (updatedBook == null)
                {
                    return NotFound();
                }
                return Ok(updatedBook);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize (Roles = "Admin")]
        public async Task<ActionResult<Book>> DeleteBook([FromRoute] int id)
        {
            try
            {
                var book = await _bookRepository.DeleteBook(id);
                if (book == null)
                {
                    return NotFound();
                }
                return Ok(book);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}