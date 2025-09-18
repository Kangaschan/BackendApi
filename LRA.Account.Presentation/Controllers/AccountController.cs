using LRA.Account.Application.DTOs;
using LRA.Account.Application.Interfaces;
using LRA.Account.Domain.Models;
using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.DTOs.KYC;
using LRA.Common.Exceptions;
using LRA.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ChangeEmailRequest = LRA.Account.Application.DTOs.ChangeEmailRequest;
using ChangePasswordRequest = LRA.Account.Application.DTOs.ChangePasswordRequest;
using CompleteResetPasswordRequest = LRA.Account.Application.DTOs.CompleteResetPasswordRequest;
using LoginRequest = LRA.Common.DTOs.Auth.LoginRequest;
using ResetPasswordRequest = LRA.Account.Application.DTOs.ResetPasswordRequest;

namespace LRA.Account.Presentation.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IAccountImageService _accountImageService;
    public AccountController(IAccountService accountService, IAccountImageService accountImageService)
    {
        _accountService = accountService;
        _accountImageService = accountImageService;
    }
        
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var accounts = await _accountService.GetAllAsync(cancellationToken);
        return Ok(accounts);
    }
    
    [HttpGet("filtered")]
    [Authorize]
    public async Task<IActionResult> GetFiltered([FromQuery] ViewFilterParams filterParams, CancellationToken cancellationToken)
    {
        var result = await _accountService.GetFilteredAsync(filterParams, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("by-criteria")]
    [Authorize]
    public async Task<IActionResult> GetSearchResult([FromQuery] AccountSearchParams searchParams, CancellationToken cancellationToken)
    {
        var result = await _accountService.SearchAsync(searchParams, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _accountService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequestDto account, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.CreateAsync(account, cancellationToken);
            return Ok();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("register")]
    [Authorize]
    public async Task<IActionResult> Register([FromBody] RegisterAccountDto account, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.RegisterAsync(account, cancellationToken);
            return Accepted("email verification needed");
        }
        catch (UserAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
    }
    
    [HttpPost("email/verifications")]
    [Authorize]
    public async Task<IActionResult> ConfirmEmail([FromBody] TokenDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.ConfirmEmailAsync(dto, cancellationToken);
            if (result)
            {
                return Ok("Email verified");
            }
            else
            {
                return BadRequest( "The verification token has expired. Please request a new one.");
            }
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] Guid id, AccountDto account, CancellationToken cancellationToken)
    {
        await _accountService.UpdateAsync(id, account, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _accountService.DeleteAsync(id, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id}/soft")]
    [Authorize]
    public async Task<IActionResult> SoftDelete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _accountService.SoftDeleteAsync(id, cancellationToken);
        return Ok();
    }

    [HttpPost("auth/login")]
    [Authorize]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.LoginAsync(loginRequest, cancellationToken);
            if(result.Item2)
            {
                return Ok(result.Item1);
            }
            else
            {
                return Forbid("you've logged in with that one time password");
            }
        }
        catch (Required2FaException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            if(e.Message == "Login failed")
            {
                return BadRequest(e.Message);
            }
            else
            {
                throw e;
            }
        }
    }

    [HttpPost("auth/2fa")]
    [Authorize]
    public async Task<IActionResult> Complete2Fa([FromBody] Complete2FaRequest dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.Complete2FaAsync(dto, cancellationToken);
            if (result.Item2)
            {
                return Ok(result.Item1);
            }
            else
            {
                return Unauthorized("Password is expired or email is incorrect. ");
            }
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost("auth/refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshRequest,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.RefreshToken(refreshRequest, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("jwks")]
    [Authorize]
    public async Task<IActionResult> GetJwks(CancellationToken cancellationToken)
    {
        var result = await _accountService.GetJwksAsync(cancellationToken);
        return Ok(result);
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
        var isSuccess = await _accountService.CompletePasswordResetAsync(completeResetPasswordRequest, cancellationToken);
        if(isSuccess)
        {
            return Ok("Password reset successful");
        }
        else
        {
            return BadRequest("One time password has expired. Please request a new one.");
        }
    }

    [HttpPut("password/{mail}")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromRoute]string mail, [FromBody] ChangePasswordRequest changePasswordRequest,
        CancellationToken cancellationToken)
    {
        await _accountService.ChangePasswordAsync(mail, changePasswordRequest, cancellationToken);
        return Ok("Password changed");
    }
    
    [HttpPut("profile/fullname/{keycloakId}")]
    [Authorize]
    public async Task<IActionResult> ChangeFullnameAsync([FromRoute] string keycloakId, [FromBody] ChangeFullNameRequest changeFullNameRequest, CancellationToken cancellationToken)
    {
        await _accountService.ChangeFullNameAsync(keycloakId, changeFullNameRequest, cancellationToken);
        return Ok("fullname changed");
    }
    
    [HttpPut("profile/2fa/{keycloakId}")]
    [Authorize]
    public async Task<IActionResult> Update2FaAsync([FromRoute] string keycloakId, [FromBody] Change2FaRequest сhange2FaRequest, CancellationToken cancellationToken)
    {
        await _accountService.Update2FaAsync(keycloakId, сhange2FaRequest, cancellationToken);
        return Ok("2Fa updated");
    }

    [HttpPost("email/change-request/{keycloakId}")]
    [Authorize]
    public async Task<IActionResult> ChangeMailAsync([FromRoute] string keycloakId, [FromBody] ChangeEmailRequest changeEmailRequest, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.ChangeMailAsync(keycloakId, changeEmailRequest, cancellationToken);
            return Ok("Email verification requested");
        }
        catch (UserAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
    }
    
    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> CompleteChangeMailAsync([FromBody]TokenDto dto, CancellationToken cancellationToken)
    {
        var isChanged = await _accountService.CompleteChangeMailAsync(dto, cancellationToken);
        if (isChanged)
        {
            return Ok("Email verified and password change");
        }
        else
        {
            return Unauthorized("Password is expired or email is incorrect.");
        }
    }

    [HttpGet("auth/google")]
    [Authorize]
    public async Task<IActionResult> GoogleLogin(CancellationToken cancellationToken)
    {
        var redirect = await _accountService.GetGoogleAuthUrl(cancellationToken);
        return Ok(redirect);
    }

    [HttpGet("auth/google/callback")]
    [Authorize]
    public async Task<IActionResult> GoogleLoginCallback([FromQuery] string code, CancellationToken cancellationToken)
    {
        var result = await _accountService.CompleteGoogleAuthAsync(code, cancellationToken);
        return Ok(result);
    }

    [HttpGet("account/roles/by-email/{email}")]
    [Authorize]
    public async Task<IActionResult> GetAccountRolesAsync([FromRoute] string email, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.GetAccountRolesAsync(email, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("account/roles/by-id/{id}")]
    [Authorize]
    public async Task<IActionResult> GetAccountRolesAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _accountService.GetAccountRolesByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPut("password/admin-reset/{email}")]
    [Authorize]
    public async Task<IActionResult> AdminResetPasswordAsync([FromRoute] string email,
        [FromBody] AdminChangePasswordRequest resetPasswordRequest, CancellationToken cancellationToken)
    {
        await _accountService.AdminChangePasswordAsync(email, resetPasswordRequest, cancellationToken);
        return Ok("Password reset successful");
    }

    [HttpPut("{id}/block")]
    [Authorize]
    public async Task<IActionResult> BlockAccount([FromRoute] Guid id, [FromBody] BlockRequest request,
        CancellationToken cancellationToken)
    {
        await _accountService.BlockAsync(id, request, cancellationToken);
        return Ok("Account blocked");
    }

    [HttpPost("profile/job-applications/{email}")]
    [Authorize]
    public async Task<IActionResult> CreateJobApplicationAsync([FromRoute] string email,
        [FromForm] JobApplicationDto jobApplicationDto, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.ApplyForAJobAsync(email, jobApplicationDto, cancellationToken);
            return Ok("Job application successful");
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("profile/job-applications/{email}")]
    [Authorize]
    public async Task<IActionResult> GetJobApplicationsAsync([FromRoute] string email,
        CancellationToken cancellationToken)
    {
        var applicationsList = await _accountService.CheckKycStatusAsync(email, cancellationToken);
        return Ok(applicationsList);
    }

    [HttpGet("job-applications")]
    [Authorize]
    public async Task<IActionResult> GetJobApplicationsAsync(CancellationToken cancellationToken)
    {
        var jobApplications = await _accountService.GetAllJobApplicationsAsync(cancellationToken);
        return Ok(jobApplications);
    }

    [HttpGet("job-applications/{id}")]
    [Authorize]
    public async Task<IActionResult> GetJobApplicationByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var jobApplication = await _accountService.GetDetailedJobApplicationAsync(id, cancellationToken);
        return Ok(jobApplication);
    }

    [HttpPut("job-applications")]
    [Authorize]
    public async Task<IActionResult> ProcessJobApplicationAsync([FromBody] KycProcessDto kycProcessDto,
        CancellationToken cancellationToken)
    {
        await _accountService.ProcessJobApplicationAsync(kycProcessDto, cancellationToken);
        return Ok("Job application processed");
    }
    
    [HttpPut("job-applications/reference")]
    [Authorize]
    public async Task<IActionResult> GetJobApplicationStatus([FromBody] KycProcessDto kycProcessDto,
        CancellationToken cancellationToken)
    {
        await _accountService.ProcessJobApplicationAsync(kycProcessDto, cancellationToken);
        return Ok("Job application processed");
    }
}
