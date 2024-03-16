using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Dtos;
using Bookshelf.Dtos.Comment;
using Bookshelf.Models;

namespace Bookshelf.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto MapToDto(this Comment comment) =>
            new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                BookId = comment.BookId
            };

        public static Comment ToCommentFromCreateDto(this CreateCommentDto comment, int bookId) =>
            new Comment
            {
                Content = comment.Content,
                CreatedAt = DateTime.Now,
                BookId = bookId
            };

        public static Comment ToCommentFromUpdateDto(this UpdateCommentDto comment, int bookId, int commentId) =>
            new Comment
            {
                Id = commentId,
                Content = comment.Content,
                UpdatedAt = comment.UpdatedAt,
                BookId = bookId
            };

        public static Comment MapToUserComment(this Comment comment) =>
            new Comment
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                Book = new Book
                {
                    Id = comment.Book.Id,
                    Title = comment.Book.Title,
                },
                BookId = comment.BookId,
                LikedBy = comment.LikedBy.Select(u => new AppUser
                {
                    Id = u.Id,
                    UserName = u.UserName,
                }).ToList()
    };
         
    }
}