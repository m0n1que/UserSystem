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
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public IReadOnlyList<Role> GetAllRoles()
        {
            var roles =  _roleManager.Roles.ToList();
            return roles.Select(roles => new Role{
                Id = roles.Id, 
                RoleName = roles.Name,            
                UpperRoleName = roles.NormalizedName
            }).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRoleById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound(new ApiResponse(404));
            return new Role{
                Id = role.Id, 
                RoleName = role.Name, 
                UpperRoleName = role.NormalizedName
            };
        }


        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CreateRoleDto>> CreateRole(CreateRoleDto roleToCreate)
        {
            var roleExits = await _roleManager.RoleExistsAsync(roleToCreate.RoleName);
            if(roleExits) return BadRequest(new ApiResponse(400, "Role name exists already"));

            var role = await _roleManager.CreateAsync(new IdentityRole(roleToCreate.RoleName));
            if(role.Succeeded) return new CreateRoleDto{
                RoleName = roleToCreate.RoleName                
            };            
            return BadRequest(new ApiResponse(400, "Saving failed"));
        }


        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> UpdateRole(UpdateRoleDto roleToUpdate)
        {
            var curRole = await _roleManager.FindByIdAsync(roleToUpdate.Id);
            if (curRole == null) return BadRequest(new ApiResponse(400, "Role does not exists"));  
        
            curRole.Name = roleToUpdate.NewRoleName;
            var res = await _roleManager.UpdateAsync(curRole);
            if(res.Succeeded) return Ok(true);
            
            return BadRequest(new ApiResponse(400, "Saving failed"));
        }
        

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> DeleteRole(string id)
        {            
            var curRole = await _roleManager.FindByIdAsync(id);
            if (curRole == null) return BadRequest(new ApiResponse(400, "Role does not exists"));  
            
            var res = await _roleManager.DeleteAsync(curRole);
            if (res.Succeeded) return Ok(true);

            return BadRequest(new ApiResponse(400, "Deleting failed"));
        }



    }
}