using Asp.Versioning;
using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.Exceptions;
using LRA.Gateways.Client.DTOs;
using LRA.Gateways.Client.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LRA.Gateways.Client.Controllers.v2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/client")]
public class ClientController : ControllerBase
{
    private readonly IAccountServiceClient _accountService;

    public ClientController(IAccountServiceClient accountService)
    {
        _accountService = accountService;
    }
    
    [HttpPost("accounts/register")]
    public async Task<IActionResult> RegisterAccont([FromBody] RegisterAccountRequestDto account, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.RegisterAsync(account, cancellationToken);
            return Accepted(new
            {
                Message = "Email verification message sent"
            });
        }
        catch (UserAlreadyExistsException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("accounts/email-verifications")]
    public async Task<IActionResult> VerifyAccont([FromBody] TokenDto token, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.VerifyAsync(token, cancellationToken);
            
            return Ok("Verified");
        }
        catch (TokenOutdatedException e)
        {
            return Problem(
                title: "Token expired",
                detail: "The verification token has expired.",
                statusCode: 410
            );
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("auth/login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
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
    
    [HttpPost("auth/refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenDto refreshToken, CancellationToken cancellationToken)
    {
        var token = await _accountService.RefreshAsync(refreshToken, cancellationToken);
        return Ok(token);
    }
    
    [HttpPost("auth/2fa")]
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
