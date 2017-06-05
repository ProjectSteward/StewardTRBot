using Autofac;

namespace Steward.Service
{
    internal class ServiceProvider : IServiceProvider
    {
        private readonly IContainer container;

        internal ServiceProvider(IContainer container)
        {
            this.container = container;
        }

        T IServiceProvider.Get<T>()
        {
            using (var scope = container.BeginLifetimeScope())
                return scope.Resolve<T>();
        }
    }
}