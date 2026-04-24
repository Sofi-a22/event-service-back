namespace EventService.Core.Models;

public class Localisation
{
    public int Id { get; set; }  
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Adresse { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string CodePostal { get; set; } = string.Empty;

    public ICollection<Evenement> Evenements { get; set; } = new List<Evenement>();
}