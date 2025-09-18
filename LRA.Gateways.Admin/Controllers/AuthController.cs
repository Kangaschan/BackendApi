using LRA.Common.DTOs.Auth;
using LRA.Common.Exceptions;
using LRA.Gateways.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LRA.Gateways.Admin.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAccountServiceClient _accountService;

    public AuthController(IAccountServiceClient accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await _accountService.LoginAsync(loginRequest, cancellationToken);
            return Ok(token);
        }
        catch (Required2FaException e)
        {
            return Unauthorized(e.Message);
        }
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenDto refreshToken, CancellationToken cancellationToken)
    {
        var token = await _accountService.RefreshAsync(refreshToken, cancellationToken);
        return Ok(token);
    }
    
    [HttpPost("2fa")]
    public async Task<IActionResult> Complete2FaAsync([FromBody] Complete2FaRequest token, CancellationToken cancellationToken)
    {
        var result = await _accountService.Complete2FaAsync(token, cancellationToken);
        if(result.Item2)
        {
            return Ok(result.Item1);
        }
        else
        {
            return Unauthorized();
        }
    }
}
