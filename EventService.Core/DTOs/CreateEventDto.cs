namespace EventService.Core.DTOs;

public class CreateEventDto
{
    public string Titre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TypeEvent { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string? Disponibilite { get; set; }
    public int Capacite { get; set; }
    public string? LienPartage { get; set; }
    public int OrganisateurId { get; set; }  // ✅ Seulement l'ID
    public int? LocalisationId { get; set; }
}