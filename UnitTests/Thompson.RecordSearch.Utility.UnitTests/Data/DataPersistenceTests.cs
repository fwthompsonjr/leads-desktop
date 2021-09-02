using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Db;

namespace Thompson.RecordSearch.Utility.UnitTests.Data
{
    [TestClass]
    public class DataPersistenceTests
    {
        private class TmpData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreateDate { get; set; }

            public string FileName { get; set; }
        }

        private Faker<TmpData> Faker;

        [TestInitialize]
        public void Setup()
        {
            if (Faker == null)
            {
                Faker = new Faker<TmpData>()
                    .RuleFor(f => f.Id, r => r.IndexFaker)
                    .RuleFor(f => f.Name, r => r.Person.FullName)
                    .RuleFor(f => f.CreateDate, r => r.Date.Recent(7))
                    .RuleFor(f => f.FileName, r => r.System.FileName("temp"));
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            var path = DataPersistence.DataFolder;
            try
            {
                if (!Directory.Exists(path)) return;
                var files = new DirectoryInfo(path).GetFiles("*.temp").ToList();
                if (!files.Any()) return;
                // files.ForEach(f => File.Delete(f.FullName));
            }
            catch (Exception)
            {
                // no action in cleanup text error
            }
        }

        [TestMethod]
        public void CanGet_Folder()
        {
            var folder = DataPersistence.DataFolder;
            Directory.Exists(folder).ShouldBeTrue();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_RequiresAFileName()
        {
            var obj = Faker.Generate();
            DataPersistence.Save(string.Empty, obj.FileName);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_RequiresData()
        {
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, null);
        }

        [TestMethod]
        public void Save_WillSave_NewData()
        {
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, obj);
            var expected = Path.Combine(DataPersistence.DataFolder, obj.FileName);
            File.Exists(expected).ShouldBeTrue();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Save_WillNot_Overwrite()
        {
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, obj);
            var expected = Path.Combine(DataPersistence.DataFolder, obj.FileName);
            File.Exists(expected).ShouldBeTrue();
            DataPersistence.Save(obj.FileName, obj);
        }

        [TestMethod]
        public void Save_FileExists_True()
        {
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, obj);
            var expected = Path.Combine(DataPersistence.DataFolder, obj.FileName);
            File.Exists(expected).ShouldBeTrue();
            DataPersistence.FileExists(obj.FileName).ShouldBeTrue();
        }

        [TestMethod]
        public void Save_FileExists_False()
        {
            var obj = Faker.Generate();
            var expected = Path.Combine(DataPersistence.DataFolder, obj.FileName);
            File.Exists(expected).ShouldBeFalse();
            DataPersistence.FileExists(obj.FileName).ShouldBeFalse();
        }

        [TestMethod]
        public void GetContent_NoFileExists_IsNull()
        {
            var obj = Faker.Generate();
            DataPersistence.GetContent<TmpData>(obj.FileName).ShouldBeNull();
        }

        [TestMethod]
        public void GetContent_FileExists()
        {
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, obj);
            var actual = DataPersistence.GetContent<TmpData>(obj.FileName);
            actual.Id.ShouldBe(obj.Id);
            actual.Name.ShouldBe(obj.Name);
            actual.CreateDate.ShouldBe(obj.CreateDate);
            actual.FileName.ShouldBe(obj.FileName);
        }
    }
}
