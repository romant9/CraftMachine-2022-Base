using System;
using System.Collections;
using TWDModel;
using UnityEngine;

public class GdprLoginStateMachine
{
	public enum States
	{
		Start = 0,
		CheckCookieConsent = 1,
		MarkedForDeletion = 2,
		IDFARequest = 3,
		End = 4
	}

	public enum Events
	{
		PreLogin = 0,
		Login = 1
	}

	private IEnumerator[,] flowStates;

	public States State { get; set; }

	public GdprLoginStateMachine(bool existingUser, States startState = States.Start)
	{
		IEnumerator[,] array = new IEnumerator[5, 2]
		{
			{
				IfNewUserAcceptTOS(),
				NoOp()
			},
			{
				NoOp(),
				CheckCookieConsent()
			},
			{
				NoOp(),
				NoOp()
			},
			{
				NoOp(),
				ShowIDFAConsent()
			},
			{
				NoOp(),
				NoOp()
			}
		};
		IEnumerator[,] array2 = new IEnumerator[5, 2]
		{
			{
				NoOp(),
				TosChangedNotification()
			},
			{
				NoOp(),
				NoOp()
			},
			{
				NoOp(),
				MarkedForDeletion()
			},
			{
				NoOp(),
				ShowIDFAConsent()
			},
			{
				NoOp(),
				NoOp()
			}
		};
		State = startState;
		if (existingUser)
		{
			flowStates = array2;
		}
		else
		{
			flowStates = array;
		}
	}

	public IEnumerator ProcessEvent(Events inEvent)
	{
		try
		{
			return flowStates[(int)State, (int)inEvent];
		}
		catch (Exception)
		{
			return NoOp();
		}
	}

	private IEnumerator IfNewUserAcceptTOS()
	{
		if (string.IsNullOrEmpty(GameManager.UserId))
		{
			SingularityMonoBehaviour<GdprFlowHandler>.Instance.ShowNewUserTOS();
			while (!SingularityMonoBehaviour<GdprFlowHandler>.Instance.AcceptedTOS)
			{
				yield return null;
			}
			State = States.CheckCookieConsent;
		}
	}

	private IEnumerator CheckCookieConsent()
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (gameEconomyData != null && gameEconomyData.ConfigData.GdprAskCookieConsent && modelManager != null && !modelManager.Player.HasTakenGdprAction("CookieConsent"))
		{
			SingularityMonoBehaviour<GdprFlowHandler>.Instance.ShowCookieConsent();
			while (!SingularityMonoBehaviour<GdprFlowHandler>.Instance.CookieConsentSeen)
			{
				yield return null;
			}
		}
		State = States.IDFARequest;
	}

	private IEnumerator TosChangedNotification()
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (gameEconomyData != null && gameEconomyData.ConfigData.GdprTosChanged && modelManager != null && !modelManager.Player.HasTakenGdprAction("TermsOfServiceChanged"))
		{
			SingularityMonoBehaviour<GdprFlowHandler>.Instance.ShowTermsChangedDialog();
			while (!SingularityMonoBehaviour<GdprFlowHandler>.Instance.TOSTermsChangedSeen)
			{
				yield return null;
			}
			State = States.IDFARequest;
		}
		else if (gameEconomyData != null && gameEconomyData.ConfigData.GdprPrivacyPolicyChanged && modelManager != null && !modelManager.Player.HasTakenGdprAction("PrivacyPolicyChanged"))
		{
			SingularityMonoBehaviour<GdprFlowHandler>.Instance.ShowPrivacyPolicyDialog();
			while (!SingularityMonoBehaviour<GdprFlowHandler>.Instance.TosPrivacyPolicySeen)
			{
				yield return null;
			}
			State = States.IDFARequest;
		}
		else
		{
			State = States.IDFARequest;
		}
	}

	private IEnumerator MarkedForDeletion()
	{
		if (GameManager.Instance.gameEconomyData != null)
		{
			SingularityMonoBehaviour<GdprFlowHandler>.Instance.ShowMarkedForDeletion();
			while (!SingularityMonoBehaviour<GdprFlowHandler>.Instance.TOSTermsChangedSeen)
			{
				yield return null;
			}
		}
		State = States.IDFARequest;
	}

	private IEnumerator NoOp()
	{
		return null;
	}

	private IEnumerator ShowIDFAConsent()
	{
		if (Application.platform == RuntimePlatform.Android || !GameManager.Instance.IsIDFACheckEnabled())
		{
			State = States.End;
			yield break;
		}
		if (!SingularityMonoBehaviour<SDKManager>.Instance.SkAdNetworkController.HasAnsweredIDFAPopup())
		{
			SingularityMonoBehaviour<GdprFlowHandler>.Instance.ShowIDFARequestPopup();
			while (!SingularityMonoBehaviour<GdprFlowHandler>.Instance.IDFARequestSeen)
			{
				yield return null;
			}
		}
		else
		{
			SingularityMonoBehaviour<SDKManager>.Instance.SkAdNetworkController.AddConsentResultAction(delegate(int answer)
			{
				PlayerPrefs.SetInt("IDFAPopupAnswer", answer);
				PlayerPrefs.Save();
				Helpers.ExecuteCommand(new SendIDFAStatusCommand(SingularityMonoBehaviour<SDKManager>.Instance.GetATTAnswerByValue(answer)));
			});
			SingularityMonoBehaviour<SDKManager>.Instance.SkAdNetworkController.ShowTrackingConsentDialog();
		}
		State = States.End;
	}
}
