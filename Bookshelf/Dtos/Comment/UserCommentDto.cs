using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Models;

namespace Bookshelf.Dtos.Comment
{
    public class UserCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Models.Book? Book { get; set; } 
        public int? BookId { get; set; }
        public List<AppUser> LikedBy { get; set; } = new List<AppUser>();
    }
}
