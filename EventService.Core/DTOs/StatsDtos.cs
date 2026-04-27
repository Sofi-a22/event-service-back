namespace EventService.Core.DTOs;

public class OrganizerStatsDto
{
    public int TotalEvenements { get; set; }
    public int EvenementsActifs { get; set; }
    public int TotalBilletsVendus { get; set; }
    public decimal RevenueTotal { get; set; }
    public List<EvenementStatDto> Evenements { get; set; } = new();
}

public class EvenementStatDto
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int Capacite { get; set; }
    public int PlacesRestantes { get; set; }
    public int BilletsVendus { get; set; }
    public decimal Revenue { get; set; }
    public double TauxRemplissage { get; set; }
}