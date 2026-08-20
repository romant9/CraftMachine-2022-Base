using UnityEngine;

namespace PlayId.Scripts.Data
{
    [CreateAssetMenu(fileName = "AuthSettings", menuName = "Play ID/Auth Settings")]
    public class AuthSettings : ScriptableObject
    {
        public string ClientId;
        public string RedirectUriScheme;
        public bool ManualCancellation;
    }
}