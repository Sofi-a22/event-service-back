using EventService.Core.DTOs;
using EventService.Core.Interfaces;
using EventService.Core.Models;

namespace EventService.Infrastructure.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IUnitOfWork _uow;
    private readonly ITicketCodeGenerator _codeGenerator;

    public PurchaseService(IUnitOfWork uow, ITicketCodeGenerator codeGenerator)
    {
        _uow = uow;
        _codeGenerator = codeGenerator;
    }

    public async Task<(bool Success, string Error, PurchaseResponseDto? Response)> PurchaseTicketsAsync(
        int eventId,
        PurchaseRequestDto request)
    {
        // 1. Load event
        var evenement = await _uow.Evenements.GetByIdAsync(eventId);

        if (evenement == null)
            return (false, "Event not found", null);

        // 2. Validate total tickets <= 5
        var totalDemande = request.Items.Sum(i => i.Quantite);
        if (totalDemande > 5)
            return (false, $"Maximum 5 tickets per purchase. You requested {totalDemande}.", null);

        // 3. Validate each item and collect available tickets
        var reservations = new List<(Billet Billet, BilletType Type)>();

        foreach (var item in request.Items)
        {
            var billetType = evenement.BilletTypes
                .FirstOrDefault(bt => bt.Id == item.BilletTypeId);

            if (billetType == null)
                return (false, $"Ticket type {item.BilletTypeId} not found for this event", null);

            var disponibles = await _uow.Billets
                .GetAvailableByTypeAsync(item.BilletTypeId, item.Quantite);

            var disponiblesList = disponibles.ToList();

            if (disponiblesList.Count < item.Quantite)
                return (false, $"Not enough tickets available for type '{billetType.Nom}'. " +
                               $"Requested: {item.Quantite}, Available: {disponiblesList.Count}", null);

            foreach (var billet in disponiblesList)
                reservations.Add((billet, billetType));
        }

        // 4. Reserve all tickets atomically
        var transactionId = Guid.NewGuid().ToString("N");
        var now = DateTime.UtcNow;

        foreach (var (billet, _) in reservations)
        {
            billet.Statut = "Reserve";
            billet.DateReservation = now;
            billet.VisiteurId = request.VisiteurId;
            billet.PaymentTransactionId = transactionId;
            billet.Code = _codeGenerator.Generate(eventId);
            _uow.Billets.Update(billet);
        }

        // 5. ONE save for everything
        await _uow.SaveChangesAsync();

        // 6. Build response
        var montantTotal = reservations.Sum(r => r.Type.Prix);

        var response = new PurchaseResponseDto
        {
            TransactionId = transactionId,
            EvenementId = eventId,
            VisiteurId = request.VisiteurId,
            TotalBillets = reservations.Count,
            MontantTotal = montantTotal,
            BilletsReserves = reservations.Select(r => new ReservedTicketDto
            {
                Id = r.Billet.Id,
                Code = r.Billet.Code,
                BilletTypeNom = r.Type.Nom,
                Prix = r.Type.Prix,
                Statut = r.Billet.Statut
            }).ToList()
        };

        return (true, string.Empty, response);
    }
}