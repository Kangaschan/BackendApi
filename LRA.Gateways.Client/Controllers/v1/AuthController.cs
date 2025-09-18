using Asp.Versioning;
using LRA.Common.DTOs.Auth;
using LRA.Common.Exceptions;
using LRA.Gateways.Client.DTOs;
using LRA.Gateways.Client.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LRA.Gateways.Client.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
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
        catch (Exception e)
        {
            return BadRequest("login failed");
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
        try
        {
            var result = await _accountService.Complete2FaAsync(token, cancellationToken);
            if (result.Item2)
            {
                return Ok(result.Item1);
            }
            else
            {
                return Unauthorized();
            }
        }
        catch (Exception e)
        {
            return BadRequest("complete2fa failed");
        }
    }
    
    [HttpGet("google")]
    public async Task<IActionResult> GoogleLoginAsync(CancellationToken cancellationToken)
    {
        var redirect = await _accountService.GoogleLoginAsync(cancellationToken);
        return Ok(redirect);
    }
    
    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleLoginCallback([FromQuery] string code, CancellationToken cancellationToken)
    {
        var token = await _accountService.CompleteGoogleLoginAsync(code, cancellationToken);
        return Ok(token);
    }
}
