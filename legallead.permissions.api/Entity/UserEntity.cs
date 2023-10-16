namespace legallead.permissions.api.Entity
{
    public class UserEntity : BaseEntity<UserEntity>, IDataEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Pwd { get; set; } = string.Empty;

        protected override bool IsProtected => true;

        bool IDataEntity.IsProtected => true;
    }
}
