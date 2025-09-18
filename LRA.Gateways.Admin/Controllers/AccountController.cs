using LRA.Common.DTOs;
using LRA.Common.Exceptions;
using LRA.Gateways.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ResetPasswordRequest = LRA.Gateways.Admin.DTOs.ResetPasswordRequest;

namespace LRA.Gateways.Admin.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountServiceClient _accountService;

    public AccountController(IAccountServiceClient accountService)
    {
        _accountService = accountService;
    }
    
    [HttpPost("password/recovery")]
    [Authorize]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken)
    {   
        await _accountService.ResetPasswordAsync(resetPasswordRequest, cancellationToken);
        return Ok("One time password sent");
    }
    
    [HttpPut("password/recovery")]
    [Authorize]
    public async Task<IActionResult> CompleteResetPasswordAsync([FromBody] CompleteResetPasswordRequest completeResetPasswordRequest, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.CompletePasswordResetAsync(completeResetPasswordRequest, cancellationToken);
            return Ok("Password reset successful");
        }
        catch (TokenOutdatedException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken)
    {
        await _accountService.ChangePasswordAsync(changePasswordRequest, cancellationToken);
        return Ok("Password changed");
    }
    
    [HttpPost("email/change-request")]
    [Authorize]
    public async Task<IActionResult> ChangeMailAsync([FromBody] ChangeEmailRequest changeEmailRequest, CancellationToken cancellationToken)
    {
        try
        {
            var userId = HttpContext.Items["UserId"].ToString();
            await _accountService.ChangeMailAsync(userId, changeEmailRequest, cancellationToken);
            return Ok("Email verification requested");
        }
        catch (UserAlreadyExistsException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> CompleteChangeMailAsync([FromBody]TokenDto dto, CancellationToken cancellationToken)
    {
        var isSucceed = await _accountService.CompleteChangeMailAsync(dto, cancellationToken);
        if (isSucceed)
        {
            return Ok("Email verified and password change");
        }
        else
        {
            return Unauthorized("Password is expired or email is incorrect.");
        }
    }
}
