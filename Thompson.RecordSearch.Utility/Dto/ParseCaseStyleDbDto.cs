namespace Thompson.RecordSearch.Utility.Dto
{
    public class ParseCaseStyleDbDto
    {
        /// <summary>
        /// Gets or sets the original input data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data { get; set; }
        /// <summary>
        /// Gets or sets the case style as translated from data parameter.
        /// </summary>
        /// <value>
        /// The case data.
        /// </value>
        public string CaseData { get; set; }

        /// <summary>
        /// Gets or sets the defendant.
        /// </summary>
        /// <value>
        /// The defendant.
        /// </value>
        public string Defendant { get; set; }

        /// <summary>
        /// Gets or sets the plantiff.
        /// </summary>
        /// <value>
        /// The plantiff.
        /// </value>
        public string Plantiff { get; set; }
    }
}
