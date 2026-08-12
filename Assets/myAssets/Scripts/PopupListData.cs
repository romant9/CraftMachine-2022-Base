using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupListData : MonoBehaviour
{
    //public bool IsSingleTooltip;

    public List<GameObject> toolTips;

    private GameObject currentTooltip;

    public UILabel recipeDescription;

    public UILabel recipeLabel;
    public UISprite recipeSprite;
    public string recipeText;

    public int currentIndex;

    public enum RecipeType
    {
        Base,
        Step,
        Jump
    }

    public RecipeType recipeType = RecipeType.Base;

    private void Start()
    {
        currentTooltip = toolTips[0];
    }

    public void OnClickPopup(UIButton button)
    {
        if (button != null && button.gameObject != null)
        {
            currentTooltip.SetActive(true);
            //TooltipManager.OpenForComponentSlot(button.gameObject, -1, toolTip);
            UpdateUI();
        }
    }

    public void OnClickItem(UIButton button)
    {
        if (button != null && button.gameObject != null)
        {
            recipeText = button.transform.GetChild(0).GetComponent<UILabel>().text;
            recipeLabel.text = recipeText;
            var sprite = button.transform.GetChild(1).GetComponent<UISprite>();
            recipeSprite.atlas = sprite.atlas;
            HelpersUI.SetSprite(recipeSprite, sprite.spriteName);

            var toggleList = currentTooltip.GetComponentInChildren<UIButtonToggleSet>();
            currentIndex = toggleList.GetSelectedIndexByObject(button);
        }
    }


    public void ChangeRecipeType(UIPopupList list)
    {
        int index = list.items.IndexOf(list.value);
        index = index >= 0 ? index : 0;

        recipeType = (RecipeType)index;

        currentTooltip = toolTips[index];

        recipeDescription.text = list.value;

        var toggleList = currentTooltip.GetComponentInChildren<UIButtonToggleSet>();
        int tgIndex = toggleList.GetSelectedIndex();
        currentIndex = tgIndex >= 0 ? tgIndex : 0;
        var toggle = toggleList.GetButton(currentIndex);
        OnClickItem(toggle);
    }


    private void UpdateUI()
    {
        DebugTWD.Log("Update");
    }
}
