using System;
using System.Collections.Generic;
using Client.Constants;
using UnityEngine;

public class ModularCharacterCombiner : MonoBehaviour
{
	[Serializable]
	public class Replacements
	{
		public Shader FromShader;

		public Shader ToShader;
	}

	public SkinnedMeshRenderer OriginalRenderer;

	public bool UseCombatOutlineShaders = true;

	public List<Replacements> PortraitShaderReplacements;

	public List<Replacements> CombatShaderReplacements;

	public List<Replacements> GuildBattlePVPLoadingShaderReplacements;

	public GameObject HeadAttachmentParent;

	private Queue<UnityEngine.Object> usedTexturesOutfit = new Queue<UnityEngine.Object>();

	private Queue<UnityEngine.Object> usedTextures = new Queue<UnityEngine.Object>();

	private static readonly string headAttachmentParentName = "Rootbone/Bind_Hips/Bind_Spine/Bind_Spine1/Bind_Spine2/Bind_Neck/Bind_Head/Bind_HeadTop_End";

	private static readonly string headBoneName = "Rootbone/Bind_Hips/Bind_Spine/Bind_Spine1/Bind_Spine2/Bind_Neck/Bind_Head";

	public Queue<UnityEngine.Object> UsedTextures => usedTextures;

	public Queue<UnityEngine.Object> UsedTexturesOutfit => usedTexturesOutfit;

