#nullable enable
namespace Scripts.Extensions
{
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public static class DIExtensions
    {
        private static LifetimeScope? CurrentSceneContext;

        public static IObjectResolver GetCurrentContainer(this object _)
        {
            if (CurrentSceneContext == null) CurrentSceneContext = Object.FindFirstObjectByType<LifetimeScope>();
            return CurrentSceneContext!.Container;
        }
    }
}