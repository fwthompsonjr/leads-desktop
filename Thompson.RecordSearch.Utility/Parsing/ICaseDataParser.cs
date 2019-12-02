using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Parsing
{
    public interface ICaseDataParser
    {
        /// <summary>
        /// Gets the search keyword used to split data.
        /// </summary>
        /// <value>
        /// The search for.
        /// </value>
        string SearchFor { get; }

        /// <summary>
        /// Gets or sets the case data.
        /// </summary>
        /// <value>
        /// The case data.
        /// </value>
        string CaseData { get; set; }

        /// <summary>
        /// Determines whether this instance can parse.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can parse; otherwise, <c>false</c>.
        /// </returns>
        bool CanParse();

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns></returns>
        ParseCaseDataResponseDto Parse();
    }
}
