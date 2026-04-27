namespace EventService.Core.DTOs;

public class PurchaseResponseDto
{
    public string TransactionId { get; set; } = string.Empty;
    public int EvenementId { get; set; }
    public int VisiteurId { get; set; }
    public int TotalBillets { get; set; }
    public decimal MontantTotal { get; set; }
    public List<ReservedTicketDto> BilletsReserves { get; set; } = new();
}

public class ReservedTicketDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string BilletTypeNom { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public string Statut { get; set; } = string.Empty;
}