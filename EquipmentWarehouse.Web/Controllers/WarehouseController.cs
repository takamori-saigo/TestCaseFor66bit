using EquipmentWarehouse.Domain;
using EquipmentWarehouse.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquipmentWarehouse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReferencesController : ControllerBase
{
    private readonly dbContext _context;

    public ReferencesController(dbContext context)
    {
        _context = context;
    }

    [HttpGet("types")]
    public async Task<ActionResult<List<object>>> GetTypes()
    {
        var types = await _context.EquipmentTypes
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name, x.Description, x.CreatedDate, x.ModifiedDate })
            .ToListAsync();
        return Ok(types);
    }

    [HttpPost("types")]
    public async Task<ActionResult<object>> SaveType([FromBody] SaveReferenceRequest request)
    {
        EquipmentType entity;
        if (request.Id.HasValue && request.Id != Guid.Empty)
        {
            entity = await _context.EquipmentTypes.FindAsync(request.Id.Value);
            if (entity == null) return NotFound();
            entity.Name = request.Name;
            entity.Description = request.Description;
        }
        else
        {
            entity = new EquipmentType
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };
            _context.EquipmentTypes.Add(entity);
        }
        await _context.SaveChangesAsync();
        return Ok(new { entity.Id, entity.Name, entity.Description, entity.CreatedDate, entity.ModifiedDate });
    }

    [HttpDelete("types/{id}")]
    public async Task<IActionResult> DeleteType(Guid id)
    {
        var entity = await _context.EquipmentTypes.FindAsync(id);
        if (entity == null) return NotFound();
        _context.EquipmentTypes.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("suppliers")]
    public async Task<ActionResult<List<object>>> GetSuppliers()
    {
        var suppliers = await _context.Supplier
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name, x.Description, x.CreatedDate, x.ModifiedDate })
            .ToListAsync();
        return Ok(suppliers);
    }

    [HttpPost("suppliers")]
    public async Task<ActionResult<object>> SaveSupplier([FromBody] SaveReferenceRequest request)
    {
        Supplier entity;
        if (request.Id.HasValue && request.Id != Guid.Empty)
        {
            entity = await _context.Supplier.FindAsync(request.Id.Value);
            if (entity == null) return NotFound();
            entity.Name = request.Name;
            entity.Description = request.Description;
        }
        else
        {
            entity = new Supplier
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };
            _context.Supplier.Add(entity);
        }
        await _context.SaveChangesAsync();
        return Ok(new { entity.Id, entity.Name, entity.Description, entity.CreatedDate, entity.ModifiedDate });
    }

    [HttpDelete("suppliers/{id}")]
    public async Task<IActionResult> DeleteSupplier(Guid id)
    {
        var entity = await _context.Supplier.FindAsync(id);
        if (entity == null) return NotFound();
        _context.Supplier.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("countries")]
    public async Task<ActionResult<List<object>>> GetCountries()
    {
        var countries = await _context.Countries
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();
        return Ok(countries);
    }
}

public class SaveReferenceRequest
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
