namespace EquipmentWarehouse.Domain;

public class WareHouseItem: CommonEntity
{
    public string PhotoUrl { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    public Guid TypeId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid CountryId { get; set; }
    
    public EquipmentType EquipmentType { get; set; } 
    public Supplier Supplier { get; set; }
    public Country CountryOfItem { get; set; } 
}