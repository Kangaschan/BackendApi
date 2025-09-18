namespace LRA.Common.Models;

public class EmailMessage
{
    public required string RecipientEmail { get; set; }
    public required EmailСontent Content { get; set; }
}
