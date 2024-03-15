using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bookshelf.Models
{
    public class AppUser: IdentityUser
    {
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Comment> LikedComments { get; set; } = new List<Comment>();
    }
}