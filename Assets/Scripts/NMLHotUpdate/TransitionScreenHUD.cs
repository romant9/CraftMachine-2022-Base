using System.Collections;
using NextGames.Sdk.AssetBundleManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScreenHUD : HUDElement
{
	private Animator animator;

	[SerializeField]
	private UISprite BackgroundSprite;

	[SerializeField]
	private UILabel loadingLabel;

	[SerializeField]
	[Tooltip("Duration for the transition in effect.")]
	private float transitionInDuration;

	[SerializeField]
	[Tooltip("Duration for the transition out effect.")]
	private float transitionOutDuration;

	private bool animationInRunning;

	private bool animationOutRunning;

	public Callback AnimationInCallback { get; set; }

	public Callback AnimationOutCallback { get; set; }

	public string SceneToLoadAfterInAnimation { get; set; }

	[HideInInspector]
	public string SceneToUnload { get; set; }

	public bool IsAnimating
	{
		get
		{
			if (!animationInRunning)
			{
				return animationOutRunning;
			}
			return true;
		}
	}

	public override void Open()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		base.Open();
		animator = GetComponent<Animator>();
		StopAllCoroutines();
		if (string.IsNullOrEmpty(SceneToUnload))
		{
			StartCoroutine(AnimationIn());
		}
		else
		{
			StartCoroutine(AnimationInUnLoadCombat());
		}
		animationInRunning = true;
		animationOutRunning = false;
	}

	public override void Close()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(AnimationOut());
		}
	}

	private IEnumerator AnimationInUnLoadCombat()
	{
		animator.SetTrigger("AnimationIn");
		animator.ResetTrigger("AnimationOut");
		yield return null;
		AsyncOperation unloadOldCombatOp = SceneManager.UnloadSceneAsync(SceneToUnload);
		while (!unloadOldCombatOp.isDone)
		{
			yield return null;
		}
		SceneToUnload = null;
		StartCoroutine(AnimationIn());
	}

	private IEnumerator AnimationIn()
	{
		bool IsReturnToMod = OfflineManager.IsLoadDataManager && OfflineManager.Instance.IsReturnToResidence;

		animator.SetTrigger("AnimationIn");
		animator.ResetTrigger("AnimationOut");
		yield return new WaitForSeconds(0.25f);
		if (SceneToLoadAfterInAnimation != null)
		{
			GameObject gameObject = ((CampManager.Instance == null) ? null : CampManager.Instance.gameObject);
			if (gameObject != null)
			{
				CampBackground component = gameObject.GetComponent<CampBackground>();
				if (component != null)
				{
					component.RemoveBackground();
				}
				CampView componentInChildren = gameObject.GetComponentInChildren<CampView>(includeInactive: true);
				if (componentInChildren != null)
				{
					Object.Destroy(componentInChildren.gameObject);
				}
			}
			HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.Tutorial);
			if (noCreation != null && noCreation.IsOpen)
			{
				noCreation.Close();
			}
			Helpers.ClearUnusedMemory(gcCollect: true);
			yield return new WaitForEndOfFrame();
			DebugTWD.Log("LoadSceneAsync from TransitionScreenHUD. SceneToLoadAfterInAnimation : " + SceneToLoadAfterInAnimation, DebugType.Load);
			AsyncOperation loadScreenMission = AssetBundleManager.Instance.LoadSceneAsync(SceneToLoadAfterInAnimation, IsReturnToMod ? LoadSceneMode.Single : LoadSceneMode.Additive);
			while (!loadScreenMission.isDone)
			{
				yield return null;
			}
			SceneToLoadAfterInAnimation = null;
		}
		yield return new WaitForSeconds(transitionInDuration);
		if (AnimationInCallback != null)
		{
			AnimationInCallback();
			AnimationInCallback = null;
		}
		animationInRunning = false;

		if (IsReturnToMod)
		{
			CommandHelper.Instance.OpenMissionHub();
			//OfflineManager.Instance.IsReturnToResidence = false;
			Close();
		}
	}

	private IEnumerator AnimationOut()
	{
		yield return null;
		yield return null;
		while (animationInRunning)
		{
			yield return null;
		}
		animator.ResetTrigger("AnimationIn");
		animator.SetTrigger("AnimationOut");
		animationOutRunning = true;
		yield return new WaitForSeconds(transitionOutDuration);
		animationOutRunning = false;
		if (AnimationOutCallback != null)
		{
			AnimationOutCallback();
			AnimationOutCallback = null;
		}
		base.Close();
	}
}
