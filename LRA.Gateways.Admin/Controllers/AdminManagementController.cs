using System.Data;
using LRA.Common.DTOs;
using LRA.Gateways.Admin.DTOs;
using LRA.Gateways.Admin.Interfaces;
using LRA.Gateways.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LRA.Gateways.Admin.Controllers;

[ApiController]
[Route("api/management")]
public class AdminManagementController : ControllerBase
{
    private readonly IAccountServiceClient _accountService;

    public AdminManagementController(IAccountServiceClient accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("accounts")]
    [Authorize]
    public async Task<IActionResult> CreateAccountAsync([FromBody] AdminCreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var isSuperAdmin = HttpContext.Items["IsSuperAdmin"] as bool? ?? false;
            var account = await _accountService.CreateAccountAsync(request, cancellationToken);
            return Ok(account);
        }
        catch (InvalidDataException e)
        {
            return BadRequest("email is taken");
        }
        catch (UnauthorizedAccessException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("accounts")]
    [Authorize]
    public async Task<IActionResult> GetFilteredAccountsAsync([FromQuery] ViewFilterParams filterParams,
        CancellationToken cancellationToken)
    {
        var accounts = await _accountService.GetFilteredAsync(filterParams, cancellationToken);
        return Ok(accounts);
    }

    [HttpGet("accounts/by-criteria")]
    [Authorize]
    public async Task<IActionResult> SearchAccountsAsync([FromQuery] AccountSearchParams searchParams,
        CancellationToken cancellationToken)
    {
        var accounts = await _accountService.SearchAsync(searchParams, cancellationToken);
        return Ok(accounts);
    }

    [HttpGet("accounts/{id}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var account = await _accountService.GetByIdAsync(id, cancellationToken);
        if (account == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(account);
        }
    }

    [HttpPut("accounts/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAccountAsync([FromRoute] Guid id, [FromBody] AccountDto accountDto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.UpdateAccountAsync(id, accountDto, cancellationToken);
            return Ok();
        }
        catch (UnauthorizedAccessException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("recovery/{email}")]
    [Authorize]
    public async Task<IActionResult> ResetPassword([FromRoute] string email, [FromBody] ResetOptions options,
        CancellationToken cancellationToken)
    {
        try
        {
            var message = await _accountService.ResetAccountPasswordAsync(email, options, cancellationToken);
            return Ok(message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}/block")]
    [Authorize]
    public async Task<IActionResult> ChangeBlockStatusAsync([FromRoute] Guid id, [FromBody] BlockRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.ChangeBlockStatus(id, request, cancellationToken);
            return Ok();
        }
        catch (UnauthorizedAccessException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidExpressionException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAccountAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _accountService.DeleteAccountAsync(id, cancellationToken);
            return Ok();
        }
        catch (UnauthorizedAccessException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}/soft")]
    [Authorize]
    public async Task<IActionResult> SoftDeleteAccountAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _accountService.SoftDeleteAccountAsync(id, cancellationToken);
        return Ok();
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
    public async Task<IActionResult> GetJobApplicationByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var jobApplication = await _accountService.GetDetailedJobApplicationAsync(id, cancellationToken);
        return Ok(jobApplication);
    }

    [HttpPut("job-applications")]
    [Authorize]
    public async Task<IActionResult> ProcessJobApplicationAsync(
        [FromBody] JobApplicationProcessDto jobApplicationProcessDto, CancellationToken cancellationToken)
    {
        await _accountService.ProcessJobApplicationAsync(jobApplicationProcessDto, cancellationToken);
        return Ok("Job application processed successful");
    }
    
    [HttpGet("job-applications/reference")]
    [Authorize]
    public  IActionResult GetJobApplication(CancellationToken cancellationToken)
    {
        var admin = new KycStatus
        {
            Name = "Pending",
            Id = 0
        };
        var superAdmin = new KycStatus
        {
            Name = "Approved",
            Id = 1
        };
        var paramedic = new KycStatus
        {
            Name = "Rejected",
            Id = 2
        };

        var roles = new List<KycStatus> { admin, superAdmin, paramedic, };

        return Ok(roles);
    }
    

    [HttpGet("roles")]
    [Authorize]
    public IActionResult GetRoles()
    {
        var admin = new RolesREsponse
        {
            RoleName = "admin",
            Id = "766745d4-137e-42d5-b8b5-73d039a91720"
        };
        var superAdmin = new RolesREsponse
        {
            RoleName = "superAdmin",
            Id = "410434b8-9c7d-47be-a591-2bc1e6005d24"
        };
        var paramedic = new RolesREsponse
        {
            RoleName = "paramedic",
            Id = "85b823f0-4b6a-45fe-a44b-efd0df385ace"
        };
        var client = new RolesREsponse
        {
            RoleName = "client",
            Id = "d6a978a8-aa11-471c-be82-71ef221afdf0"
        };

        var roles = new List<RolesREsponse> { admin, superAdmin, paramedic, client };

        return Ok(roles);
    }
}



