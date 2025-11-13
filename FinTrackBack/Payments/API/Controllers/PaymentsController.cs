using FinTrackBack.Payments.Application.DTOs;
using FinTrackBack.Payments.Application.Features.CreatePayment;
using FinTrackBack.Payments.Application.Features.GetPaymentsByUserId;
using FinTrackBack.Payments.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinTrackBack.Payments.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly CreatePaymentCommandHandler _createHandler;
        private readonly GetPaymentsByUserIdQueryHandler _getHandler;
        private readonly GetPaymentsByDocumentIdQueryHandler _getByDocumentIdHandler;

        public PaymentsController(IPaymentRepository paymentRepository)
        {
            _createHandler = new CreatePaymentCommandHandler(paymentRepository);
            _getHandler = new GetPaymentsByUserIdQueryHandler(paymentRepository);
            _getByDocumentIdHandler = new GetPaymentsByDocumentIdQueryHandler(paymentRepository);
        }

        /// <summary>
        /// Obtiene todos los pagos de un usuario específico.
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByUserId(Guid userId)
        {
            var result = await _getHandler.HandleAsync(new GetPaymentsByUserIdQuery { UserId = userId });

            if (!result.Any())
                return NotFound($"No se encontraron pagos para el usuario con ID {userId}.");

            return Ok(result);
        }
        
        /// <summary>
        /// Obtiene todos los pagos de un usuario específico.
        /// </summary>
        [HttpGet("document/{docId:guid}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByDocumentId(Guid docId)
        {
            var result = await _getByDocumentIdHandler.HandleAsync(new GetPaymentsByDocumentIdQuery { DocId = docId });

            if (!result.Any())
                return NotFound($"No se encontraron pagos para el usuario con ID {docId}.");

            return Ok(result);
        }

        /// <summary>
        /// Crea un nuevo pago.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreatePayment([FromBody] PaymentDto dto)
        {
            if (dto.Monto <= 0)
                return BadRequest("El monto debe ser mayor a cero.");

            var command = new CreatePaymentCommand { Payment = dto };
            var newId = await _createHandler.HandleAsync(command);

            return CreatedAtAction(nameof(GetPaymentsByUserId),
                new { userId = dto.UserId },
                new { id = newId, message = "Pago creado correctamente." });
        }
    }
}