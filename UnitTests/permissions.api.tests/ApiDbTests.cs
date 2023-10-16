using legallead.permissions.api.Entity;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace permissions.api.tests
{
    public class ApiDbTests : IDisposable
    {
        private static readonly object locker = new();
        private bool disposedValue;
        private readonly legallead.permissions.api.DataProvider Provider;
        public ApiDbTests()
        {
            Provider = GetDataProvider();
        }

        [Fact]
        public void Db_Can_Add_Update_And_Delete()
        {
            var user = new UserEntity
            {
                Name = "Test",
                UserId = "john.smith@email.org",
                Pwd = "abcdefghijklmop"
            };
            Provider.Insert(user);
            Assert.NotNull(user.Id);
            user.Name = "Test Changed";
            Provider.Update(user);
            var item = Provider.Get(user, x => { return x.Id == user.Id; });
            Assert.NotNull(item);
            Assert.Equal(user.Name, item.Name);
            Provider.Delete(user);
        }


        [Fact]
        public void Db_Can_Add_And_FindAll()
        {
            var users = new List<UserEntity> {
                new UserEntity
                {
                    Name = "Test 1",
                    UserId = "test1.smith@email.org",
                    Pwd = "abcdefghijklmop"
                },
                new UserEntity
                {
                    Name = "Test 2",
                    UserId = "roger.smith@email.org",
                    Pwd = "abcdefghijklmop"
                },
                new UserEntity
                {
                    Name = "Test 3",
                    UserId = "test1.smith@email.org",
                    Pwd = "abcdefghijklmop"
                }
            };
            users.ForEach(u => Provider.Insert(u));

            var items = Provider.GetAll(users[0], x => { return x.UserId == users[0].UserId; });
            Assert.NotNull(items);
            Assert.Equal(2, items.Count());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DropDb();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        private static legallead.permissions.api.DataProvider GetDataProvider()
        {
            var config = GetConfiguration();
            return new legallead.permissions.api.DataProvider(config);
        }

        private static void DropDb()
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null || assembly.Location == null) { return; }
            var execName = new Uri(assembly.Location).AbsolutePath;
            if (execName == null || !File.Exists(execName)) { return; }

            var contentRoot = Path.GetDirectoryName(execName) ?? "";
            var dataRoot = Path.Combine(contentRoot, "_db");
            if (!Directory.Exists(dataRoot)) { return; }
            var files = Directory.GetFiles(dataRoot, "*.json");

            lock (locker)
            {
                foreach (var file in files) { File.Delete(file); }
            }
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .AddJsonFile("appsettings.Development.json")
                 .Build();
        }
    }
}