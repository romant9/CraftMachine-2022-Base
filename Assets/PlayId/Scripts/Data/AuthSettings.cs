using UnityEngine;

namespace Assets.PlayId.Scripts.Data
{
    [CreateAssetMenu(fileName = "AuthSettings", menuName = "Simple Sign-In/Auth Settings/Play ID")]
    public class AuthSettings : ScriptableObject
    {
        public string ClientId;
        public string RedirectUriScheme;
        public bool ManualCancellation;
    }
}