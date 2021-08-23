namespace LegalLead.Changed.Commands
{
    public interface IUserCommand
    {
        /// <summary>
        /// Determines the order of execution for command
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Name of this instance
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Executes this command instance
        /// </summary>
        void Execute();

        /// <summary>
        /// Sets Source to backend data file
        /// </summary>
        /// <param name="fileName"></param>
        void SetSource(string fileName);
    }
}
