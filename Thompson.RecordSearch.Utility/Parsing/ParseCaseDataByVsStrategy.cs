namespace Thompson.RecordSearch.Utility.Parsing
{
    public class ParseCaseDataByVsStrategy : ParseCaseDataByVersusStrategy
    {
        private static readonly string _searchKeyWord = @"vs";

        public override string SearchFor => _searchKeyWord;

    }
}
