using Asp.Versioning;
using LRA.Common.DTOs;
using LRA.Gateways.Client.DTOs;
using LRA.Gateways.Client.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LRA.Gateways.Client.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/profile")]
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
        try
        {
            var userId = HttpContext.Items["UserId"].ToString();
            await _accountService.ChangeFullNameAsync(userId, changeFullNameRequest, cancellationToken);
            return Ok("fullname changed");
        }
        catch (Exception ex)
        {
            return BadRequest("Full name cahnge error");
        }
    }
    
    [HttpPut("2fa")]
    [Authorize]
    public async Task<IActionResult> Update2FaAsync([FromBody] Change2FaRequest сhange2FaRequest, CancellationToken cancellationToken)
    {
        try
        {
            var userId = HttpContext.Items["UserId"].ToString();
            await _accountService.Change2FaAsync(userId, сhange2FaRequest, cancellationToken);
            return Ok("2Fa updated");
        }
        catch (Exception ex)
        {
            return BadRequest("2Fa update error");
        }
    }

    [HttpPost("job-applications")]
    [Authorize]
    public async Task<IActionResult> CreateJobApplicationAsync([FromForm] JobApplicationDto jobApplicationDto,
        CancellationToken cancellationToken)
    {
        var userMail = HttpContext.Items["Mail"].ToString();
        try
        {
            await _accountService.ApplyForAJobAsync(userMail, jobApplicationDto, cancellationToken);
            return Ok("job application updated");
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("job-applications")]
    [Authorize]
    public async Task<IActionResult> GetJobApplicationsAsync(CancellationToken cancellationToken)
    {
        var userMail = HttpContext.Items["Mail"].ToString();
        var applicationsList = await _accountService.CheckKycStatusAsync(userMail, cancellationToken);
        return Ok(applicationsList);
    }
    
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetInfo(CancellationToken cancellationToken)
    {
        var userMail = HttpContext.Items["Mail"].ToString();
        var applicationsList = await _accountService.CheckKycStatusAsync(userMail, cancellationToken);
        return Ok(applicationsList);
    }
    
    
}
