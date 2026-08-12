using System.Collections;
using TwdCustomMod;
using UnityEngine;

public class ButtonResetData : MonoBehaviour
{
    public bool waitForClick;
    public UISprite blockSprite;

    void Start()
    {       
    }

    private IEnumerator ResetC()
    {
        if (waitForClick)
        {
            DebugTWD.Log("ResetAll");
            DataManager.Instance.ClearPlayerPrefs();
            yield break;
        }
        float startTime = Time.realtimeSinceStartup;

        waitForClick = true;
        blockSprite.gameObject.SetActive(false);

        while (waitForClick)
        {
            if (Time.realtimeSinceStartup - startTime > 1f)
            {
                DebugTWD.LogWarning("timeout");
                waitForClick = false;
                blockSprite.gameObject.SetActive(true);
                yield break;
            }
            yield return null;
        }
    }
    public void OnClickReset()
    {
        StopAllCoroutines();
        StartCoroutine(ResetC());
    }
}
