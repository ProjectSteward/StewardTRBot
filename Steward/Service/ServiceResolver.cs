using Autofac;

namespace Steward.Service
{
    internal static class ServiceResolver
    {
        internal static IServiceProvider ServiceProvider;

        internal static T Get<T>()
        {
            return ServiceProvider.Get<T>();
        }
    }
}