namespace Thompson.RecordSearch.Utility.Parsing
{
    public class ParseCaseDataByVsStrategy : ParseCaseDataByVersusStrategy
    {
        private const string _searchKeyWord = @"vs";

        public override string SearchFor => _searchKeyWord;

    }
}
