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


        public async Task<Comment> AddComment(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment.MapToBookComment();
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
            var comment = await _context
                .Comments
                .Include(c => c.Book)
                .Include(c => c.AppUser)
                .Include(c => c.LikedBy)
                .FirstOrDefaultAsync(c => c.Id == id);
            return comment;
        }

        public async Task<List<Comment>> GetBookComments(CommentQueryObject commentQuery)
        {
            var comments = _context.Comments
            .Where(c => c.Book.Title == commentQuery.BookTitle)
            .Include(c => c.Book)
            .Include(c => c.AppUser)
            .Include(c => c.LikedBy)
            .AsQueryable();

            if (commentQuery.OrderByFavorite)
            {
                comments = comments.OrderByDescending(c => c.LikedBy.Count);
            }
            
            var skip = (commentQuery.Page - 1) * commentQuery.PageSize;
            return await comments
                .Skip(skip)
                .Take(commentQuery.PageSize)
                .Select(c => c.MapToBookComment())
                .ToListAsync();
        }

        public async Task<Comment?> UpdateComment(string userId, int commentId, UpdateCommentDto comment)
        {
            var existingComment = await _context.Comments
                .Include(c => c.Book)
                .Include(c => c.AppUser)
                .Include(c => c.LikedBy)
                .FirstOrDefaultAsync(c => c.Id == commentId);
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
            return existingComment.MapToBookComment();
        }

        public async Task<List<Comment>> GetCommentsByUser(string userId)
        {
            var comments = await _context.Comments
                        .OrderByDescending(c => c.CreatedAt)
                        .Include(c => c.Book)
                        .Include(c => c.AppUser)
                        .Include(c => c.LikedBy)
                        .Where(c => c.AppUserId == userId)
                        .Select(c => c.MapToBookComment())
                        .ToListAsync();
            return comments;
        }

        public async Task<List<Comment>> GetUserFavoriteComments(string userId)
        {
            var comments = await _context.Comments
                .Where(c => c.LikedBy.Any(u => u.Id == userId))
                .Include(c => c.Book)
                .Include(c => c.AppUser)
                .Include(c => c.LikedBy)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => c.MapToBookComment())
                .ToListAsync();
            return comments;
        }
    }
}