namespace EventService.Core.DTOs;

public class PurchaseResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TotalTickets { get; set; }
    public List<PurchasedTicketDto> PurchasedTickets { get; set; } = new();
}