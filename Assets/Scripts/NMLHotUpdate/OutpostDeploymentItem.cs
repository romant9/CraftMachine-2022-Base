using TWDModel;
using UnityEngine;

public class OutpostDeploymentItem : MonoBehaviour
{
	public UISprite BackgroundWalker;

	public UISprite BackgroundSurvivor;

	public UISprite BackgroundObject;

	public UISprite BackgroundRemove;

	public UISprite IconWalkerRegular;

	public UISprite IconWalkerTank;

	public UISprite IconWalkerArmored;

	public UISprite IconSurvivor;

	public UISprite IconContainer;

	public UISprite IconFlag;

	public UILabel AvailableCount;

	public UILabel DeploymentCost;

	[SerializeField]
	private GameObject AvailableParent;

	[SerializeField]
	private GameObject DeploymentParent;

	private bool Active;

	public HotspotState State { get; private set; }

	public WalkerType WalkerType { get; private set; }

	public event DeploymentInteractionHandler OnDeploymentInteraction;

	private void NotifyDeploymentInteraction()
	{
		this.OnDeploymentInteraction?.Invoke(this);
	}

	public void OnDeploymentClicked()
	{
		_ = Active;
	}

	public void Activate()
	{
		if (DeploymentCost != null)
		{
			DeploymentCost.gameObject.SetActive(value: false);
		}
		SetAlphaToAllSprites(1f);
		Active = true;
	}

	public void Deactivate()
	{
		SetAlphaToAllSprites(0.3f);
		Active = false;
	}

	public void SetAvailableCount(int availableCount)
	{
		AvailableCount.text = availableCount.ToString();
	}

	public static OutpostDeploymentItem CreateDeploymentItem(GameObject deploymentItemPrefab, GameObject container, Vector3 offset)
	{
		GameObject gameObject = Object.Instantiate(deploymentItemPrefab);
		OutpostDeploymentItem component = gameObject.GetComponent<OutpostDeploymentItem>();
		if (component != null)
		{
			gameObject.transform.parent = container.transform;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.localPosition = offset;
			return component;
		}
		Object.Destroy(gameObject);
		return component;
	}

	private void DisableAll()
	{
		BackgroundWalker.gameObject.SetActive(value: false);
		BackgroundSurvivor.gameObject.SetActive(value: false);
		BackgroundObject.gameObject.SetActive(value: false);
		BackgroundRemove.gameObject.SetActive(value: false);
		IconWalkerRegular.gameObject.SetActive(value: false);
		IconWalkerTank.gameObject.SetActive(value: false);
		IconWalkerArmored.gameObject.SetActive(value: false);
		IconSurvivor.gameObject.SetActive(value: false);
		IconContainer.gameObject.SetActive(value: false);
		IconFlag.gameObject.SetActive(value: false);
		if (AvailableParent != null)
		{
			AvailableParent.SetActive(value: false);
		}
		if (DeploymentParent != null)
		{
			DeploymentParent.SetActive(value: false);
		}
		State = HotspotState.None;
	}

	public void SetRemove()
	{
		DisableAll();
		BackgroundRemove.gameObject.SetActive(value: true);
		State = HotspotState.None;
	}

	public void SetWalker(WalkerType type, int availableCount, int deploymentCost)
	{
		DisableAll();
		BackgroundWalker.gameObject.SetActive(value: true);
		switch (type)
		{
		case WalkerType.WalkerNormal:
			IconWalkerRegular.gameObject.SetActive(value: true);
			break;
		case WalkerType.WalkerTank:
			IconWalkerTank.gameObject.SetActive(value: true);
			break;
		case WalkerType.WalkerArmored:
			IconWalkerArmored.gameObject.SetActive(value: true);
			break;
		}
		WalkerType = type;
		State = HotspotState.Walker;
		if (AvailableParent != null && DeploymentParent != null)
		{
			AvailableParent.SetActive(value: true);
			DeploymentParent.SetActive(value: true);
		}
		AvailableCount.text = availableCount.ToString();
		DeploymentCost.text = deploymentCost.ToString();
	}

	public void SetObject(bool flag, int deploymentCost)
	{
		DisableAll();
		BackgroundObject.gameObject.SetActive(value: true);
		if (flag)
		{
			IconFlag.gameObject.SetActive(value: true);
			State = HotspotState.Flag;
		}
		else
		{
			IconContainer.gameObject.SetActive(value: true);
			State = HotspotState.ResourceContainer;
		}
		if (DeploymentParent != null)
		{
			DeploymentParent.SetActive(value: true);
		}
		DeploymentCost.text = deploymentCost.ToString();
	}

	public void SetSurvivor(int deploymentCost)
	{
		DisableAll();
		BackgroundSurvivor.gameObject.SetActive(value: true);
		IconSurvivor.gameObject.SetActive(value: true);
		State = HotspotState.DefenderSpawn_0;
		if (DeploymentParent != null)
		{
			DeploymentParent.SetActive(value: true);
		}
		DeploymentCost.text = deploymentCost.ToString();
	}

	public void SetInteraction(bool enabled)
	{
		UIWidget component = GetComponent<UIWidget>();
		if (component != null)
		{
			component.alpha = (enabled ? 1f : 0.5f);
		}
		BoxCollider componentInChildren = GetComponentInChildren<BoxCollider>();
		if (componentInChildren != null)
		{
			componentInChildren.enabled = enabled;
		}
	}

	private void SetAlphaToAllSprites(float alphaValue)
	{
		if (BackgroundWalker != null && BackgroundSurvivor != null && BackgroundObject != null && BackgroundRemove != null && IconWalkerRegular != null && IconWalkerTank != null && IconWalkerArmored != null && IconSurvivor != null && IconContainer != null && IconFlag != null)
		{
			BackgroundWalker.alpha = alphaValue;
			BackgroundSurvivor.alpha = alphaValue;
			BackgroundObject.alpha = alphaValue;
			BackgroundRemove.alpha = alphaValue;
			IconWalkerRegular.alpha = alphaValue;
			IconWalkerTank.alpha = alphaValue;
			IconWalkerArmored.alpha = alphaValue;
			IconSurvivor.alpha = alphaValue;
			IconContainer.alpha = alphaValue;
			IconFlag.alpha = alphaValue;
		}
	}
}
