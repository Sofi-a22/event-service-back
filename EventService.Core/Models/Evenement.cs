using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EventService.Core.Enums;

namespace EventService.Core.Models;

public class Evenement
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TypeEvenement? TypeEvent { get; set; }
    public CategorieEvenement? Categorie { get; set; }
    public DisponibiliteEvenement? Disponibilite { get; set; }
    public int Capacite { get; set; }
    public int PlacesRestantes { get; set; }
    public string? LienPartage { get; set; }

    [Column("IMAGE_URL")]
    public string? ImageUrl { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public int OrganisateurId { get; set; }  
    public int? LocalisationId { get; set; }  

    
    public Localisation? Localisation { get; set; }
    public ICollection<BilletType> BilletTypes { get; set; } = new List<BilletType>();
}