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

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook([FromRoute] int id)
        {
            try
            {
                var book = await _bookRepository.GetBook(id);
                if (book == null)
                {
                    return NotFound();
                }
                return Ok(book);
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
                var newBook = await _bookRepository.AddBook(book);
                return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Book>> UpdateBook([FromRoute] int id, [FromBody] UpdateBookDto book)
        {
            try {
                // get the user id from the token
                var username = User.GetUsername();
                var user = await _userManger.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized();
                }
                var updatedBook = await _bookRepository.UpdateBook(id, book);
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

        [HttpGet("{bookId}/comments")]
        public async Task<ActionResult<List<Comment>>> GetBookComments([FromQuery] CommentQueryObject commentQuery)
        {
            try
            {
                var comments = await _commentRepository.GetBookComments(commentQuery);
                return Ok(comments);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{bookId}/comments/{id}")]
        public async Task<ActionResult<Comment>> GetComment([FromRoute] int id)
        {
            try
            {
                var comment = await _commentRepository.GetComment(id);
                if (comment == null)
                {
                    return NotFound();
                }
                return Ok(comment);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{bookId}/comments")]
        [Authorize]
        public async Task<ActionResult<Comment>> AddComment(int bookId, CreateCommentDto comment)
        {
            try
            {
                var newComment = await _commentRepository.AddComment(bookId, comment);
                return CreatedAtAction(nameof(GetComment), new { id = newComment.Id }, newComment);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{bookId}/comments/{id}")]
        [Authorize]
        public async Task<ActionResult<Comment>> UpdateComment(int bookId, int id, UpdateCommentDto comment)
        {
            try
            {
                var username = User.GetUsername();
                var user = await _userManger.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized();
                }
                var updatedComment = await _commentRepository.UpdateComment(bookId, user.Id, id, comment);
                if (updatedComment == null)
                {
                    return NotFound();
                }
                return Ok(updatedComment);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{bookId}/comments/{id}")]
        [Authorize]
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
            try
            {
                var username = User.GetUsername();
                var user = await _userManger.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized();
                }
                var comment = await _commentRepository.DeleteComment(id, user.Id);
                if (comment == null)
                {
                    return NotFound();
                }
                return Ok(comment);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}