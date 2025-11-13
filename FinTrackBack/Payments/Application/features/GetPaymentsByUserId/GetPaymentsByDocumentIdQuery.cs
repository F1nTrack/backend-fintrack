namespace FinTrackBack.Payments.Application.Features.GetPaymentsByUserId;
/// <summary>
/// Consulta para obtener pagos de un documento específico.
/// </summary>
public class GetPaymentsByDocumentIdQuery
{
    public Guid DocId { get; set; }
}