using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace legallead.permissions.api.Entity
{
    public abstract class BaseEntity<T> where T : class, IDataEntity, new()
    {
        private const string dbFolder = "_db";
        private static readonly object _instance = new();

        protected abstract bool IsProtected { get; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }


        public void Add(T item, string location)
        {
            _ = Insert(item, location);
        }

        public void Save(T item, string location)
        {
            _ = Update(item, location);
        }

        public void Remove(T item, string location)
        {
            _ = Delete(item, location);
        }

        public T? Get(string location, Func<T, bool> expression)
        {
            return Find(location, expression);
        }

        public IEnumerable<T>? GetAll(string location, Func<T, bool> expression)
        {
            return FindAll(location, expression);
        }

        protected T Insert(T entity, string location)
        {
            if (string.IsNullOrEmpty(location)) 
                throw new ArgumentNullException(nameof(location), "Data file is missing or not initialized.");

            if (!string.IsNullOrEmpty(entity.Id)) { 
                throw new ArgumentOutOfRangeException(nameof(entity), "Id is not expected for data insert");
            }
            entity.Id = Guid.NewGuid().ToString();
            var table = GetContent(location);
            table.Add(entity);
            var content = JsonConvert.SerializeObject(table);
            SaveContent(content, location);
            return entity;
        }

        protected T Update(T entity, string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException(nameof(location), "Data file is missing or not initialized.");

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentOutOfRangeException(nameof(entity), "Id is required for data update");
            }
            var table = GetContent(location);
            var id = table.FindIndex(x => (x.Id ?? "").Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
            table[id] = entity;
            var content = JsonConvert.SerializeObject(table);
            SaveContent(content, location);
            return entity;
        }

        protected T? Find(string location, Func<T, bool> expression)
        {
            if (string.IsNullOrEmpty(location))
                throw new FileNotFoundException("Data file is missing or not initialized.");

            var table = GetContent(location);
            return table.Find(x => expression(x));
        }

        protected IEnumerable<T>? FindAll(string location, Func<T, bool> expression)
        {
            if (string.IsNullOrEmpty(location))
                throw new FileNotFoundException("Data file is missing or not initialized.");

            var table = GetContent(location);
            return table.FindAll(x => expression(x));
        }

        protected T? Delete(T entity, string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException(nameof(entity), "Data file is missing or not initialized.");

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentOutOfRangeException(nameof(entity), "Id is required for data delete");
            }
            var table = GetContent(location);
            var found = table.FindIndex(x => (x.Id ?? "").Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
            if (found < 0) return null;
            table.RemoveAt(found);
            var content = JsonConvert.SerializeObject(table);
            SaveContent(content, location);
            return entity;
        }

        private List<T> GetContent(string location)
        {
            lock (_instance)
            {
                var content = File.ReadAllText(location);
                if (IsProtected)
                {
                    // decrypt 
                    var prefix = GetFileKeyName(location);
                    content = CryptoEngine.Decrypt(content, prefix);
                }
                return JsonConvert.DeserializeObject<List<T>>(content) ?? new();
            }
        }
        private void SaveContent(string content, string location)
        {
            lock (_instance)
            {
                if (IsProtected)
                {
                    // encrypt 
                    var prefix = GetFileKeyName(location);
                    content = CryptoEngine.Encrypt(content, prefix);
                }
                File.WriteAllText(location, content);
            }
        }

        private static string GetOrInitializeVariable(string name)
        {
            var prefix = Environment.GetEnvironmentVariable(name);
            if (prefix != null) { return prefix; }
            var guid = Guid.NewGuid().ToString("D");
            var randoms = guid.Split('-');
            var item = string.Concat(randoms[0], randoms[randoms.Length - 1]);
            Environment.SetEnvironmentVariable(name, item);
            return item;
        }

        private static string GetFileKeyName(string dataFile)
        {

            var prefix = GetOrInitializeVariable("API_DATA_PFX");
            var additionalData = $"{prefix}-{Path.GetFileNameWithoutExtension(dataFile)}";
            var sb = new StringBuilder(additionalData);
            while (sb.Length < 64)
            {
                sb.Append('!');
            }
            return sb.ToString();
        }
    }
}
