using LRA.Gateways.Admin.DTOs;
using LRA.Gateways.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LRA.Gateways.Admin.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IAccountServiceClient _accountService;

    public ProfileController(IAccountServiceClient accountService)
    {
        _accountService = accountService;
    }
    
    [HttpPut("fullname")]
    [Authorize]
    public async Task<IActionResult> ChangeFullnameAsync([FromBody] ChangeFullNameRequest changeFullNameRequest, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Items["UserId"].ToString();
        await _accountService.ChangeFullNameAsync(userId, changeFullNameRequest, cancellationToken);
        return Ok("fullname changed");
    }
    
    [HttpPut("2fa")]
    [Authorize]
    public async Task<IActionResult> Update2FaAsync([FromBody] Change2FaRequest сhange2FaRequest, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Items["UserId"].ToString();
        await _accountService.Change2FaAsync(userId, сhange2FaRequest, cancellationToken);
        return Ok("2Fa updated");
    }
}
