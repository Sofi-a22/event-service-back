using System;
using System.Collections.Generic;

namespace EventService.Core.Models;

public class Evenement
{
    public int Id { get; set; }  
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? TypeEvent { get; set; }
    public string? Categorie { get; set; }
    public string? Disponibilite { get; set; }
    public int Capacite { get; set; }
    public int PlacesRestantes { get; set; }
    public string? LienPartage { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public int OrganisateurId { get; set; }  
    public int? LocalisationId { get; set; }  

    
    public Localisation? Localisation { get; set; }
    public ICollection<BilletType> BilletTypes { get; set; } = new List<BilletType>();
}