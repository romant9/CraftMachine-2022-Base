using UnityEngine;

namespace PlayId.Scripts.Data
{
	[CreateAssetMenu(fileName = "YandexSettings", menuName = "Play ID/Yandex Settings")]
	public class YandexSettings : ScriptableObject
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
