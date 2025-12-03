using MediatR;
using FinTrackBack.Documents.Application.DTOs;
using System.Collections.Generic;

namespace FinTrackBack.Documents.Application.Features.GetDocumentByUserId;

public class GetDocumentsByUserIdQuery : IRequest<List<DocumentDto>>
{
    public Guid UserId { get; set; }

    public GetDocumentsByUserIdQuery(Guid userId)
    {
        UserId = userId;
    }
}