namespace EventService.Core.DTOs;

public class ValidateTicketDto
{
    public string Code { get; set; } = string.Empty;
    public int? EventId { get; set; }
}
