namespace LegalLead.PublicData.Search.Command
{
    public abstract class CommandBase
    {
        public abstract string Name { get; }

        public abstract void Execute(FormMain mainForm);
    }
}
