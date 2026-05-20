using EquipmentWarehouse.Application.DTO;
using EquipmentWarehouse.Application.Interfaces;
using EquipmentWarehouse.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EquipmentWarehouse.Infrastructure;

public class WarehouseService: IWarehouseService
{
    private readonly dbContext _context;
    private readonly IHubContext<WarehouseHub> _hubContext;
    public WarehouseService(dbContext context, IHubContext<WarehouseHub> hub)
    {
        _context = context;
        _hubContext = hub;
    }
    
    public async Task<List<WareHouseItemDTO>> GetAllAsync()
    {
        var items = await _context.WareHouseItem
            .Include(x => x.EquipmentType)
            .Include(x => x.Supplier)
            .Include(x => x.CountryOfItem)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
        return items.Select(MakeWareHouseItemDto).ToList();
    }

    public async Task<WareHouseItemDTO> SaveAsync(CreateUpdateItemDto dto)
    {
        var country = await _context
            .Countries.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == dto.CountryName.Trim().ToLower());
        if (country == null)
        {
            country = new Country
            {
                Id = Guid.NewGuid(),
                Name = dto.CountryName.Trim()
            };
            _context.Countries.Add(country);
        }

        var entity = new WareHouseItem();
        if (dto.Id.HasValue && dto.Id != Guid.Empty)
        {
            entity = await _context.WareHouseItem
                .FirstOrDefaultAsync(x => x.Id == dto.Id.Value);
            entity.Model = dto.Model;
            entity.Price = dto.Price; 
            entity.PhotoUrl = dto.PhotoUrl;
            entity.TypeId = dto.TypeId;
            entity.SupplierId = dto.SupplierId;
            entity.CountryId = country.Id;
            _context.WareHouseItem.Update(entity);
        }
        else
        {
            entity.Id = Guid.NewGuid();
            entity.Model = dto.Model;
            entity.Price = dto.Price;
            entity.PhotoUrl = dto.PhotoUrl;
            entity.TypeId = dto.TypeId;
            entity.SupplierId = dto.SupplierId;
            entity.CountryId = country.Id;
            await _context.WareHouseItem.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
        var result = MakeWareHouseItemDto(entity);
        await _hubContext.Clients.All.SendAsync("ReceiveItem", result);
        return result;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = _context.WareHouseItem.Find(id);
        if (entity != null)
        {
            _context.WareHouseItem.Remove(entity);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveItem", id);
        }
    }

    public async Task<List<Guid>> GetAvailableTypeIdsAsync(Guid? selectedSupplierId)
    {
        if (!selectedSupplierId.HasValue) return new List<Guid>();
        return await _context.WareHouseItem
            .Where(x => x.SupplierId == selectedSupplierId.Value)
            .Select(x => x.Id)
            .ToListAsync();
    }

    public async Task<List<Guid>> GetAvailableSupplierIdsAsync(Guid? selectedTypeId)
    {
        if (!selectedTypeId.HasValue) return new List<Guid>();
        return await _context.WareHouseItem
            .Where(x => x.TypeId == selectedTypeId.Value)
            .Select(x => x.Id)
            .ToListAsync();
    }

    private WareHouseItemDTO MakeWareHouseItemDto(WareHouseItem wareHouseItem)
    {
        return new WareHouseItemDTO()
        {
            Id = wareHouseItem.Id,
            Model = wareHouseItem.Model,
            Price = wareHouseItem.Price,
            PhotoUrl = wareHouseItem.PhotoUrl,
            CreationDate = wareHouseItem.CreatedDate,
            LastUpdateDate = wareHouseItem.ModifiedDate,
            Type = new ReferenceDTO
            {
                Id = wareHouseItem.EquipmentType?.Id ?? Guid.Empty,
                Name = wareHouseItem.EquipmentType?.Name ?? string.Empty,
            },
            Supplier = new ReferenceDTO
            {
                Id = wareHouseItem.Supplier?.Id ?? Guid.Empty,
                Name = wareHouseItem.Supplier?.Name ?? string.Empty,
            },
            Country = new ReferenceDTO
            {
                Id = wareHouseItem.CountryOfItem?.Id ?? Guid.Empty,
                Name = wareHouseItem.CountryOfItem?.Name ?? string.Empty,
            }
        };
    }
}
