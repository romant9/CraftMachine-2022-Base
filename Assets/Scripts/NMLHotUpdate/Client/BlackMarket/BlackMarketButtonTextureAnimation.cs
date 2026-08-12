using System.Collections;
using System.Linq;
using TWDModel;
using UnityEngine;

namespace Client.BlackMarket
{
	public class BlackMarketButtonTextureAnimation : MonoBehaviour
	{
		[SerializeField]
		private UITexture texture1;

		[SerializeField]
		private UITexture texture2;

		[SerializeField]
		private float timeATextureIsVisible;

		private int currentHeroIndex;

		private int currentTextureVisible;

		private WaitForSeconds wait;

		private TweenerPlayer texture1Player;

		private TweenerPlayer texture2Player;

		private const int fadeInAnimationGroup = 10;

		private const int fadeOutAnimationGroup = 11;

		private void Awake()
		{
			texture1Player = texture1.GetComponent<TweenerPlayer>();
			texture2Player = texture2.GetComponent<TweenerPlayer>();
		}

		private void OnEnable()
		{
			wait = new WaitForSeconds(timeATextureIsVisible);
			StartCoroutine(Animate());
		}

		private IEnumerator Animate()
		{
			while (!GameManager.Instance.playerModel.BlackMarket.ContentInitialized)
			{
				yield return new WaitForEndOfFrame();
			}
			Shader shader = Shader.Find("Drill/NGUI-Unlit/Transparent Colored Dual (SoftClip)");
			texture1Player.PlayGroup(11, instant: true);
			texture2Player.PlayGroup(11, instant: true);
			while (true)
			{
				string actorId = GameManager.Instance.playerModel.BlackMarket.Slots[currentHeroIndex].ActiveActorDefinitionID;
				string heroSeasonIDArt = GameManager.Instance.gameEconomyData.BlackMarketHeroDefinitions.First((BlackMarketHeroDefinition x) => x.ActorDefinitionID == actorId).HeroSeasonIDArt;
				UITexture obj = ((currentHeroIndex == 0) ? texture1 : texture2);
				HelpersGfx.SetSeasonHeroMaterial(obj, heroSeasonIDArt);
				Material material = new Material(obj.material)
				{
					shader = shader
				};
				obj.material = material;
				((currentHeroIndex == 0) ? texture1Player : texture2Player).PlayGroup(10, instant: false);
				yield return wait;
				((currentHeroIndex == 0) ? texture1Player : texture2Player).PlayGroup(11, instant: false);
				currentHeroIndex++;
				currentHeroIndex %= GameManager.Instance.playerModel.BlackMarket.Slots.Length;
			}
		}
	}
}
