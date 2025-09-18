using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LRA.Account.Application.DTOs;

public class UploadImageRequest
{
    public IFormFile File { get; set; }
}
