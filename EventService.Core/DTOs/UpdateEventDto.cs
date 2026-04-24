namespace EventService.Core.DTOs;

public class UpdateEventDto
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TypeEvent { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string Disponibilite { get; set; } = string.Empty;
    public int Capacite { get; set; }
    public int OrganisateurId { get; set; }
    public int? LocalisationId { get; set; }
}