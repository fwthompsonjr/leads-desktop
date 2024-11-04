using Harris.Criminal.Db;
using Harris.Criminal.Db.Downloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Harris.Criminal.UnitTests.Downloads
{
    [TestClass]
    public class HarrisCriminalBoLookupTests
    {
        private readonly HarrisCriminalDto Dto = new HarrisCriminalDto();
        private readonly Dictionary<string, string> Statuses = new Dictionary<string, string>
        {
            {"A", "HAS OPEN SETTINGS" },
            {"B", "INACTIVE DUE TO BOND FORFEITURE" },
            {"C", "FINAL DISPOSITION, NO ACTIVITY EXPECTED" },
            {"D", "DISMISSED" },
            {"E", "NGRI COMMITTED" },
            {"F", "INACTIVE-PENDING FELONY DISPOSITION EXPECTED" },
            {"G", "PENDING GRAND JURY" },
            {"I", "HAS NO OPEN SETTINGS" },
            {"J", "INACTIVE - DEF. SENT TO TDCJ ON OTHER CASE" },
            {"K", "PTS OOC SUPERVISION" },
            {"L", "TRIAL DOCKET" },
            {"M", "PENDING REFILE" },
            {"N", "INACTIVE - BOND FORFEITURE ON OTHER CASE" },
            {"O", "OTHER CASE ON APPEAL" },
            {"P", "PENDING APPEAL" },
            {"Q", "PENDING COMPLETION COMMUNITY SERVICE" },
            {"R", "PENDING COMPLETION OF PROBATION" },
            {"S", "PENDING RESTORATION OF SANITY" },
            {"T", "TEMPORARY" },
            {"V", "EXECUTION RETURNED" },
            {"W", "WAITING EXECUTION" },
            {"X", "PENDING DUE COURSE" },
            {"Z", "INACTIVE-BOND FORFEITURE" }
        };
        private readonly Random random = new Random(Environment.TickCount);

        [TestInitialize]
        public void Setup()
        {
            // on start load references
            Startup.References.ReadReferences();

        }

        [TestMethod]
        public void CanMap_Status_A()
        {
            var status = "A";
            List<HarrisCriminalDto> list = GetDtoList(status);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
        }

        [TestMethod]
        public void CanMap_Status_B()
        {
            var status = "B";
            List<HarrisCriminalDto> list = GetDtoList(status);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
        }

        [TestMethod]
        public void CanMap_Status_C()
        {
            var status = "C";
            List<HarrisCriminalDto> list = GetDtoList(status);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
        }

        [TestMethod]
        public void CanMap_Status_D()
        {
            var status = "D";
            List<HarrisCriminalDto> list = GetDtoList(status);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
        }

        [TestMethod]
        public void CanMap_Status_E()
        {
            var status = "E";
            List<HarrisCriminalDto> list = GetDtoList(status);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
        }

        [TestMethod]
        public void CanMap_Status_F()
        {
            var status = "F";
            List<HarrisCriminalDto> list = GetDtoList(status);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
        }

        [TestMethod]
        public void CanMap_Status_G()
        {
            var status = "G";
            List<HarrisCriminalDto> list = GetDtoList(status);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
        }

        [TestMethod]
        public void CanMap_Status_Random()
        {
            var keys = Statuses.Keys.ToList();
            var index = random.Next(0, keys.Count - 1);
            var status = keys[index];
            List<HarrisCriminalDto> list = GetDtoList(status);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
        }

        [TestMethod]
        public void CanMap_Status_Random_ReadMultiple()
        {
            var keys = Statuses.Keys.ToList();
            var index = random.Next(0, keys.Count - 1);
            var status = keys[index];
            List<HarrisCriminalDto> list = GetDtoList(status);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
            bo.LiteralCaseStatus.ShouldBe(Statuses[status]);
        }

        [TestMethod]
        public void CanMap_Status_NullValue()
        {
            List<HarrisCriminalDto> list = GetDtoList(null);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(string.Empty);
        }

        [TestMethod]
        public void CanMap_Status_EmptyValue()
        {
            List<HarrisCriminalDto> list = GetDtoList(string.Empty);
            var bo = HarrisCriminalBo.Map(list).First();
            bo.LiteralCaseStatus.ShouldBe(string.Empty);
        }

        private List<HarrisCriminalDto> GetDtoList(string status)
        {
            Dto.FilingDate = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            Dto.CaseStatus = status;
            var list = new List<HarrisCriminalDto> { Dto };
            return list;
        }
    }
}
