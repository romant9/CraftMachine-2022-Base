using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using UnityEngine;

namespace Supabase.TWD
{
	public class SessionListener : MonoBehaviour
	{
		public SupabaseManager SupabaseManager = null;
		public string LoggedInEmailAddress;

		public void UnityAuthListener(IGotrueClient<User, Session> sender, Constants.AuthState newState)
		{
			if (sender.CurrentUser?.Email == null)
			{
				LoggedInEmailAddress = "No user logged in";
			}
			else
			{
				LoggedInEmailAddress = $"Logged in as {sender.CurrentUser.Email}";
				DebugTWD.Log(LoggedInEmailAddress, DebugType.Supabase);
			}

			switch (newState)
			{
				case Constants.AuthState.SignedIn:
					Debug.Log("Signed In");
					break;
				case Constants.AuthState.SignedOut:
					Debug.Log("Signed Out");
					break;
				case Constants.AuthState.UserUpdated:
					Debug.Log("Signed In");
					break;
				case Constants.AuthState.PasswordRecovery:
					Debug.Log("Password Recovery");
					break;
				case Constants.AuthState.TokenRefreshed:
					Debug.Log("Token Refreshed");
					break;
				case Constants.AuthState.Shutdown:
					Debug.Log("Shutdown");
					break;
				default:
					Debug.Log("Unknown Auth State Update");
					break;
			}
		}
	}
}
