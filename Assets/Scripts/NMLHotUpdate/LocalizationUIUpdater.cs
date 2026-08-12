using System.Collections;
using UnityEngine;

public class LocalizationUIUpdater : MonoBehaviour, ILocalizationListener
{
	[Tooltip("Must match the keys in the MO file.")]
	public string LocalizationKey;

	[Tooltip("If enabled, localized text will be split into paragraphs if a triple-space ('   ') is found within the string.")]
	public bool AutomaticNewlines;

	protected UILabel label;

	private Coroutine updateLabelCoroutine;

	public virtual string localizationKey => LocalizationKey;

	protected virtual void Awake()
	{
		label = GetComponent<UILabel>();
		if (gameObject.name == "Label_nextGames")
		{
			Debug.Log("Awake Label_nextGames");
		}
	}

	private void OnEnable()
	{
		updateLabelCoroutine ??= StartCoroutine(UpdateLabel());
	}

	private IEnumerator UpdateLabel(params object[] parameters)
	{
		while (SingularityMonoBehaviour<LocalizationManager>.Instance == null)
		{
			yield return new WaitForEndOfFrame();
		}
		if (label == null)
		{
			label = GetComponent<UILabel>();
		}
		if (label == null)
		{
			Debug.LogError("LocalizationUIUpdated has no Text component attached: " + base.gameObject.name);
			if (gameObject.name == "Label_nextGames")
			{
				Debug.Log("Awake Label_nextGames");
			}
			yield break;
		}
		string text = GetLocalizedText(parameters);
		if (AutomaticNewlines)
		{
			text = HelpersLocalization.ReplaceTripleSpaceWithNewline(text);
		}
		label.text = text;
		updateLabelCoroutine = null;
	}

	protected virtual string GetLocalizedText(params object[] parameters)
	{
		if (!IsCustomTranslate)
		{
			return LocalizationManager.GetText(localizationKey, parameters);
		}
		else
		{
			var currentText = LocalizationManager.Instance.CurrentLanguage == "ru" ? RuCustomText : EnCustomText;

			if (string.IsNullOrEmpty(EnCustomText))
			{
				currentText = RuCustomText;
			}
			if (string.IsNullOrEmpty(RuCustomText))
			{
				currentText = EnCustomText;
			}

			return currentText;

			//if (!string.IsNullOrEmpty(EnCustomText))
			//{
			//	return LocalizationManager.Instance.CurrentLanguage == "ru" ? !string.IsNullOrEmpty(RuCustomText) ? RuCustomText : EnCustomText : EnCustomText;
			//}
		}
	}

	[ContextMenu("UpdateContent")]
	public virtual void OnLanguageChanged()
	{
		UpdateContent();
	}

	public void UpdateContent(params object[] parameters)
	{
		if (updateLabelCoroutine == null)
		{
			this.gameObject.SetActive(true);
			updateLabelCoroutine = StartCoroutine(UpdateLabel(parameters));
		}
	}



	#region myparams
	public string customLocalizationKey;
	public bool IsCustomTranslate;
	public string EnCustomText;
	public string RuCustomText;
	#endregion

	#region mycode
	public void UpdateCustomContent(string customKey, params object[] parameters)
	{
		IsCustomTranslate = true;
		customLocalizationKey = customKey;
		EnCustomText = null;
		UpdateContent(parameters);
	}
	#endregion
}
