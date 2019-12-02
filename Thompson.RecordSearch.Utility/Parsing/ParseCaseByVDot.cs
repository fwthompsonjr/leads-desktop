namespace Thompson.RecordSearch.Utility.Parsing
{
    public class ParseCaseByVDot : ParseCaseDataByVersusStrategy
    {

        private static readonly string _searchKeyWord = @" v. ";

        public override string SearchFor => _searchKeyWord;
    }
}
