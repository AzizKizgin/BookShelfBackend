using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("api/comments")]
    public class CommentController: ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<AppUser> _userManger;

        public CommentController(IBookRepository bookRepository, ICommentRepository commentRepository,UserManager<AppUser> userManager)
        {
            _bookRepository = bookRepository;
            _commentRepository = commentRepository;
            _userManger = userManager;
        }
        
        [HttpGet]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment([FromRoute] int id)
        {
            try
            {
                var comment = await _commentRepository.GetComment(id);
                if (comment == null)
                {
                    return NotFound();
                }
                return Ok(comment.MapToBookComment());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Comment>> AddComment(string bookTitle, CreateCommentDto comment)
        {
            try
            {
                var username = User.Identity.Name;
                var user = await _userManger.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized();
                }
                var book = await _bookRepository.GetBook(bookTitle);
                if (book == null)
                {
                    return NotFound("Book not found");
                }
                var commentEntity = comment.ToCommentFromCreateDto(book);
             
                commentEntity.AppUserId =  user.Id;
                commentEntity.AppUser = user;
                var newComment = await _commentRepository.AddComment(commentEntity);
                return Ok(newComment.MapToBookComment());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Comment>> UpdateComment(int id, UpdateCommentDto comment)
        {
            try
            {
                var username = User.Identity.Name;
                var user = await _userManger.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized();
                }
                var updatedComment = await _commentRepository.UpdateComment(user.Id, id, comment);
                if (updatedComment == null)
                {
                    return NotFound();
                }
                return Ok(updatedComment.MapToBookComment());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
            try
            {
                var username = User.Identity.Name;
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

        [HttpPost("{id}")]
        [Authorize]
        public async Task<ActionResult<Comment>> FavoriteComment([FromRoute] int id)
        {
            try
            {
                var username = User.Identity.Name;
                var user = await _userManger.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized();
                }
                var comment = await _commentRepository.GetComment(id);
                if (comment == null)
                {
                    return NotFound("Comment not found");
                }
                var likedComment = user.LikedComments.FirstOrDefault(c => c.Id == id);
                if (likedComment != null)
                {
                    user.LikedComments.Remove(likedComment);
                    await _userManger.UpdateAsync(user);
                    return Ok(comment);
                }
                user.LikedComments.Add(comment);
                await _userManger.UpdateAsync(user);
                return Ok(comment);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}