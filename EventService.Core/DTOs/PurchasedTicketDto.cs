namespace EventService.Core.DTOs;
public class PurchasedTicketDto
{
    public string TicketType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}