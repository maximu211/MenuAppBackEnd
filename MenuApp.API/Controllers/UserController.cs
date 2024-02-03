using MenuApp.DAL.Models;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MenuApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YourController : ControllerBase
    {
        private readonly IUsersRepository _userRepository;

        public YourController(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("addUser")]
        public IActionResult AddUser([FromBody] Users user)
        {
            _userRepository.AddUser(user);
            return Ok("User added successfully");
        }
    }
}
