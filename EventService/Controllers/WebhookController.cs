using Microsoft.AspNetCore.Mvc;
using EventService.Core.Interfaces;

namespace EventService.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhookController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(IUnitOfWork uow, ILogger<WebhookController> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    // ── POST /api/webhooks/payment-confirmed ─────────────────────────────────
    // Called by the Payment Service after PayPal/Stripe payment succeeds
    [HttpPost("payment-confirmed")]
    public async Task<IActionResult> PaymentConfirmed([FromBody] PaymentConfirmedDto dto)
    {
        _logger.LogInformation(
            "Payment confirmed webhook received: txn={TxnId} event={EventId} user={UserId} provider={Provider}",
            dto.TransactionId, dto.EventId, dto.UserId, dto.Provider);

        if (!int.TryParse(dto.EventId, out var eventId) ||
            !int.TryParse(dto.UserId,  out var userId))
        {
            _logger.LogWarning(
                "Invalid EventId or UserId in payment-confirmed webhook: event={EventId} user={UserId}",
                dto.EventId, dto.UserId);
            return BadRequest(new { message = "Invalid EventId or UserId" });
        }

        // Find all Reserved tickets for this user + event
        var billets = (await _uow.Billets.GetReservedByUserAndEventAsync(userId, eventId)).ToList();

        if (billets.Count == 0)
        {
            _logger.LogWarning(
                "No reserved tickets found for user={UserId} event={EventId}", userId, eventId);
            // Return 200 anyway — idempotent
            return Ok(new { message = "No reserved tickets found", confirmed = 0 });
        }

        // Mark all as Confirme
        foreach (var billet in billets)
        {
            billet.Statut = "Confirme";
            _uow.Billets.Update(billet);
        }

        await _uow.SaveChangesAsync();

        _logger.LogInformation(
            "Confirmed {Count} tickets for user={UserId} event={EventId}",
            billets.Count, userId, eventId);

        return Ok(new { message = "Tickets confirmed", confirmed = billets.Count });
    }
}

// ── DTO for incoming webhook body ────────────────────────────────────────────
public class PaymentConfirmedDto
{
    public Guid   TransactionId { get; set; }
    public string EventId       { get; set; } = string.Empty;
    public string UserId        { get; set; } = string.Empty;
    public decimal Amount       { get; set; }
    public string Currency      { get; set; } = string.Empty;
    public string Provider      { get; set; } = string.Empty;
    public DateTime ConfirmedAt { get; set; }
}
