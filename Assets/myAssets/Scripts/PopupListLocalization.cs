using System.Collections;
using System.Collections.Generic;
using TwdCustomMod;
using UnityEngine;

[RequireComponent(typeof(UIPopupList))]
public class PopupListLocalization : MonoBehaviour, ILocalizationListener
{
    public List<string> LocalizationKeys;

    public bool AutomaticNewlines;

    public bool IsCustomTranslate;

    public List<string> EnCustomText;
    public List<string> RuCustomText;

    private Coroutine updateLabelCoroutine;

    public virtual List<string> localizationKeys => LocalizationKeys;

    UIPopupList mList;

    void Awake() { mList = GetComponent<UIPopupList>(); }

    private void OnEnable()
    {
        if (updateLabelCoroutine == null)
        {
            updateLabelCoroutine = StartCoroutine(UpdateLabel());
        }
    }

    private IEnumerator UpdateLabel()
    {
        while (SingularityMonoBehaviour<LocalizationManager>.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i< mList.items.Count; i++)
        {
            string text;

            if (IsCustomTranslate)
            {
                text = DataManager.Instance.language == DataManager.Language.Ru ? RuCustomText[i] : EnCustomText[i];
            }
            else
            {
                text = GetLocalizedText(i);
                if (AutomaticNewlines)
                {
                    text = HelpersLocalization.ReplaceTripleSpaceWithNewline(text);
                }
            }
            mList.items[i] = text;
        }
        updateLabelCoroutine = null;

    }

    protected virtual string GetLocalizedText(int index)
    {
        return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(localizationKeys[index]);
    }

    public virtual void OnLanguageChanged()
    {
        UpdateContent();
    }

    public void UpdateContent()
    {
        if (updateLabelCoroutine == null)
        {
            updateLabelCoroutine = StartCoroutine(UpdateLabel());
        }
    }


    public void Refresh()
    {
        if (mList != null && Localization.knownLanguages != null)
        {
            mList.Clear();

            for (int i = 0, imax = Localization.knownLanguages.Length; i < imax; ++i)
                mList.items.Add(Localization.knownLanguages[i]);

            mList.value = Localization.language;
        }
    }
}
