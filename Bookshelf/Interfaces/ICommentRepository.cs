using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Dtos.Comment;
using Bookshelf.Helpers;
using Bookshelf.Models;

namespace Bookshelf.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetBookComments(CommentQueryObject commentQuery);
        Task<Comment?> GetComment(int id);
        Task<Comment> AddComment(Comment comment);
        Task<Comment?> UpdateComment(int bookId, string userId, int commentId, UpdateCommentDto comment);
        Task<Comment?> DeleteComment(int id,string userId);
        Task<List<Comment>> GetCommentsByUser(string userId);
        Task<List<Comment>> GetUserFavoriteComments(string userId);
    }
}