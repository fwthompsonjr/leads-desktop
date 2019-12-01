using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Parsing
{
    public class ParseCaseDataByVsStrategy : ParseCaseDataByVersusStrategy
    {
        private static readonly string _searchKeyWord = @"vs";

        public override string SearchFor => _searchKeyWord;

    }
}
