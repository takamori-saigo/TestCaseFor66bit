namespace EquipmentWarehouse.Application.DTO;

public class CreateUpdateItemDto
{
    public Guid? Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public Guid TypeId { get; set; }
    public Guid SupplierId { get; set; }
    public string CountryName { get; set; } = string.Empty;
}