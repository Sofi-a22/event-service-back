using EventService.Core.Enums;

namespace EventService.Core.DTOs;

public class CreateEventDto
{
    public string Titre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TypeEvenement TypeEvent { get; set; }
    public CategorieEvenement Categorie { get; set; }
    public DisponibiliteEvenement? Disponibilite { get; set; }
    public int Capacite { get; set; }
    public string? LienPartage { get; set; }
    public string? ImageUrl { get; set; }
    public int OrganisateurId { get; set; }
    public int? LocalisationId { get; set; }
}
