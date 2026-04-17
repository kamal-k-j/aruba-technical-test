using Aruba.Document.Api.Documents.Requests;
using Aruba.Document.Api.Documents.Responses;
using Aruba.Document.Application.Documents.Commands.Approve;
using Aruba.Document.Application.Documents.Commands.Complete;
using Aruba.Document.Application.Documents.Commands.Generate;
using Aruba.Document.Application.Documents.Commands.Insert;
using Aruba.Document.Application.Documents.Commands.Link;
using Aruba.Document.Application.Documents.Commands.Reject;
using Aruba.Document.Application.Documents.Commands.Send;
using Aruba.Document.Application.Documents.Commands.Update;
using Aruba.Document.Application.Documents.Queries.GetAll;
using Aruba.Document.Application.Documents.Queries.GetById;
using Aruba.Document.Application.Documents.Queries.Search;
using Aruba.Document.Domain.Enums;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aruba.Document.Api.Documents;

[Authorize]
[ApiController]
[Route("documents")]
[Produces("application/json")]
[Consumes("application/json")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public DocumentsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DocumentResponse>> GetById(string id)
    {
        var documentResult = await _mediator.Send(new GetDocumentByIdQuery { Id = id });

        return Ok(_mapper.Map<DocumentResponse>(documentResult));
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(List<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DocumentResponse>>> GetAll()
    {
        var documentsResult = await _mediator.Send(new GetAllDocumentsQuery());

        return Ok(_mapper.Map<List<DocumentResponse>>(documentsResult));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DocumentResponse>> Update(string id, [FromBody] UpdateDocumentRequest payload)
    {
        var command = _mapper.Map<UpdateDocumentCommand>(payload) with { Id = id };

        var documentResult = await _mediator.Send(command);

        return Ok(_mapper.Map<DocumentResponse>(documentResult));
    }

    [HttpPost]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DocumentResponse>> Insert([FromBody] InsertDocumentRequest payload)
    {
        var command = _mapper.Map<InsertDocumentCommand>(payload);

        var documentResult = await _mediator.Send(command);

        return Ok(_mapper.Map<DocumentResponse>(documentResult));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DocumentResponse>>> Search([FromQuery] string field, [FromQuery] string value)
    {
        var query = new SearchDocumentsQuery
        {
            Field = field,
            Value = value
        };

        var documents = await _mediator.Send(query);

        return Ok(_mapper.Map<List<DocumentResponse>>(documents));
    }

    [HttpPost("{id}/generate")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DocumentResponse>> Generate(string id, [FromQuery] DocumentType type)
    {
        var result = await _mediator.Send(new GenerateDocumentCommand { Id = id, Type = type });

        return Ok(_mapper.Map<DocumentResponse>(result));
    }

    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DocumentResponse>> Complete(string id)
    {
        var result = await _mediator.Send(new CompleteDocumentCommand { Id = id });

        return Ok(_mapper.Map<DocumentResponse>(result));
    }

    [HttpPost("{id}/send")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DocumentResponse>> Send(string id)
    {
        var result = await _mediator.Send(new SendDocumentCommand { Id = id });

        return Ok(_mapper.Map<DocumentResponse>(result));
    }

    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DocumentResponse>> Approve(string id)
    {
        var result = await _mediator.Send(new ApproveDocumentCommand { Id = id });

        return Ok(_mapper.Map<DocumentResponse>(result));
    }

    [HttpPost("{id}/reject")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DocumentResponse>> Reject(string id)
    {
        var result = await _mediator.Send(new RejectDocumentCommand { Id = id });

        return Ok(_mapper.Map<DocumentResponse>(result));
    }

    [HttpPost("{id}/link/{targetId}")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DocumentResponse>> Link(string id, string targetId)
    {
        var result = await _mediator.Send(new LinkDocumentCommand { Id = id, TargetId = targetId });

        return Ok(_mapper.Map<DocumentResponse>(result));
    }
}