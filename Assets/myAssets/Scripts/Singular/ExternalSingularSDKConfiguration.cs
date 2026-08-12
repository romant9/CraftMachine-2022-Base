using UnityEngine;

namespace Decagames.Externals.SingularSDK
{
	[CreateAssetMenu(fileName = "ExternalSingularSDKData", menuName = "Externals Configuration/SingularSDK")]
	public class ExternalSingularSDKConfiguration : ScriptableObject
	{
		public string singularAPIKey;

		public string singularAPISecret;
	}
}
