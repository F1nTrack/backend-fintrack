namespace FinTrackBack.Documents.Application.Features.GetDocumentByUserId;
using MediatR;
using FinTrackBack.Documents.Domain.Interfaces;
using FinTrackBack.Documents.Application.DTOs;

public class GetDocumentsByUserIdHandler : IRequestHandler<GetDocumentsByUserIdQuery, List<DocumentDto>>
{
    private readonly IDocumentRepository _repository;

    public GetDocumentsByUserIdHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DocumentDto>> Handle(GetDocumentsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var docs = await _repository.GetByUserIdAsync(request.UserId);

        return docs.Select(d => new DocumentDto
        {
            Id = d.Id,
            UserId = d.UserId,
            DocumentNumber = d.DocumentNumber,
            FullName = d.FullName,
            Type = d.Type,
            Status = d.Status,
            IssueDate = d.IssueDate,
            ExpirationDate = d.ExpirationDate,
            FilePath = d.FilePath,
            Balance = d.Balance
        }).ToList();
    }
}