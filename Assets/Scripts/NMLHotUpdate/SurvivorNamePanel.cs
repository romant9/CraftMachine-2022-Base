using TWDModel;
using UnityEngine;

public class SurvivorNamePanel : MonoBehaviour
{
	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UIInput nameInput;

	[SerializeField]
	private UIButton infoButton;

	[SerializeField]
	private UISprite renameIcon;

	[SerializeField]
	private GameObject[] rarityRankItems;

	private SurvivorModel survivorModel;

	public void EnableNameInput(bool value)
	{
		if (nameInput != null)
		{
			nameInput.enabled = value;
			if (nameInput.GetComponent<BoxCollider>() != null)
			{
				nameInput.GetComponent<BoxCollider>().enabled = value;
				renameIcon.enabled = value;
			}
		}
	}

	public void setInfo(SurvivorModel survivorModel)
	{
		this.survivorModel = survivorModel;
		if (survivorModel != null)
		{
			if (nameLabel != null)
			{
				if (survivorModel.IsHero)
				{
					nameLabel.text = survivorModel.FullName;
				}
				else
				{
					nameLabel.text = survivorModel.Name;
				}
			}
			if (classIcon != null)
			{
				classIcon.spriteName = HelpersGfx.GetSurvivorClassIconName(survivorModel);
			}
			ColorEntry rarityColorData = GameManager.Instance.GetRarityColorData(survivorModel.SurvivorRarityLevel);
			for (int i = 0; i < rarityRankItems.Length; i++)
			{
				if (survivorModel.SurvivorRarityLevel >= i)
				{
					rarityRankItems[i].SetActive(value: true);
					if (rarityRankItems[i].GetComponent<UIWidget>() != null && rarityColorData != null)
					{
						rarityRankItems[i].GetComponent<UIWidget>().color = rarityColorData.GradientColorTop;
					}
					if (rarityRankItems[i].GetComponent<TweenScale>() != null)
					{
						rarityRankItems[i].GetComponent<TweenScale>().ResetToBeginning();
						rarityRankItems[i].GetComponent<TweenScale>().PlayForward();
					}
				}
				else
				{
					rarityRankItems[i].SetActive(value: false);
				}
			}
			if (infoButton != null)
			{
				infoButton.gameObject.SetActive(value: false);
			}
		}
		nameInput.isSelected = false;
		nameInput.enabled = true;
		if (survivorModel.IsHero)
		{
			nameInput.characterLimit = 0;
			nameInput.value = survivorModel.FullName;
		}
		else
		{
			nameInput.characterLimit = 12;
			nameInput.value = survivorModel.Name;
		}
		bool flag = GameManager.Instance.gameEconomyData.ConfigData.CanRenameSurvivors && !TutorialView.Instance.Running;
		nameInput.GetComponent<BoxCollider>().enabled = flag;
		renameIcon.enabled = flag;
	}

	public void OnNameChanged()
	{
		if (survivorModel != null)
		{
			if (RenameCharacterCommand.IsNameValid(GameManager.Instance.playerModel, nameInput.value))
			{
				Helpers.ExecuteCommand(new RenameCharacterCommand(survivorModel, nameInput.value));
				UIEvent.Send("OnSurvivorRenamed");
			}
			setInfo(survivorModel);
		}
	}
}
