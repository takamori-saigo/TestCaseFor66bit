namespace EquipmentWarehouse.Domain;

public abstract class CommonEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}