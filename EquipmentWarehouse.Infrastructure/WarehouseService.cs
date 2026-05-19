using EquipmentWarehouse.Application.DTO;
using EquipmentWarehouse.Application.Interfaces;
using EquipmentWarehouse.Domain;
using Microsoft.AspNetCore.SignalR;

namespace EquipmentWarehouse.Infrastructure;

public class WarehouseService: IWarehouseService
{
    public WarehouseService(dbContext context, IHubContext<WarehouseHub> hub)
    {
        
    }
    
    public Task<List<WareHouseItemDTO>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<WareHouseItemDTO> SaveAsync(CreateUpdateItemDto dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Guid>> GetAvailableTypeIdsAsync(Guid? selectedSupplierId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Guid>> GetAvailableSupplierIdsAsync(Guid? selectedTypeId)
    {
        throw new NotImplementedException();
    }
}
