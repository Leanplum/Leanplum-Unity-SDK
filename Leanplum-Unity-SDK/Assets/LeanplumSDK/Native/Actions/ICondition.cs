namespace LeanplumSDK
{
    internal interface ICondition
    {
        string Subject { get; set; }
        string Noun { get; set; }
        string Verb { get; set; }

        bool IsMatch(Trigger trigger);
    }
}