using System.Collections.Generic;
using System.Reflection;
using BaseModel;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class CampaignDeeplinkItem : MonoBehaviour
{
	[SerializeField]
	private UILabel deeplinkNameLabel;

	[SerializeField]
	private GameObject buttonParent;

	[SerializeField]
	private GameObject timerParent;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UILabel timerHeader;

	private CurrencyRewardSolver solver = new CurrencyRewardSolver();

	private AvailableRewardsCollector collector;

	private TWDModelObject twdModelObject;

	private string deeplinkName;

	private string deeplinkURL;

	private float timerRefreshInterval;

	private bool timerBased;

	private Dictionary<TWDModelObject, MethodInfo> modelTimerMethods = new Dictionary<TWDModelObject, MethodInfo>();

	public void SetParameters(AvailableRewardsCollector collectorIn, TWDModelObject objectToInspect, string deeplinkName, string deeplinkURL, string refreshLocKey, bool timerBased = false)
	{
		collector = collectorIn;
		twdModelObject = objectToInspect;
		this.deeplinkName = deeplinkName;
		this.deeplinkURL = deeplinkURL;
		this.timerBased = timerBased;
		if (!string.IsNullOrEmpty(refreshLocKey))
		{
			HelpersUI.SetContentToLabel(timerHeader, LocalizationManager.GetText(refreshLocKey));
		}
		if (twdModelObject != null && timerBased)
		{
			twdModelObject.Changed -= OnModelChanged;
			twdModelObject.Changed += OnModelChanged;
		}
	}

	public void UpdateUI()
	{
		Helpers.GameObjectSetActive(timerParent, value: false);
		if (timerBased)
		{
			if (solver.HasRewardsOfType(CurrencyType.CampaignToken, collector.GetRewards()))
			{
				Helpers.GameObjectSetActive(buttonParent, value: true);
			}
			else
			{
				UpdateTimeOnTimer();
				Helpers.GameObjectSetActive(timerParent, value: true);
				Helpers.GameObjectSetActive(buttonParent, value: false);
			}
		}
		else
		{
			bool value = solver.HasRewardsOfType(CurrencyType.CampaignToken, collector.GetRewards());
			Helpers.GameObjectSetActive(buttonParent, value);
		}
		HelpersUI.SetContentToLabel(deeplinkNameLabel, LocalizationManager.GetText(deeplinkName));
	}

	private long GetNextAvailableTimer(TWDModelObject modelObject)
	{
		MethodInfo value = null;
		if (!modelTimerMethods.TryGetValue(modelObject, out value))
		{
			MethodInfo[] methods = modelObject.GetType().GetMethods();
			for (int i = 0; i < ((methods != null) ? methods.Length : 0); i++)
			{
				MethodInfo methodInfo = methods[i];
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ModelAvailableTimerAttribute), inherit: false);
				if (customAttributes != null && customAttributes.Length == 1)
				{
					modelTimerMethods[modelObject] = methodInfo;
					value = methodInfo;
					break;
				}
			}
		}
		long result = 0L;
		if (value != null)
		{
			result = (long)value.Invoke(modelObject, null);
		}
		return result;
	}

	private void Update()
	{
		if (timerBased)
		{
			timerRefreshInterval -= Time.deltaTime;
			if (timerRefreshInterval < 0f)
			{
				UpdateTimeOnTimer();
				timerRefreshInterval = 1f;
			}
		}
	}

	private void UpdateTimeOnTimer()
	{
		long nextAvailableTimer = GetNextAvailableTimer(twdModelObject);
		if (nextAvailableTimer > 0)
		{
			HelpersUI.SetContentToLabel(timerLabel, Helpers.FormatTime(nextAvailableTimer));
		}
	}

	public void OnClickGo()
	{
		string text = deeplinkURL;
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (!DeepLinkNavigation.HandleDeepLink(text))
		{
			Debug.LogError($"Invalid deep link {text} in campaign deeplink {deeplinkName}.");
			return;
		}
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.ActivityPopup);
		if (noCreation != null)
		{
			noCreation.Close();
		}
	}

	private void OnModelChanged(ModelObject modelObject, string changed, object args)
	{
		if (modelObject != null)
		{
			UpdateUI();
		}
	}
}
