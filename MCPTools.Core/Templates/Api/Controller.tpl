namespace {{Namespace}}.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using {{Namespace}}.Application.Managers;
using {{Namespace}}.Application.Requests;
using {{Namespace}}.Application.Responses;

/// <summary>
/// Provides API endpoints for {{PluralEntityName}}.
/// </summary>
[ApiController]
[ApiVersion("{{ApiVersion}}")]
[Route("api/v{version:apiVersion}/{{Route}}")]
public sealed class {{ControllerName}} : ControllerBase
{
    private readonly I{{ManagerName}} _manager;

    /// <summary>
    /// Initializes a new instance of the <see cref="{{ControllerName}}"/> class.
    /// </summary>
    public {{ControllerName}}(I{{ManagerName}} manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// Gets all {{PluralEntityName}}.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<{{EntityName}}Response>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<{{EntityName}}Response>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _manager.GetAllAsync(cancellationToken);
        return result.Success ? Ok(result.Value) : BadRequest(result.Errors);
    }

    /// <summary>
    /// Gets a {{EntityName}} by its primary key.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof({{EntityName}}Response), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<{{EntityName}}Response>> GetByIdAsync({{PrimaryKeyType}} id, CancellationToken cancellationToken)
    {
        var result = await _manager.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result.Value) : NotFound(result.Errors);
    }

    /// <summary>
    /// Creates a new {{EntityName}}.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof({{PrimaryKeyType}}), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<{{PrimaryKeyType}}>> CreateAsync(Create{{EntityName}}Request request, CancellationToken cancellationToken)
    {
        var result = await _manager.CreateAsync(request, cancellationToken);
        return result.Success
            ? CreatedAtAction(nameof(GetByIdAsync), new { id = result.Value, version = "{{ApiVersion}}" }, result.Value)
            : BadRequest(result.Errors);
    }

    /// <summary>
    /// Updates an existing {{EntityName}}.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync({{PrimaryKeyType}} id, Update{{EntityName}}Request request, CancellationToken cancellationToken)
    {
        var updateRequest = request with { {{PrimaryKey}} = id };
        var result = await _manager.UpdateAsync(updateRequest, cancellationToken);
        return result.Success ? NoContent() : NotFound(result.Errors);
    }

    /// <summary>
    /// Deletes a {{EntityName}}.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync({{PrimaryKeyType}} id, CancellationToken cancellationToken)
    {
        var result = await _manager.DeleteAsync(id, cancellationToken);
        return result.Success ? NoContent() : NotFound(result.Errors);
    }
}
