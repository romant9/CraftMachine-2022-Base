using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class TooltipManager
{
	public enum Prefabs
	{
		TooltipTextbox = 0,
		TooltipCombatTextbox = 1,
		TooltipTextboxHud = 2,
		TooltipTextboxActorInfo = 3,
		TooltipChallengeReward = 4,
		TooltipChallengeReward_sp = 5,
		TooltipChallengeReward_sp_cy = 6,
		TooltipComponentSelectorSmall = 7,
		TooltipComponentSelectorLarge = 8,
		TooltipEndlessModeReward = 9,
		TooltipSurvivalReward = 10,
		TooltipCombatSupportSkill = 11,
		TooltipTextboxGold = 12
	}

	private const string LOG_NAME = "TooltipManager: ";

	private static TooltipManager internalInstance;

	private static int TooltipsActive;

	private Dictionary<string, TooltipBase> allTooptipGameObject = new Dictionary<string, TooltipBase>();

	private static TooltipManager Instance => internalInstance ?? (internalInstance = new TooltipManager());

	public static void HideAll(GameObject target = null)
	{
		if (TooltipsActive <= 0)
		{
			return;
		}
		foreach (KeyValuePair<string, TooltipBase> item in Instance.allTooptipGameObject)
		{
			if (!(item.Value == null))
			{
				if (target == null)
				{
					item.Value.Hide();
					TooltipsActive = 0;
				}
				if (target != null && item.Value.GetTarget() != null && target == item.Value.GetTarget())
				{
					item.Value.Hide();
					TooltipsActive--;
				}
			}
		}
	}

	public static void DestroyAllAndClear()
	{
		if (internalInstance == null || internalInstance.allTooptipGameObject == null)
		{
			return;
		}
		foreach (KeyValuePair<string, TooltipBase> item in internalInstance.allTooptipGameObject)
		{
			if (item.Value != null)
			{
				item.Value.Hide();
				Helpers.DestroyOrCache(item.Value);
			}
		}
		internalInstance.allTooptipGameObject.Clear();
		TooltipsActive = 0;
	}

	public static TooltipBase OpenTextBoxWithText(GameObject target, string text, Prefabs prefabEnum = Prefabs.TooltipTextbox)
	{
		if (!string.IsNullOrEmpty(text))
		{
			TooltipBase tooltipBase = Open(target, prefabEnum);
			if (tooltipBase != null)
			{
				tooltipBase.SetText(text);
				tooltipBase.Show();
				NGUITools.SetLayer(tooltipBase.gameObject, target.layer);
				TooltipsActive++;
			}
			return tooltipBase;
		}
		return null;
	}

	public static TooltipBase OpenTextBoxHud(GameObject target, string text, string[] paramNames, string[] paramValues, Prefabs prefabEnum = Prefabs.TooltipTextboxHud)
	{
		TooltipBase tooltipBase = Open(target, prefabEnum);
		if (tooltipBase != null && tooltipBase.GetComponent<TooltipTextboxHud>() != null)
		{
			tooltipBase.SetText(text);
			tooltipBase.GetComponent<TooltipTextboxHud>().SetParamAndValuesTexts(paramNames, paramValues);
			tooltipBase.Show();
			TooltipsActive++;
		}
		return tooltipBase;
	}

	public static TooltipBase OpenTextBoxActorInfo(GameObject target, ActorModel actor, Prefabs prefabEnum = Prefabs.TooltipTextboxActorInfo)
	{
		TooltipBase tooltipBase = Open(target, prefabEnum);
		if (tooltipBase != null && tooltipBase.GetComponent<TooltipTextboxActorInfo>() != null)
		{
			tooltipBase.GetComponent<TooltipTextboxActorInfo>().SetParamAndValuesTexts(actor);
			tooltipBase.Show();
			TooltipsActive++;
		}
		return tooltipBase;
	}

	public static TooltipBase OpenForChallengeReward(GameObject target, WeeklyChallengeReward reward, int starValue, Prefabs prefabEnum = Prefabs.TooltipChallengeReward)
	{
		TooltipBase tooltipBase = Open(target, prefabEnum);
		if (tooltipBase != null && tooltipBase.GetComponent<TooltipChallengeReward>() != null)
		{
			tooltipBase.GetComponent<TooltipChallengeReward>().UpdateWithParams(reward, starValue);
			tooltipBase.Show();
			TooltipsActive++;
		}
		return tooltipBase;
	}

	public static TooltipBase OpenForChallengeReward_sp(GameObject target, WeeklyChallengeReward reward, int starValue, int OverSpeedConvertedAmount, Prefabs prefabEnum = Prefabs.TooltipChallengeReward_sp)
	{
		TooltipBase tooltipBase = Open(target, prefabEnum);
		if (tooltipBase != null && tooltipBase.GetComponent<TooltipChallengeReward>() != null)
		{
			tooltipBase.GetComponent<TooltipChallengeReward>().UpdateWithParams(reward, starValue, OverSpeedConvertedAmount);
			tooltipBase.Show();
			TooltipsActive++;
		}
		return tooltipBase;
	}

	public static TooltipBase OpenForChallengeReward_sp_cy(GameObject target, IReward reward, int OverSpeedConvertedAmount, Prefabs prefabEnum = Prefabs.TooltipChallengeReward_sp_cy)
	{
		TooltipBase tooltipBase = Open(target, prefabEnum);
		if (tooltipBase != null && tooltipBase.GetComponent<TooltipChallengeReward>() != null)
		{
			tooltipBase.GetComponent<TooltipChallengeReward>().UpdateCommon(reward, OverSpeedConvertedAmount);
			tooltipBase.Show();
			TooltipsActive++;
		}
		return tooltipBase;
	}

	public static TooltipBase OpenForEndlessModeReward(GameObject target, string text, int rank, Prefabs prefabEnum = Prefabs.TooltipEndlessModeReward, SurvivorClass survivorClass = SurvivorClass.None)
	{
		TooltipBase tooltipBase = Open(target, prefabEnum);
		if (tooltipBase != null && tooltipBase.GetComponent<TooltipEndlessModeReward>() != null)
		{
			tooltipBase.GetComponent<TooltipEndlessModeReward>().UpdateWithParams(text, rank, survivorClass);
			tooltipBase.Show();
			TooltipsActive++;
		}
		return tooltipBase;
	}

	public static TooltipBase OpenForCombatSupportSurvivor(GameObject target, Prefabs prefabEnum = Prefabs.TooltipCombatSupportSkill)
	{
		TooltipBase tooltipBase = Open(target, prefabEnum);
		if (tooltipBase != null && tooltipBase is TooltipSupportSkill tooltipSupportSkill)
		{
			tooltipSupportSkill.SetSurvivor();
			tooltipSupportSkill.SetShadowedGuard();
			tooltipBase.Show();
			TooltipsActive++;
		}
		return tooltipBase;
	}

	public static TooltipBase OpenForCombatSupport(GameObject target, SupportModel supportModel, Prefabs prefabEnum = Prefabs.TooltipCombatSupportSkill)
	{
		TooltipBase tooltipBase = Open(target, prefabEnum);
		if (tooltipBase != null && tooltipBase is TooltipSupportSkill tooltipSupportSkill)
		{
			tooltipSupportSkill.Set(supportModel);
			tooltipBase.Show();
			TooltipsActive++;
		}
		return tooltipBase;
	}

	public static TooltipBase OpenForComponentSlot(GameObject target, int index, Prefabs prefabEnum = Prefabs.TooltipComponentSelectorSmall, List<CurrencyType> excludeCurrencies = null)
	{
		TooltipBase tooltipBase = Open(target, prefabEnum);
		if (tooltipBase != null && tooltipBase.GetComponent<TooltipComponentSelector>() != null && index > -1)
		{
			tooltipBase.GetComponent<TooltipComponentSelector>().UpdateWithParams(index, excludeCurrencies);
			tooltipBase.Show();
			TooltipsActive++;
		}
		return tooltipBase;
	}

	private static TooltipBase Open(GameObject target, Prefabs prefabEnum)
	{
		string prefabName = prefabEnum.ToString();
		if (target != null)
		{
			if (Instance.getTooltipInstance(prefabName, out var tooltip))
			{
				tooltip.SetTarget(target);
				tooltip.Overlay();
				return tooltip;
			}
		}
		else
		{
			LogWarning("Could not open tooltip for NULL target!");
		}
		return null;
	}

	private bool getTooltipInstance(string prefabName, out TooltipBase tooltip)
	{
		if (!allTooptipGameObject.TryGetValue(prefabName, out tooltip) || tooltip == null)
		{
			GameObject gameObject = UnityUtils.LoadFromAssetBundle(prefabName, HUDElementConfig.BundleName) as GameObject;
			if (!(gameObject != null))
			{
				LogError("Could not find Prefab with name: " + prefabName);
				return false;
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, SingularityMonoBehaviour<HUDManager>.Instance.UIContainerTopCameras.transform);
			if (!(gameObject2 != null))
			{
				LogError("Could not Instantiate GameObject. Prefab name: " + prefabName);
				return false;
			}
			tooltip = gameObject2.GetComponent<TooltipBase>();
			if (!(tooltip != null))
			{
				LogError("GameObject does not have a any Tooltip component. Prefab name:" + prefabName);
				return false;
			}
			//if (!allTooptipGameObject.ContainsKey(prefabName))
			//{
			//	allTooptipGameObject.Add(prefabName, tooltip);
			//}
			allTooptipGameObject[prefabName] = tooltip;
			tooltip.transform.localScale = Vector3.one;
			tooltip.transform.localPosition = Vector3.zero;
		}
		return tooltip != null;
	}

	private static void LogWarning(string message)
	{
		Debug.LogWarning("TooltipManager: " + message);
	}

	private static void LogError(string message)
	{
		Debug.LogError("TooltipManager: " + message);
	}

	#region mycode
	public static TooltipBase OpenTextBoxWithText(GameObject target, string text, GameObject prefab)
	{
		if (!string.IsNullOrEmpty(text))
		{
			if (prefab == null)
			{
				GameObject prefabRef = Resources.Load<GameObject>("hudelements/TooltipTextbox");
				if (!prefabRef) return null;
				prefab = GameObject.Instantiate(prefabRef);
			}
			if (prefab.TryGetComponent<TooltipBase>(out var tooltipBase))
			{
				if (prefab.layer != target.layer)
				{
					NGUITools.SetLayer(prefab, target.layer);
				}
				tooltipBase.SetTarget(target);
				tooltipBase.Overlay();

				tooltipBase.SetText(text);
				tooltipBase.Show();
				TooltipsActive++;

				return tooltipBase;
			}
		}
		return null;
	}

	public static TooltipBase OpenTextBoxHud(GameObject target, string text, GameObject prefab, string[] paramNames, string[] paramValues)
	{
		if (prefab.TryGetComponent<TooltipBase>(out var tooltipBase))
		{
			if (tooltipBase.TryGetComponent<TooltipTextboxHud>(out var tooltipTextboxHud))
			{
				tooltipBase.SetTarget(target);
				tooltipBase.Overlay();

				tooltipBase.SetText(text);
				tooltipTextboxHud.SetParamAndValuesTexts(paramNames, paramValues);
				tooltipBase.Show();
				TooltipsActive++;

				return tooltipBase;
			}
		}
		return null;
	}

	public static TooltipBase OpenForComponentSlot(GameObject target, int index, GameObject prefab, List<CurrencyType> excludeCurrencies = null)
	{
		if (prefab.TryGetComponent<TooltipBase>(out var tooltipBase))
		{
			if (tooltipBase.TryGetComponent<TooltipComponentSelector>(out var tooltipComponentSelector) && index > -1)
			{
				tooltipBase.SetTarget(target);
				tooltipBase.Overlay();
				tooltipComponentSelector.UpdateWithParams(index, excludeCurrencies);
				tooltipBase.Show();
				TooltipsActive++;

				return tooltipBase;
			}
		}
		return null;
	}

	public static TooltipBase OpenForTraitSlot(GameObject target, GameObject prefab, UpgradeTraitsData data, EquipmentItemModel model, int layer)
	{
		if (prefab.TryGetComponent<TooltipBase>(out var tooltipBase))
		{
			if (tooltipBase.TryGetComponent<TooltipTraitSelector>(out var tooltipTraitSelector))
			{
				tooltipBase.SetTarget(target);
				tooltipBase.Overlay();
				tooltipTraitSelector.SetLayer(layer);
				tooltipTraitSelector.UpdateWithParams(data, model);
				tooltipBase.Show();
				TooltipsActive++;

				return tooltipBase;
			}
		}
		return null;
	}

	public static TooltipBase OpenForTokenSlot(GameObject target, GameObject prefab)
	{
		if (prefab.TryGetComponent<TooltipBase>(out var tooltipBase))
		{
			if (tooltipBase.TryGetComponent<TooltipTokenSelector>(out var tooltipTokenSelector))
			{
				tooltipBase.SetTarget(target);
				tooltipBase.Overlay();
				tooltipTokenSelector.UpdateWithParams();
				tooltipBase.Show();
				TooltipsActive++;

				return tooltipBase;
			}
		}
		return null;
	}
	#endregion
}
