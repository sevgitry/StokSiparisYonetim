using DataAccessLayer;
using EntityLayer;
using EntityLayer.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public LoginApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        #region Login Giriş İşlemleri
        
        [HttpPost("SignIn")]
        public IActionResult SignIn(LoginDto us) 
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == us.Email && x.Password == us.Password);
            if (user != null)
            {
                {
                    return Ok(new {Message = "Giriş Başarılı", User = user});
                }
            }
            else
            {
                return Unauthorized(new { Message = "Geçersiz Kullanıcı" });
            }
        }
        #endregion
    }
}
