using Microsoft.AspNetCore.Mvc;
using Asp.Versioning.ApiExplorer;

namespace LRA.Gateways.Client.Controllers;

[ApiController]
[Route("api/version-check")]
public class VersionCheckController : ControllerBase
{
    private readonly IApiVersionDescriptionProvider _versionDescriptionProvider;

    public VersionCheckController(IApiVersionDescriptionProvider versionDescriptionProvider)
    {
        _versionDescriptionProvider = versionDescriptionProvider;
    }

    [HttpGet]
    public IActionResult CheckVersion([FromQuery] string version)
    {
        if (string.IsNullOrEmpty(version))
        {
            return BadRequest("Version parameter is required.");
        }

        if (!Version.TryParse(version, out var parsedVersion))
        {
            return BadRequest("Invalid version format. Please provide a valid version (e.g., '1.0').");
        }

        var supportedVersions = _versionDescriptionProvider.ApiVersionDescriptions
            .Select(v => v.ApiVersion.ToString())
            .ToList();

        bool isSupported = supportedVersions.Any(v => v.Equals(version, StringComparison.OrdinalIgnoreCase));

        return Ok(new { Version = version, IsSupported = isSupported });
    }
}
