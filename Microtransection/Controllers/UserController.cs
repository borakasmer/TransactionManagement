using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microtransection.DB;

namespace Microtransection.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public List<Users> Get()
        {
            return _userService.GetAll();
        }
        [Route("InsertUser")]
        [HttpPost]
        public string InsertUser([FromBody] UserShop data)
        {
            return _userService.InsertUser(data);
        }
    }
}
