namespace LegalLead.PublicData.Search.Enumerations
{
    public enum ExecutionResponseType
    {
        None = 0,
        ValidationFail = 10,
        IndexOfOutBounds = 20,
        ExecutionFailed = 30,
        Success = 100,
        UnexpectedError = 1000
    }
}
