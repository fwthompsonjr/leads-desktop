namespace LegalLead.Changed.Classes
{
    public interface IBuildCommand
    {
        /// <summary>
        /// Determines the order of execution for command
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the Source file for this command
        /// </summary>
        string SourceFile { get; }

        /// <summary>
        /// Attempts to load source file
        /// </summary>
        /// <param name="sourceFileName"></param>
        void SetSource(string sourceFileName);

        /// <summary>
        /// Executes associated command action
        /// </summary>
        bool Execute();

        /// <summary>
        /// Gets the log file associated to application changes
        /// </summary>
        Models.ChangeLog Log { get; }

        string Name { get; }
    }
}
