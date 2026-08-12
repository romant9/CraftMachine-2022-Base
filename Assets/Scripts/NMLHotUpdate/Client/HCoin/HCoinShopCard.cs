using TWDModel;
using UnityEngine;

namespace Client.HCoin
{
	public class HCoinShopCard : NUIListItem<HillTopStoreSlot>
	{
		[SerializeField]
		private UITexture heroImage;

		[SerializeField]
		private UIButton onOpen;

		[SerializeField]
		private UILabel title;

		private HillTopStoreSlot data;

		private readonly WaitForSeconds oneSecondWait = new WaitForSeconds(1f);

		private EventDelegate onClickEventDelegate;

		private void Awake()
		{
			onClickEventDelegate = new EventDelegate(OnOpenEventHandler);
		}

		private void OnEnable()
		{
			onOpen.onClick.Add(onClickEventDelegate);
		}

		private void OnDisable()
		{
			onOpen.onClick.Remove(onClickEventDelegate);
		}

		public override void SetData(HillTopStoreSlot data)
		{
			base.SetData(data);
			this.data = data;
			UpdateUI();
		}

		public override void UpdateUI()
		{
			HillTopStoreSlotDefinition hillTopStoreSlotDefinition = GameManager.Instance.gameEconomyData.GetHillTopStoreSlotDefinition(data.SlotType);
			string textId = "None";
			string text = "None";
			if (hillTopStoreSlotDefinition != null)
			{
				textId = hillTopStoreSlotDefinition.CoverLocalizationKey;
				text = hillTopStoreSlotDefinition.CoverImagePath;
			}
			title.text = LocalizationManager.GetText(textId);
			heroImage.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle(text, "itemgraphics");
		}

		private void OnOpenEventHandler()
		{
			HCoinShopController.Instance.ShowFor(data);
		}
	}
}
