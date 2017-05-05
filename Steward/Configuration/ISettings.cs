namespace Steward.Configuration
{
    internal interface ISettings
    {
        string this[string name] { get; }
    }
}
