using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class TraitsContainer : MonoBehaviour
{
	[SerializeField]
	private GameObject traitPillPrefab;

	[SerializeField]
	private float spaceBetweenPillsX;

	[SerializeField]
	private float spaceBetweenPillsY;

	private List<TraitPill> pills = new List<TraitPill>();

	public void SetSurvivorTraits(SurvivorModel survivorModel)
	{
		HideTraits();
		List<TraitEntry> traits = survivorModel.GetTraits();
		int count = traits.Count;
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		for (int i = 0; i < count; i++)
		{
			string traitIdentifier = traits[i].TraitIdentifier;
			TraitDefinition traitDefinition = gameEconomyData.GetTraitDefinition(traitIdentifier);
			if (traitDefinition != null)
			{
				SetPill(i, HelpersLocalization.GetTraitName(traitDefinition), HelpersLocalization.GetTraitDescription(traitDefinition));
			}
			else
			{
				Debug.LogError("SetSurvivorTraits failed: could not find trait definition for trait [" + traits[i].TraitIdentifier + "]");
			}
		}
		Reposition();
	}

	public void SetEquipmentTraits(EquipmentItemModel equipmentItemModel)
	{
		HideTraits();
		Reposition();
	}

	private void Reposition()
	{
		Vector3 zero = Vector3.zero;
		float num = GetComponent<UIWidget>().width;
		foreach (TraitPill pill in pills)
		{
			Vector3 size = pill.GetComponent<BoxCollider>().size;
			if (zero.x + size.x >= num)
			{
				zero.x = 0f;
				zero.y -= size.y + spaceBetweenPillsY;
			}
			pill.transform.localPosition = zero;
			zero.x += size.x + spaceBetweenPillsX;
		}
	}

	private void HideTraits()
	{
		foreach (TraitPill pill in pills)
		{
			pill.gameObject.SetActive(value: false);
		}
	}

	private void SetPill(int index, string name, string description, int rarityLevel = 0, int nameMaximumCharacters = -1)
	{
		if (index >= pills.Count)
		{
			GameObject gameObject = Helpers.InstantiateToParentAndLayer(traitPillPrefab, base.gameObject);
			NGUITools.SetLayer(gameObject, base.gameObject.layer);
			pills.Add(gameObject.GetComponent<TraitPill>());
		}
		TraitPill traitPill = pills[index];
		if (nameMaximumCharacters == -1)
		{
			traitPill.Name = name;
		}
		else
		{
			traitPill.Name = name.Substring(0, Math.Min(name.Length, nameMaximumCharacters));
		}
		traitPill.Description = description;
		traitPill.RarityLevel = rarityLevel;
		traitPill.UpdateUI();
		traitPill.gameObject.SetActive(value: true);
	}
}
