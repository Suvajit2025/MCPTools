namespace {{Namespace}}.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using {{Namespace}}.Application.Dtos;
using {{Namespace}}.Application.Managers;

/// <summary>
/// Provides API endpoints for {{ModelName}}.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class {{ModelName}}Controller : ControllerBase
{
    private readonly I{{ModelName}}Manager _manager;

    /// <summary>
    /// Initializes a new instance of the <see cref="{{ModelName}}Controller"/> class.
    /// </summary>
    public {{ModelName}}Controller(I{{ModelName}}Manager manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// Gets all {{ModelName}} records.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<{{ModelName}}Response>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _manager.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a {{ModelName}} record by its primary key.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<{{ModelName}}Response>> GetByIdAsync({{PrimaryKeyType}} id, CancellationToken cancellationToken)
    {
        var result = await _manager.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new {{ModelName}} record.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateAsync(Create{{ModelName}}Request request, CancellationToken cancellationToken)
    {
        var affectedRows = await _manager.CreateAsync(request, cancellationToken);
        return affectedRows > 0 ? Ok() : BadRequest();
    }

    /// <summary>
    /// Updates an existing {{ModelName}} record.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync({{PrimaryKeyType}} id, Update{{ModelName}}Request request, CancellationToken cancellationToken)
    {
        var affectedRows = await _manager.UpdateAsync(id, request, cancellationToken);
        return affectedRows > 0 ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a {{ModelName}} record by its primary key.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync({{PrimaryKeyType}} id, CancellationToken cancellationToken)
    {
        var affectedRows = await _manager.DeleteAsync(id, cancellationToken);
        return affectedRows > 0 ? NoContent() : NotFound();
    }
}
