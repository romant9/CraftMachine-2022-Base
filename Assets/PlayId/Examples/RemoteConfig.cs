using Assets.PlayId.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PlayId.Examples
{
    public class RemoteConfig : MonoBehaviour
    {
        public Text Output;

        public void Load()
        {
            PlayIdServices.Instance.RemoteConfig.Load(OnLoadRemoteConfig);

            void OnLoadRemoteConfig(bool success, string error, string remoteConfig)
            {
                Output.text = success ? remoteConfig : error;
            }
        }
    }
}