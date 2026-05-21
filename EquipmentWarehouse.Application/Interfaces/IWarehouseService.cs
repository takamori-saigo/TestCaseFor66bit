using EquipmentWarehouse.Application.DTO;

namespace EquipmentWarehouse.Application.Interfaces;

public interface IWarehouseService
{
    Task<List<WareHouseItemDTO>> GetAllAsync();
    Task<WareHouseItemDTO> SaveAsync(CreateUpdateItemDto dto);
    Task DeleteAsync(Guid id);
    Task<List<Guid>> GetAvailableTypeIdsAsync(Guid? selectedSupplierId);
    Task<List<Guid>> GetAvailableSupplierIdsAsync(Guid? selectedTypeId);
}