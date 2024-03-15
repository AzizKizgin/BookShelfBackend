using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Dtos.Book;
using Bookshelf.Dtos.Comment;
using Bookshelf.Helpers;
using Bookshelf.Interfaces;
using Bookshelf.Mappers;
using Bookshelf.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController: ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICommentRepository _commentRepository;

        public BookController(IBookRepository bookRepository, ICommentRepository commentRepository)
        {
            _bookRepository = bookRepository;
            _commentRepository = commentRepository;
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
        public async Task<ActionResult<Book>> UpdateBook([FromRoute] int id, [FromBody] UpdateBookDto book)
        {
            try {
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
        public async Task<ActionResult<List<Comment>>> GetComments([FromQuery] CommentQueryObject commentQuery)
        {
            try
            {
                var comments = await _commentRepository.GetComments(commentQuery);
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
        public async Task<ActionResult<Comment>> UpdateComment(int bookId, int id, UpdateCommentDto comment)
        {
            try
            {
                var updatedComment = await _commentRepository.UpdateComment(bookId, id, comment);
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
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
            try
            {
                var comment = await _commentRepository.DeleteComment(id);
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