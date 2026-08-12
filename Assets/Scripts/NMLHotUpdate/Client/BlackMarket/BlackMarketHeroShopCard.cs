using System.Collections;
using System.Linq;
using TWDModel;
using UnityEngine;

namespace Client.BlackMarket
{
	public class BlackMarketHeroShopCard : NUIListItem<BlackMarketHeroSlot>
	{
		[SerializeField]
		private UILabel timeLeft;

		[SerializeField]
		private UITexture heroImage;

		[SerializeField]
		private UIButton onOpen;

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private GameObject newDealsContainer;

		private BlackMarketHeroSlot data;

		private readonly WaitForSeconds oneSecondWait = new WaitForSeconds(1f);

		private Coroutine timeLeftCoroutine;

		private EventDelegate onClickEventDelegate;

		private Collider collider;

		private void Awake()
		{
			collider = GetComponent<Collider>();
			onClickEventDelegate = new EventDelegate(OnOpenEventHandler);
		}

		private void OnEnable()
		{
			onOpen.onClick.Add(onClickEventDelegate);
			collider.enabled = true;
		}

		private void OnDisable()
		{
			if (timeLeftCoroutine != null)
			{
				StopCoroutine(timeLeftCoroutine);
			}
			timeLeftCoroutine = null;
			onOpen.onClick.Remove(onClickEventDelegate);
			collider.enabled = true;
		}

		public override void SetData(BlackMarketHeroSlot data)
		{
			collider.enabled = true;
			base.SetData(data);
			this.data = data;
			UpdateUI();
		}

		public override void UpdateUI()
		{
			string heroSeasonIDArt = GameManager.Instance.gameEconomyData.BlackMarketHeroDefinitions.First((BlackMarketHeroDefinition x) => x.ActorDefinitionID == data.ActiveActorDefinitionID).HeroSeasonIDArt;
			HelpersGfx.SetSeasonHeroMaterial(heroImage, heroSeasonIDArt);
			string text = GameManager.Instance.gameEconomyData.GetActorDefinition(data.ActiveActorDefinitionID).Name;
			title.text = LocalizationManager.GetText("Popup.Shop.BlackMarket.ShopTitle{Name}", text);
			if (timeLeftCoroutine == null)
			{
				timeLeftCoroutine = StartCoroutine(UpdateTimeLeft());
			}
			bool flag = BlackMarketShopController.Instance.HasSeenOfferFor(data.ActiveActorDefinitionID);
			newDealsContainer.SetActive(!flag);
			GetComponent<Collider>().enabled = false;
		}

		private IEnumerator UpdateTimeLeft()
		{
			while (true)
			{
				long num = data.NextUpdate - GameManager.Instance.playerModel.UtcTimeStamp;
				if (num < 0 && GameManager.Instance.playerModel.BlackMarket.NeedToUpdate())
				{
					((ShopPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ShopPopup)).UpdateSelectedTab();
				}
				string text = Helpers.FormatTime(num);
				timeLeft.text = text;
				yield return oneSecondWait;
			}
		}

		private void OnOpenEventHandler()
		{
			BlackMarketShopController.Instance.ShowFor(data);
		}
	}
}
