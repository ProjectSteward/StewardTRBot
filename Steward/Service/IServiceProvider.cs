namespace Steward.Service
{
    internal interface IServiceProvider
    {
        T Get<T>();
    }
}
