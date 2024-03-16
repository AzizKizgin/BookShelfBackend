using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Data;
using Bookshelf.Dtos.Comment;
using Bookshelf.Helpers;
using Bookshelf.Interfaces;
using Bookshelf.Mappers;
using Bookshelf.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;

        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }


        public async Task<Comment> AddComment(int bookId, CreateCommentDto comment)
        {
            var result = await _context.Comments.AddAsync(comment.ToCommentFromCreateDto(bookId));
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Comment?> DeleteComment(int id, string userId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }
            if (comment.AppUser.Id != userId)
            {
                throw new InvalidOperationException("You are not authorized to delete this comment");
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }
            return comment;
        }

        public async Task<List<Comment>> GetBookComments(CommentQueryObject commentQuery)
        {
            var comments = _context.Comments.Where(c => c.BookId == commentQuery.BookId).AsQueryable();
            // if (commentQuery.OrderByFavorite)
            // {

            //     //TODO: Implement OrderByFavorite
            // }
            var skip = (commentQuery.Page - 1) * commentQuery.PageSize;
            return await comments.Skip(skip).Take(commentQuery.PageSize).ToListAsync();
        }

        public async Task<Comment?> UpdateComment(int bookId, string userId, int commentId, UpdateCommentDto comment)
        {
            var existingComment = await _context.Comments.FindAsync(commentId);
            if (existingComment == null)
            {
                return null;
            }
            if (existingComment.AppUser.Id != userId)
            {
                throw new InvalidOperationException("You are not authorized to update this comment");
            }
            existingComment.Content = comment.Content;
            existingComment.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return existingComment;
        }

        public async Task<List<Comment>> GetCommentsByUser(string userId)
        {
            var comments = await _context.Comments.Where(c => c.AppUserId == userId).ToListAsync();
            return comments;
        }

        public async Task<List<Comment>> GetUserFavoriteComments(string userId)
        {
            var comments = await _context.Users
                        .Where(u => u.Id == userId)
                        .SelectMany(u => u.LikedComments)
                        .Include(c => c.Book)
                        .ToListAsync();
            return comments;
        }
    }
}