using UnityEngine;

namespace PlayId.Scripts.Data
{
	[CreateAssetMenu(fileName = "CustomAuthSettings", menuName = "Play ID/Custom Auth Settings")]
	public class CustomAuthSettings : ScriptableObject
	{
		public string ClientId;
		public string RedirectUriScheme;
		public string[] RedirectUri;
		public string AuthorizationEndpoint;
		public string TokenEndpoint;
		public bool UseTimeout;
		public int TimeoutSeconds = 60;
	}
}
