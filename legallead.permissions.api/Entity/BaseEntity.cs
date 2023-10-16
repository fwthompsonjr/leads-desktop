using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace legallead.permissions.api.Entity
{
    public abstract class BaseEntity<T> where T : class, IDataEntity, new()
    {
        private const string dbFolder = "_db";
        private static readonly object _instance = new();
        private readonly string? _location;

        protected BaseEntity()
        {
            var name = typeof(T).Name.ToLower();
            Generate(name, this.IsProtected, ref _location);
        }
        protected abstract bool IsProtected { get; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }


        public void Add(T item)
        {
            _ = Insert(item);
        }

        public void Save(T item)
        {
            _ = Update(item);
        }

        public void Remove(T item)
        {
            _ = Delete(item);
        }

        public T? Get(Func<T, bool> expression)
        {
            return Find(expression);
        }

        public IEnumerable<T>? GetAll(Func<T, bool> expression)
        {
            return FindAll(expression);
        }

        private string Location => _location ?? string.Empty;

        protected T Insert(T entity)
        {
            if (string.IsNullOrEmpty(Location)) 
                throw new ArgumentNullException(nameof(entity), "Data file is missing or not initialized.");

            if (!string.IsNullOrEmpty(entity.Id)) { 
                throw new ArgumentOutOfRangeException(nameof(entity), "Id is not expected for data insert");
            }
            entity.Id = Guid.NewGuid().ToString();
            var table = GetContent();
            table.Add(entity);
            var content = JsonConvert.SerializeObject(table);
            SaveContent(content);
            return entity;
        }

        protected T Update(T entity)
        {
            if (string.IsNullOrEmpty(Location))
                throw new ArgumentNullException(nameof(entity), "Data file is missing or not initialized.");

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentOutOfRangeException(nameof(entity), "Id is required for data update");
            }
            var table = GetContent();
            var id = table.FindIndex(x => (x.Id ?? "").Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
            table[id] = entity;
            var content = JsonConvert.SerializeObject(table);
            SaveContent(content);
            return entity;
        }

        protected T? Find(Func<T, bool> expression)
        {
            if (string.IsNullOrEmpty(Location))
                throw new FileNotFoundException("Data file is missing or not initialized.");

            var table = GetContent();
            return table.Find(x => expression(x));
        }

        protected IEnumerable<T>? FindAll(Func<T, bool> expression)
        {
            if (string.IsNullOrEmpty(Location))
                throw new FileNotFoundException("Data file is missing or not initialized.");

            var table = GetContent();
            return table.FindAll(x => expression(x));
        }

        protected T? Delete(T entity)
        {
            if (string.IsNullOrEmpty(Location))
                throw new ArgumentNullException(nameof(entity), "Data file is missing or not initialized.");

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentOutOfRangeException(nameof(entity), "Id is required for data delete");
            }
            var table = GetContent();
            var found = table.FindIndex(x => (x.Id ?? "").Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
            if (found < 0) return null;
            table.RemoveAt(found);
            var content = JsonConvert.SerializeObject(table);
            SaveContent(content);
            return entity;
        }

        private List<T> GetContent()
        {
            lock (_instance)
            {
                var content = File.ReadAllText(Location);
                if (IsProtected)
                {
                    // decrypt 
                    var prefix = GetFileKeyName(Location);
                    content = CryptoEngine.Decrypt(content, prefix);
                }
                return JsonConvert.DeserializeObject<List<T>>(content) ?? new();
            }
        }
        private void SaveContent(string content)
        {
            lock (_instance)
            {
                if (IsProtected)
                {
                    // encrypt 
                    var prefix = GetFileKeyName(Location);
                    content = CryptoEngine.Encrypt(content, prefix);
                }
                File.WriteAllText(Location, content);
            }
        }

        private static void Generate(string entityName, bool isProtected, ref string? location)
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null || assembly.Location == null) return;
            var execName = new Uri(assembly.Location).AbsolutePath;
            if (execName != null && System.IO.File.Exists(execName))
            {
                var name = $"{entityName}.json";
                var contentRoot = Path.GetDirectoryName(execName) ?? "";
                var dataRoot = Path.Combine(contentRoot, dbFolder);
                var dataFile = Path.Combine(dataRoot, name);
                if (!System.IO.File.Exists(dataFile))
                {
                    lock (_instance)
                    {
                        var content = "[]";
                        if (isProtected)
                        {
                            // encrypt 
                            var prefix = GetFileKeyName(dataFile);
                            content = CryptoEngine.Encrypt(content, prefix);
                        }
                        System.IO.File.WriteAllText(dataFile, content);
                    }
                }
                location = dataFile;
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
