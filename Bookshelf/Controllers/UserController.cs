using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Data;
using Bookshelf.Dtos.User;
using Bookshelf.Interfaces;
using Bookshelf.Mappers;
using Bookshelf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager; 
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly ICommentRepository _commentRepository;
        private readonly ApplicationDBContext _context;
        public UserController(
            UserManager<AppUser> userManager, 
            ITokenService tokenService, 
            SignInManager<AppUser> signInManager,
            ICommentRepository commentRepository,
            ApplicationDBContext context
            )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _commentRepository = commentRepository;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid data");
                }

                var user = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password!);

                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");

                    if (roleResult.Succeeded)
                    {
                        return Ok(new NewUserDto
                        {
                            UserName = user.UserName!,
                            Email = user.Email!,
                            Token = _tokenService.GenerateToken(user)
                        });
                    }
                    else
                    {
                        await _userManager.DeleteAsync(user); // Rollback user creation
                        return BadRequest("Failed to create user role");
                    }
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest($"Failed to create user: {errors}");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid data");
                }
                var user = await _userManager.FindByNameAsync(loginDto.UserName!);

                if (user == null)
                {
                    return BadRequest("Invalid username or password");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, false);

                if (result.Succeeded)
                {
                    return Ok(new NewUserDto
                    {
                        UserName = user.UserName!,
                        Email = user.Email!,
                        Token = _tokenService.GenerateToken(user)
                    });
                }
                else
                {
                    return BadRequest("Invalid username or password");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok("Logged out");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{userId}/comments")]
        public async Task<ActionResult<List<Comment>>> GetCommentsByUser(string userId)
        {
            try
            {
                var comments = await _commentRepository.GetCommentsByUser(userId);
                return Ok(comments);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{userId}/comments/favorites")]
        public async Task<ActionResult<List<Comment>>> GetFavoriteCommentsByUser(string userId)
        {
            try
            {
                var comments = await _commentRepository.GetUserFavoriteComments(userId);
                return Ok(comments);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<AppUser>> GetUser(string userId)
        {
            try
            {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new AppUser
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Comments = u.Comments.Select(c => new Comment
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                }).ToList(),
                LikedComments = u.LikedComments.Select(fc => fc.MapToUserComment()).ToList()
            })
            .FirstOrDefaultAsync();
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}