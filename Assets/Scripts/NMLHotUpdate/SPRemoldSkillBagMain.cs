using GooglePlayGames.BasicApi;
using System.Collections;
using TWDModel;
using UnityEngine;

public class SPRemoldSkillBagMain : MonoBehaviour
{
	[SerializeField]
	private SPRemoldSkillBagLeft left;

	[SerializeField]
	private SPRemoldSkillBagMid mid;

	[SerializeField]
	private SPRemoldSkillBagRight right;

	private SurvivorClass currentFilterClass = SurvivorClass.None;

	private void OnEnable()
	{
		StartCoroutine(SelectFirstClassDataCoroutine());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator SelectFirstClassDataCoroutine()
	{
		yield return new WaitForEndOfFrame();
		currentFilterClass = SurvivorClass.Shooter;
		UIEvent.Send("SPRemoldChangeSurvivorClassFilter", currentFilterClass);
	}

	private void Start()
	{
		if (!Helpers.IsSkillBagOpened())
		{
			Helpers.SetSkillBagOpened(on: true);
			HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillOpenTipsPopup);
			if (hUDElement != null)
			{
				hUDElement.Open();
			}
		}
	}



	#region mycode
	public void OnClickUnlockAll(UIToggle tg)
	{
		ModSkillManager modSkillManager = GameManager.Instance.playerModel.ModSkillManager;

		if (tg.value)
		{
			modSkillManager.ModSkillModesBackup ??= new BaseModel.ModelList<ModSkillMode>();
			var list2 = modSkillManager.GetUnlockableModSkills(mid.filterSurvivorClass);
			foreach (var skill in list2)
			{
				//SPTraitsRemoldDefinitions def = GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinition(skill.ID);
				right.MakeModSkillCommandResult(skill.ID, skill.Type, out ModSkillMode skillMod);
				if (skillMod != null) modSkillManager.ModSkillModesBackup.Add(skillMod);
			}
		}
		else
		{
			if (modSkillManager.ModSkillModesBackup != null && modSkillManager.ModSkillModesBackup.Count > 0)
			{
				foreach (var skillMod in modSkillManager.ModSkillModesBackup.Models)
				{
					modSkillManager.ModSkillModes.Remove(skillMod);
				}
			}
		}
		mid.UpdateUI();
	}
	#endregion
}
