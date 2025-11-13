using FinTrackBack.Payments.Application.DTOs;
using FinTrackBack.Payments.Domain.Interfaces;

namespace FinTrackBack.Payments.Application.Features.GetPaymentsByUserId;

public class GetPaymentsByDocumentIdQueryHandler
{
    private readonly IPaymentRepository _repository;

    public GetPaymentsByDocumentIdQueryHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PaymentDto>> HandleAsync(GetPaymentsByDocumentIdQuery query)
    {
        var payments = await _repository.GetPaymentsByDocumentIdAsync(query.DocId);

        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            Servicio = p.Servicio,
            Monto = p.Monto,
            Fecha = p.Fecha,
            UserId = p.UserId,
            DocumentId = p.DocumentId
        });
    }
}
