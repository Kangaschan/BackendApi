namespace LRA.Common.DTOs;

public class AccountSearchParams
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public bool? IncludeAdmins { get; set; }
    public bool? IncludeClients { get; set; }
    public bool? IncludeParamedics { get; set; }
    public bool? IncludeBlocked { get; set; }
    public string? EmailSearch { get; set; }
    public string? PhoneSearch { get; set; }
    public string? FirstNameSearch { get; set; }
    public string? LastNameSearch { get; set; }
}
