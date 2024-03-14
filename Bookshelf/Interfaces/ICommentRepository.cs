using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Dtos.Comment;
using Bookshelf.Models;

namespace Bookshelf.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetComments(int bookId);
        Task<Comment?> GetComment(int id);
        Task<Comment> AddComment(int bookId, CreateCommentDto comment);
        Task<Comment?> UpdateComment(int bookId, int commentId, UpdateCommentDto comment);
        Task<Comment?> DeleteComment(int id);
    }
}