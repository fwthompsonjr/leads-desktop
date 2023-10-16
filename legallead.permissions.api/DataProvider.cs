using legallead.permissions.api.Entity;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace legallead.permissions.api
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", 
        "CA1822:Mark members as static", Justification = "Exposing generic static methods as public to follow DI pattern")]

    public class DataProvider
    {
        private const string dbFolder = "_db";
        private const string typeAssignMessage = "improper assignment of typename argument.";
        private static readonly object _instance = new();

        private readonly IConfiguration _config;
        private static readonly Dictionary<string, string> _data = new();
        private static readonly Dictionary<string, string> _dataKeys = new();
        private static readonly Dictionary<string, string> _dataFiles = new();
        public DataProvider(IConfiguration configuration)
        {
            _config = configuration;
            Initialize();
        }

        public T Insert<T>(T entity) where T : class, IDataEntity, new()
        {
            if (entity is not BaseEntity<T> baseDto)
                throw new TypeAccessException(typeAssignMessage);
            
            var key = typeof(T).Name.ToLower();
            if (!_dataFiles.ContainsKey(key))
                throw new FileNotFoundException();
            var data = _dataFiles[key];
            baseDto.Add(entity, data);
            return entity;
        }

        public T Update<T>(T entity) where T : BaseEntity<T>, IDataEntity, new()
        {
            if (entity is not BaseEntity<T> baseDto)
                throw new TypeAccessException(typeAssignMessage);
            
            var key = typeof(T).Name.ToLower();
            if (!_dataFiles.ContainsKey(key))
                throw new FileNotFoundException();
            
            var data = _dataFiles[key];
            baseDto.Save(entity, data);
            return entity;
        }

        public T Delete<T>(T entity) where T : BaseEntity<T>, IDataEntity, new()
        {
            if (entity is not BaseEntity<T> baseDto)
                throw new TypeAccessException(typeAssignMessage);
            
            var key = typeof(T).Name.ToLower();
            if (!_dataFiles.ContainsKey(key))
                throw new FileNotFoundException();
            var data = _dataFiles[key];

            baseDto.Remove(entity, data);

            return entity;
        }



        public T? Get<T>(T entity, Func<T, bool> expression)
            where T : BaseEntity<T>, IDataEntity, new()
        {
            if (entity is not BaseEntity<T> baseDto)
                throw new TypeAccessException(typeAssignMessage);

            var key = typeof(T).Name.ToLower();
            if (!_dataFiles.ContainsKey(key))
                throw new FileNotFoundException();
            var data = _dataFiles[key];

            return baseDto.Get(data, expression);
        }

        public IEnumerable<T>? GetAll<T>(T entity, Func<T, bool> expression)
            where T : BaseEntity<T>, IDataEntity, new()
        {
            if (entity is not BaseEntity<T> baseDto)
                throw new TypeAccessException(typeAssignMessage);

            var key = typeof(T).Name.ToLower();
            if (!_dataFiles.ContainsKey(key))
                throw new FileNotFoundException();
            var data = _dataFiles[key];

            return baseDto.GetAll(data, expression);
        }


        private void Generate(string entityName, bool isProtected, ref string? location)
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


        private string GetOrInitializeVariable(string name)
        {
            if (_data.ContainsKey(name)) { return _data[name]; }
            var prefix = _config.GetValue<string>(name);
            if (prefix != null && Guid.TryParse(prefix, out var _))
            {
                var code = GetGuidCode(name, prefix);
                _data.Add(name, code);
                return code;
            }
            var guid = Guid.NewGuid().ToString("D");
            var tmpCode = GetGuidCode(name, guid);
            _data.Add(name, tmpCode);
            return tmpCode;
        }



        private static string GetGuidCode(string name, string prefix)
        {
            var randoms = prefix.Split('-');
            var item = string.Concat(randoms[0], randoms[randoms.Length - 1]);
            Environment.SetEnvironmentVariable(name, item);
            return item;
        }
        private string GetFileKeyName(string dataFile)
        {
            const string dataKey = "API_DATA_PFX";

            if (_dataKeys.ContainsKey(dataKey)) { return _dataKeys[dataKey]; }

            var prefix = GetOrInitializeVariable(dataKey);
            var additionalData = $"{prefix}-{Path.GetFileNameWithoutExtension(dataFile)}";
            var sb = new StringBuilder(additionalData);
            while (sb.Length < 64)
            {
                sb.Append('!');
            }
            var data = sb.ToString();
            _dataKeys.Add(dataKey, data);
            return data;
        }

        private void Initialize()
        {
            var type = typeof(IDataEntity);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface)
                .Select(s => new {  Name = s.Name.ToLowerInvariant(), type = s})
                .ToList();
            types.ForEach(t =>
            {
                var typeName = t.Name.ToLowerInvariant();
                if (Activator.CreateInstance(t.type) is IDataEntity instance)
                {
                    var location = GetFileLocation(typeName, instance.IsProtected);
                    Generate(typeName, instance.IsProtected, ref location);
                    location ??= string.Empty;
                    if (_dataFiles.ContainsKey(t.Name))
                    {
                        _dataFiles.Remove(t.Name);
                    }
                    _dataFiles.Add(t.Name, location);
                }
            });
        }

        private string GetFileLocation(string typeName, bool isProtected)
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null || assembly.Location == null) return string.Empty;
            var execName = new Uri(assembly.Location).AbsolutePath;
            if (execName != null && System.IO.File.Exists(execName))
            {

                var name = $"{typeName}.json";
                var contentRoot = Path.GetDirectoryName(execName) ?? "";
                var dataRoot = Path.Combine(contentRoot, dbFolder);
                var dataFile = Path.Combine(dataRoot, name); if (!System.IO.File.Exists(dataFile))
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
                return dataFile;
            }
            return string.Empty;
        }
    }
}
