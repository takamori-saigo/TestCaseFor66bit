namespace EquipmentWarehouse.Domain;

public class Supplier: CommonEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public ICollection<WareHouseItem> WareHouseItems { get; set; } = new List<WareHouseItem>();
}