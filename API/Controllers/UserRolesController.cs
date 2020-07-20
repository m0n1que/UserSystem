using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UserRolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRolesController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IList<string>>> GetUserRoles(string userName)
        {
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null) return NotFound(new ApiResponse(404, "User was not found"));
            var res = await _userManager.GetRolesAsync(user);
            if (res == null) return NotFound(new ApiResponse(404, "User has no roles assigned"));
            return res.ToList();
        }

        [HttpPost("addToRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> AddUserToRole(UserRoleDto userRole)
        {
            var user = await _userManager.FindByEmailAsync(userRole.UserName);
            var role = await _roleManager.FindByNameAsync(userRole.RoleName);
            var res = await _userManager.AddToRoleAsync(user, role.Name);
            if (res == null) return BadRequest(new ApiResponse(400));
            return Ok(true);
        }

        [HttpPost("removeFromRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> RemoveUserFromRole(UserRoleDto userRole)
        {
            var user = await _userManager.FindByEmailAsync(userRole.UserName);
            var role = await _roleManager.FindByNameAsync(userRole.RoleName);
            if(user == null || role == null ) return BadRequest(new ApiResponse(400, "User or role doen't exists in database"));

            var res = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (res == null) return Ok(false);
            if (!res.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(true);
        }


        }
}