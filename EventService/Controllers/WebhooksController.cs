using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventService.Core.Interfaces;

namespace EventService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebhooksController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(IUnitOfWork uow, ILogger<WebhooksController> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    // POST api/webhooks/payment-confirmed
    // Called by PaymentService after a successful payment.
    // JWT must carry role=service (signed with the shared JWT secret).
    [HttpPost("payment-confirmed")]
    [Authorize]
    public async Task<IActionResult> PaymentConfirmed([FromBody] PaymentConfirmedWebhookDto dto)
    {
        var role = User.FindFirst("role")?.Value;

        if (role != "service")
            return StatusCode(403, new { message = "Service token required" });

        if (!int.TryParse(dto.UserId, out var userId) ||
            !int.TryParse(dto.EventId, out var eventId))
            return BadRequest(new { message = "Invalid UserId or EventId" });

        var billets = await _uow.Billets.GetReservedByUserAndEventAsync(userId, eventId);
        var billetList = billets.ToList();

        if (billetList.Count == 0)
        {
            _logger.LogWarning(
                "No reserved tickets found for user={UserId} event={EventId} txn={TxnId}",
                userId, eventId, dto.TransactionId);
            return Ok(new { confirmed = 0 });
        }

        foreach (var billet in billetList)
        {
            billet.Statut = "Confirme";
            billet.DateValidation = DateTime.UtcNow;
            _uow.Billets.Update(billet);
        }

        await _uow.SaveChangesAsync();

        _logger.LogInformation(
            "Confirmed {Count} ticket(s) for user={UserId} event={EventId} txn={TxnId}",
            billetList.Count, userId, eventId, dto.TransactionId);

        return Ok(new { confirmed = billetList.Count });
    }
}

public class PaymentConfirmedWebhookDto
{
    public string TransactionId { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public DateTime ConfirmedAt { get; set; }
}
