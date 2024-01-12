using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.Dto;
using Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webstory.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]ApplicationUserCreateDto model)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid registration data", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                // Check if the user already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "Email is already registered" });
                }

                // Create a new ApplicationUser
                var newUser = new ApplicationUser
                {
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email,
                    // Add other properties as needed
                };

                // Attempt to create the user
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new { Message = "Registration failed", Errors = result.Errors.Select(e => e.Description) });
                }

                // Add user to the desired role
                await _userManager.AddToRoleAsync(newUser, SD.USER);

                // You can optionally sign in the user after registration
                // await _signInManager.SignInAsync(newUser, isPersistent: false);

                // Generate JWT token
                var token = GenerateJwtToken(newUser);

                // Return success response with JWT token
                return Ok(new { Message = "Registration successful", Token = token });
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return an error response
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]ApplicationUserCreateDto model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid registration data", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // User found, generate and return JWT token
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }

            // Invalid credentials
            return Unauthorized(new { Message = "Invalid credentials" });

        }

        [Authorize]
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromBody]ApplicationUserUpdateDto model)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid registration data", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                // Lấy thông tin user hiện tại từ token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                // Cập nhật thông tin người dùng từ Dto
                user.Fullname = model.Fullname;

                // Lưu các thay đổi vào cơ sở dữ liệu
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "Edit successful" });
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { Errors = errors });
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, ghi log, hoặc trả về lỗi phù hợp
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        [Authorize(SD.ADMIN)]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] ApplicationUserDeleteDto model)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid registration data", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                // Lấy thông tin user hiện tại từ token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                // Xác minh mật khẩu để xác nhận xóa tài khoản
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);

                if (!isPasswordValid)
                {
                    return BadRequest(new { Message = "Invalid password" });
                }

                // Thực hiện xóa tài khoản người dùng
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // Đăng xuất người dùng sau khi xóa tài khoản
                    await _signInManager.SignOutAsync();

                    return Ok(new { Message = "Account deleted" });
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { Errors = errors });
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, ghi log, hoặc trả về lỗi phù hợp
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id),
                                new Claim(ClaimTypes.Name, user.UserName),
                                // Add more claims as needed
                            };

            // Lấy danh sách các vai trò của người dùng
            var roles = _userManager.GetRolesAsync(user).Result;

            // Thêm mỗi vai trò như một claim
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(10800), // Adjust expiration as needed
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private string GetUserRole(ApplicationUser user)
        {
            // Lấy vai trò của người dùng
            var roles = _userManager.GetRolesAsync(user).Result;

            // Nếu người dùng có vai trò, trả về vai trò đầu tiên, ngược lại trả về một giá trị mặc định
            return roles.Any() ? roles.First() : SD.USER;
        }
    }
}