	private void AttachMesh(GameObject meshPrefab, Color skinColor, Color detailColor, CharacterBuildType type = CharacterBuildType.CombatLoading)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(meshPrefab);
		gameObject.name = meshPrefab.name;
		foreach (SkinnedMeshRenderer item in new List<SkinnedMeshRenderer>(gameObject.GetComponentsInChildren<SkinnedMeshRenderer>()))
		{
			Material[] sharedMaterials = item.sharedMaterials;
			Material[] array = new Material[sharedMaterials.Length];
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				Material material = (array[i] = UnityEngine.Object.Instantiate(sharedMaterials[i]));
				GetUsedTexturesFromMaterial(material);
				string text = material.shader.name;
				switch (type)
				{
				case CharacterBuildType.Combat:
				{
					for (int k = 0; k < CombatShaderReplacements.Count; k++)
					{
						if (CombatShaderReplacements[k].FromShader != null && CombatShaderReplacements[k].ToShader != null && text.Equals(CombatShaderReplacements[k].FromShader.name, StringComparison.Ordinal))
						{
							material.shader = CombatShaderReplacements[k].ToShader;
						}
					}
					break;
				}
				case CharacterBuildType.Portrait:
				{
					for (int l = 0; l < PortraitShaderReplacements.Count; l++)
					{
						if (PortraitShaderReplacements[l].FromShader != null && PortraitShaderReplacements[l].ToShader != null && text.Equals(PortraitShaderReplacements[l].FromShader.name, StringComparison.Ordinal))
						{
							material.shader = PortraitShaderReplacements[l].ToShader;
						}
					}
					Mesh sharedMesh = item.sharedMesh;
					int num = 0;
					if (sharedMesh != null)
					{
						num = item.sharedMesh.blendShapeCount;
					}
					for (int m = 0; m < num; m++)
					{
						if (sharedMesh.GetBlendShapeName(m).Equals("Portrait"))
						{
							item.SetBlendShapeWeight(m, 100f);
						}
					}
					break;
				}
				case CharacterBuildType.GuildBattleLoading:
				{
					for (int j = 0; j < GuildBattlePVPLoadingShaderReplacements.Count; j++)
					{
						if (GuildBattlePVPLoadingShaderReplacements[j].FromShader != null && GuildBattlePVPLoadingShaderReplacements[j].ToShader != null && text.Equals(GuildBattlePVPLoadingShaderReplacements[j].FromShader.name, StringComparison.Ordinal))
						{
							material.shader = GuildBattlePVPLoadingShaderReplacements[j].ToShader;
						}
					}
					break;
				}
				}
				if (i == 0)
				{
					material.color = skinColor;
				}
				material.SetColor(MaterialParameters.DetailColor, detailColor);
			}
			item.materials = array;
			item.transform.parent = OriginalRenderer.transform.parent;
			Transform[] bones = item.bones;
			Transform[] bones2 = OriginalRenderer.bones;
			Transform[] array2 = new Transform[bones.Length];
			for (int n = 0; n < bones.Length; n++)
			{
				for (int num2 = 0; num2 < bones2.Length; num2++)
				{
					if (bones[n].name == bones2[num2].name)
					{
						array2[n] = bones2[num2];
						break;
					}
					_ = bones2.Length - 1;
				}
			}
			item.bones = array2;
			item.rootBone = OriginalRenderer.rootBone;
		}
		UnityEngine.Object.DestroyImmediate(gameObject);
	}

	private void AddHeadAttachment(HeadAttachment attachment, GameObject headPart, CharacterBuildType type = CharacterBuildType.CombatLoading)
	{
		GameObject obj = Helpers.InstantiateToParent(attachment.GetPrefab(), OriginalRenderer.gameObject);
		Transform transform = headPart.transform.Find(headAttachmentParentName);
		if (transform != null)
		{
			HeadAttachmentParent.transform.localPosition = transform.localPosition;
			HeadAttachmentParent.transform.localRotation = transform.localRotation;
			HeadAttachmentParent.transform.localScale = transform.localScale;
		}
		obj.transform.parent = HeadAttachmentParent.transform;
		MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			Material material;
			if (!string.IsNullOrEmpty(attachment.ReplacementMaterialName))
			{
				material = UnityEngine.Object.Instantiate(attachment.GetReplacementMaterial());
			}
			else
			{
				material = UnityEngine.Object.Instantiate(meshRenderer.sharedMaterial);
				if (material != null)
				{
					material.color = attachment.Color;
				}
			}
			string text = material.shader.name;
			switch (type)
			{
			case CharacterBuildType.Combat:
			{
				for (int k = 0; k < CombatShaderReplacements.Count; k++)
				{
					if (CombatShaderReplacements[k].FromShader != null && CombatShaderReplacements[k].ToShader != null && text.Equals(CombatShaderReplacements[k].FromShader.name, StringComparison.Ordinal))
					{
						material.shader = CombatShaderReplacements[k].ToShader;
					}
				}
				break;
			}
			case CharacterBuildType.Portrait:
			{
				for (int l = 0; l < PortraitShaderReplacements.Count; l++)
				{
					if (PortraitShaderReplacements[l].FromShader != null && PortraitShaderReplacements[l].ToShader != null && text.Equals(PortraitShaderReplacements[l].FromShader.name, StringComparison.Ordinal))
					{
						material.shader = PortraitShaderReplacements[l].ToShader;
					}
				}
				break;
			}
			case CharacterBuildType.GuildBattleLoading:
			{
				for (int j = 0; j < GuildBattlePVPLoadingShaderReplacements.Count; j++)
				{
					if (GuildBattlePVPLoadingShaderReplacements[j].FromShader != null && GuildBattlePVPLoadingShaderReplacements[j].ToShader != null && text.Equals(GuildBattlePVPLoadingShaderReplacements[j].FromShader.name, StringComparison.Ordinal))
					{
						material.shader = GuildBattlePVPLoadingShaderReplacements[j].ToShader;
					}
				}
				break;
			}
			}
			meshRenderer.material = material;
		}
	}

	private void Start()
	{
	}

	public void GenerateCharacter(ModularCharacter characterBase, ModularCharacter characterOverride, CharacterBuildType type = CharacterBuildType.CombatLoading)
	{
		ModularCharacter modularCharacter = characterBase;
		if (characterOverride != null)
		{
			modularCharacter = ComposeCharacter(characterBase, characterOverride);
		}
		else if (characterBase.OutfitOverride != null)
		{
			modularCharacter = ComposeCharacter(characterBase, characterBase.OutfitOverride);
		}
		if (!(modularCharacter != null))
		{
			return;
		}
		if (!string.IsNullOrEmpty(modularCharacter.PortraitHeadPartName) && !string.IsNullOrEmpty(modularCharacter.HeadPartName) && !string.IsNullOrEmpty(modularCharacter.TorsoPartName) && !string.IsNullOrEmpty(modularCharacter.LegsPartName))
		{
			switch (type)
			{
			case CharacterBuildType.Combat:
				AttachMesh(modularCharacter.GetHeadPart(), Color.white, modularCharacter.TorsoColor, type);
				AttachMesh(modularCharacter.GetTorsoPart(), modularCharacter.SkinColor, modularCharacter.TorsoColor, type);
				AttachMesh(modularCharacter.GetLegsPart(), modularCharacter.SkinColor, modularCharacter.LegsColor, type);
				break;
			case CharacterBuildType.Portrait:
				AttachMesh(modularCharacter.GetPortraitHeadPart(), Color.white, modularCharacter.TorsoColor, type);
				AttachMesh(modularCharacter.GetTorsoPart(), modularCharacter.SkinColor, modularCharacter.TorsoColor, type);
				break;
			default:
				AttachMesh(modularCharacter.GetHeadPart(), Color.white, modularCharacter.TorsoColor, type);
				AttachMesh(modularCharacter.GetTorsoPart(), modularCharacter.SkinColor, modularCharacter.TorsoColor, type);
				AttachMesh(modularCharacter.GetLegsPart(), modularCharacter.SkinColor, modularCharacter.LegsColor, type);
				break;
			}
		}
		foreach (HeadAttachment headAttachment in modularCharacter.HeadAttachments)
		{
			if (!string.IsNullOrEmpty(headAttachment.PrefabName))
			{
				AddHeadAttachment(headAttachment, modularCharacter.GetHeadPart(), type);
			}
		}
		Transform transform = OriginalRenderer.gameObject.transform.parent.parent.Find(headBoneName);
		if (type == CharacterBuildType.Portrait)
		{
			transform.localEulerAngles += modularCharacter.HeadBoneRotation;
			base.gameObject.GetComponent<ShadowBlobOrient>().enabled = false;
		}
		if (type == CharacterBuildType.Camp)
		{
			UnityUtils.StripPhysicsFromHierarchy(base.gameObject);
		}
		if (type == CharacterBuildType.CombatLoading)
		{
			SwitchOnAllShadows();
		}
	}

	private ModularCharacter ComposeCharacter(ModularCharacter characterBase, ModularCharacter characterOverride)
	{
		ModularCharacter modularCharacter = ScriptableObject.CreateInstance<ModularCharacter>();
		modularCharacter.Gender = characterBase.Gender;
		modularCharacter.Weight = characterBase.Weight;
		modularCharacter.name = characterBase.name;
		modularCharacter.HeadPartName = characterBase.HeadPartName;
		modularCharacter.SkinColor = characterBase.SkinColor;
		modularCharacter.SkinColorPreset = characterBase.SkinColorPreset;
		modularCharacter.TorsoColorPreset = characterBase.TorsoColorPreset;
		modularCharacter.LegsColorPreset = characterBase.LegsColorPreset;
		modularCharacter.PortraitHeadPartName = characterBase.PortraitHeadPartName;
		modularCharacter.PortraitSetup = ((characterOverride.PortraitSetup == ActorProperties.PortraitSetupType.Random) ? characterBase.PortraitSetup : characterOverride.PortraitSetup);
		modularCharacter.HeadBoneRotation = ((characterOverride.HeadBoneRotation == Vector3.zero) ? characterBase.HeadBoneRotation : characterOverride.HeadBoneRotation);
		modularCharacter.LegsPartName = (string.IsNullOrEmpty(characterOverride.LegsPartName) ? characterBase.LegsPartName : characterOverride.LegsPartName);
		modularCharacter.TorsoPartName = (string.IsNullOrEmpty(characterOverride.TorsoPartName) ? characterBase.TorsoPartName : characterOverride.TorsoPartName);
		modularCharacter.TorsoColor = characterOverride.TorsoColor;
		modularCharacter.LegsColor = characterOverride.LegsColor;
		modularCharacter.HeadAttachments = new List<HeadAttachment>();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (characterOverride.HeadAttachments.Count > 0)
		{
			foreach (HeadAttachment headAttachment in characterOverride.HeadAttachments)
			{
				if ((!string.IsNullOrEmpty(headAttachment.PrefabName) && headAttachment.PrefabName.Contains("Hair")) || headAttachment.PrefabName.Contains("Hat"))
				{
					flag = true;
				}
				if (!string.IsNullOrEmpty(headAttachment.PrefabName) && headAttachment.PrefabName.Contains("Eyes"))
				{
					flag2 = true;
				}
				if (!string.IsNullOrEmpty(headAttachment.PrefabName) && headAttachment.PrefabName.Contains("Face"))
				{
					flag3 = true;
				}
			}
			modularCharacter.HeadAttachments.AddRange(characterOverride.HeadAttachments);
			foreach (HeadAttachment headAttachment2 in characterBase.HeadAttachments)
			{
				if (!string.IsNullOrEmpty(headAttachment2.PrefabName) && (((headAttachment2.PrefabName.Contains("Hair") || headAttachment2.PrefabName.Contains("Hat")) && !flag) || (headAttachment2.PrefabName.Contains("Eyes") && !flag2) || (headAttachment2.PrefabName.Contains("Face") && !flag3)))
				{
					modularCharacter.HeadAttachments.Add(headAttachment2);
				}
			}
		}
		else
		{
			modularCharacter.HeadAttachments.AddRange(characterBase.HeadAttachments);
		}
		return modularCharacter;
	}

	public string GetName(GameObject part)
	{
		return part.name.Replace("Parts_", "").Replace("_", " ");
	}

	public void SwitchOnAllShadows()
	{
		ShadowBlobOrient[] componentsInChildren = base.gameObject.GetComponentsInChildren<ShadowBlobOrient>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
			componentsInChildren[i].CreateShadow();
		}
	}

	private void GetUsedTexturesFromMaterial(Material material)
	{
		UnityUtils.CollectTexture(material, MaterialParameters.MainTex, ref usedTextures, ref usedTexturesOutfit);
		UnityUtils.CollectTexture(material, MaterialParameters.BumpMap, ref usedTextures, ref usedTexturesOutfit);
		UnityUtils.CollectTexture(material, MaterialParameters.MaskTex, ref usedTextures, ref usedTexturesOutfit);
		UnityUtils.CollectTexture(material, MaterialParameters.AlphaTex, ref usedTextures, ref usedTexturesOutfit);
	}
}
