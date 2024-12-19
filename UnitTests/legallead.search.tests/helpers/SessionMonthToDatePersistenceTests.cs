using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.helpers
{
    public class SessionMonthToDatePersistenceTests
    {

        [Fact]
        public void ServiceCanBeContructed()
        {
            var service = new MocPersistence();
            Assert.NotNull(service);
        }

        [Fact]
        public void ServiceCanGetReader()
        {
            var service = new MocReader();
            Assert.NotNull(service.GetPersistence);
        }

        [Fact]
        public void ServiceCanWriteUserRecord()
        {
            var error = Record.Exception(() =>
            {
                var service = new MocPersistence();
                lock (locker)
                {
                    try
                    {
                        service.WriteUserRecord();
                    }
                    finally
                    {
                        service.Initialize();
                    }
                }
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanRead()
        {
            var error = Record.Exception(() =>
            {
                var service = new MocPersistence();
                lock (locker)
                {
                    try
                    {
                        service.WriteUserRecord();
                        var txt = service.Read();
                        var list = txt.ToInstance<List<UsageHistoryModel>>();
                        Assert.NotNull(list);
                        Assert.Equal(5, list.Count);
                    }
                    finally
                    {
                        service.Initialize();
                    }
                }
            });
            Assert.Null(error);
        }


        [Fact]
        public void ServiceCanGetCount()
        {
            var error = Record.Exception(() =>
            {
                var service = new MocPersistence();
                lock (locker)
                {
                    try
                    {
                        service.Initialize();
                        service.WriteUserRecord();
                        var count = service.GetCount("dallas");
                        Assert.Equal(10, count);
                    }
                    finally
                    {
                        service.Initialize();
                    }
                }
            });
            Assert.Null(error);
        }
        private static readonly object locker = new();
        private sealed class MocPersistence : SessionMonthToDatePersistence
        {
            public MocPersistence()
            {
                var http = new Mock<IHttpService>();
                var helper = new Mock<IRemoteDbHelper>();
                var mock = new Mock<SessionUsageReader>(http.Object, helper.Object);
                var list = GetHistory(DateTime.Now, "dallas", 5);
                mock.Setup(m => m.GetUsage(
                    It.IsAny<DateTime>())).Returns(list);
                UsagePersistence = mock.Object;
            }

            private static List<UsageHistoryModel> GetHistory(DateTime d, string county, int count)
            {
                var items = new int[count];
                var list = new List<UsageHistoryModel>();
                foreach (var _ in items)
                {
                    var obj = new UsageHistoryModel
                    {
                        CountyName = county,
                        CreateDate = d,
                        MonthlyUsage = 2
                    };
                    list.Add(obj);
                }
                return list;
            }

        }

        private sealed class MocReader : SessionMonthToDatePersistence
        {
            public MocReader()
            {
                var http = new Mock<IHttpService>();
                var mock = new Mock<SessionUsageReader>(http.Object);
            }

            public SessionUsageReader GetPersistence => UsagePersistence;
        }
    }
}