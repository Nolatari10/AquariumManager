using System.Security.Claims;
using AquariumManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquariumManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    [Authorize(Roles = "Owner")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        if (tenantIdClaim is null || !int.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var users = await _authService.GetAllUsersAsync(tenantId);
        return Ok(users);
    }

    [Authorize(Roles = "Owner")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var currentUserId))
            return Unauthorized();

        var tenantIdClaim = User.FindFirst("tenantId")?.Value;
        if (tenantIdClaim is null || !int.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var result = await _authService.DeleteUserAsync(id, currentUserId, tenantId);
        if (!result.Success)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { message = "User deleted." });
    }
}
