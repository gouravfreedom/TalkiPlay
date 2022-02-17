using System;
using TalkiPlay;
using Xamarin.Forms;

[assembly: ResolutionGroupName(EffectHelper.EffectResolutionGroupName)]

namespace TalkiPlay
{
    public static class EffectHelper
    {
        public const string EffectResolutionGroupName = Config.AppName;

        public static Effect ResolveLocal<T>()
        {
            return Effect.Resolve(GetLocalName<T>());
        }

        public static string GetLocalName<T>()
        {
            var name = typeof(T).Name;
            return $"{EffectResolutionGroupName}.{name}";
        }
    }
}
