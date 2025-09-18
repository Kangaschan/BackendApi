namespace LRA.Common.DTOs;

public class ViewFilterParams
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public bool? IncludeAdmins { get; set; }
    public bool? IncludeClients { get; set; }
    public bool? IncludeParamedics { get; set; }
    public bool? IncludeBlocked { get; set; }
}
