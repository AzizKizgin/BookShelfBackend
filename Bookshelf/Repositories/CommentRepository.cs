using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Data;
using Bookshelf.Dtos.Comment;
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

        public async Task<Comment?> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
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

        public async Task<List<Comment>> GetComments(int bookId)
        {
            var comments = await _context.Comments.Where(c => c.BookId == bookId).ToListAsync();
            return comments;
        }

        public async Task<Comment?> UpdateComment(int bookId, int commentId, UpdateCommentDto comment)
        {
            var existingComment = await _context.Comments.FindAsync(commentId);
            if (existingComment == null)
            {
                return null;
            }
            existingComment.Content = comment.Content;
            existingComment.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return existingComment;
        }
    }
}