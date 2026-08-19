using PlayId.Scripts.Data;
using PlayId.Scripts.Services;
using UnityEngine;

namespace PlayId.Scripts
{
    public class PlayIdServices
    {
        public readonly Auth Auth;
        public readonly RemoteConfig RemoteConfig;

        public static PlayIdServices Instance => _ ??= new PlayIdServices();

        private static PlayIdServices _;

        public PlayIdServices()
        {
            var settings = Resources.Load<AuthSettings>("AuthSettings");

            Auth = new Auth(settings);
            RemoteConfig = new RemoteConfig(settings.ClientId);
        }
    }
}