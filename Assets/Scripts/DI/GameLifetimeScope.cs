using Scripts;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.Register<Test1>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
        }
    }
}