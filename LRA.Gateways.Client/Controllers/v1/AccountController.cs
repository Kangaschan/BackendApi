using Asp.Versioning;
using LRA.Common.DTOs;
using LRA.Common.Exceptions;
using LRA.Gateways.Client.Attributes;
using LRA.Gateways.Client.DTOs;
using LRA.Gateways.Client.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LRA.Gateways.Client.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountServiceClient _accountService;
    private readonly ISubscriptionsServiceClient _subscriptionsService;

    public AccountController(IAccountServiceClient accountService, ISubscriptionsServiceClient subscriptionsService)
    {
        _accountService = accountService;
        _subscriptionsService = subscriptionsService;
    }
    
    [HttpPost("register")]
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
        catch (Exception e)
        {
            return BadRequest("Registration failed");
        }
    }
    
    [HttpPost("email/verifications")]
    public async Task<IActionResult> VerifyAccont([FromBody] TokenDto token, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.VerifyAsync(token, cancellationToken);

            return Ok("Verified");
        }
        catch (TokenOutdatedException e)
        {
            return BadRequest("The verification token has expired.");
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest($"Verification failed. {e.Message}");
        }
    }
    
    [HttpPost("password/recovery")]
    [Authorize]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.ResetPasswordAsync(resetPasswordRequest, cancellationToken);
            return Ok("One time password sent");
        }
        catch
        {
            return BadRequest("Error reestting password");
        }
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
        catch (Exception e)
        {
            return BadRequest("password reset failed");
        }
    }
    
    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken)
    {
        try
        {
            var mail = HttpContext.Items["Mail"].ToString();
            await _accountService.ChangePasswordAsync(mail, changePasswordRequest, cancellationToken);
            return Ok("Password changed");
        }
        catch
        {
            return BadRequest("Password change failed");
        }
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
        catch (Exception e)
        {
            return BadRequest("start of change mail failed");
        }
    }
    
    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> CompleteChangeMailAsync([FromBody]TokenDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var isSucceed = await _accountService.CompleteChangeMailAsync(dto, cancellationToken);
            if (isSucceed)
            {
                return Ok("Email verified and password change");
            }
            else
            {
                return BadRequest("Password is expired or email is incorrect.");
            }
        }
        catch (Exception e)
        {
            return BadRequest("Change mail failed");
        }
    }
    
    [HttpPost("checkout-url")]
    [Authorize]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] ChoosedSubscriptionDto request, CancellationToken cancellationToken)
    {
        try
        {
            var mail = HttpContext.Items["Mail"].ToString();
            var sessionUrl = await _subscriptionsService.BuySubscriptionsAsync(request, mail, cancellationToken);
            return Ok(sessionUrl);
        }
        catch (SubscriptionException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("prices")]
    [Authorize]
    public async Task<IActionResult> GetPrices(CancellationToken cancellationToken)
    {
        var prices = await _subscriptionsService.GetPricesAsync(cancellationToken);
        return Ok(prices);
    }
    
    [HttpPost("portal-url")]
    [Authorize]
    [RequireActiveSubscription]
    public async Task<IActionResult> CreatePortalSession(CancellationToken cancellationToken)
    {
        var mail = HttpContext.Items["Mail"].ToString();
        var sessionUrl = await _subscriptionsService.ManageSubscriptionsAsync(mail, cancellationToken);
        return Ok(sessionUrl);
    }
}
