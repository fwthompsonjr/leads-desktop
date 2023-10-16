namespace legallead.permissions.api.Entity
{
    public class ApplicationEntity : BaseEntity<ApplicationEntity>, IDataEntity
    {
        protected override bool IsProtected => true;

        bool IDataEntity.IsProtected => IsProtected;
    }
}