using System.Threading.Tasks;
using FinTrackBack.Documents.Application.DTOs;
using FinTrackBack.Documents.Application.Features.CreateDocument;
using FinTrackBack.Documents.Application.Features.GetDocumentByUserId;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinTrackBack.Documents.Interfaces.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<DocumentDto>> Create([FromBody] CreateDocumentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<DocumentDto>>> GetByUserId(Guid userId)
        {
            var query = new GetDocumentsByUserIdQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}