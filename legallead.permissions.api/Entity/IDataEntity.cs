namespace legallead.permissions.api.Entity
{
    public interface IDataEntity
    {
        bool IsProtected { get; }
        string? Id { get; set; }
        string? Name { get; set; }
        bool IsDeleted { get; set; }
    }
}
