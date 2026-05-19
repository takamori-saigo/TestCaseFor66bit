namespace EquipmentWarehouse.Application.DTO;

public class WareHouseItemDTO
{
    public Guid Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public ReferenceDTO Type { get; set; } = new();
    public ReferenceDTO Supplier { get; set; } = new();
    public ReferenceDTO Country { get; set; } = new();
    public DateTime CreationDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
}