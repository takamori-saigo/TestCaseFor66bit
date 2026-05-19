namespace EquipmentWarehouse.Domain;

public class Country
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<WareHouseItem> WarehouseItems { get; set; } = new List<WareHouseItem>();
}