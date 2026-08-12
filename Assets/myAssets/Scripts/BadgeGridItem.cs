using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

namespace TwdCustomMod
{
    public class BadgeGridItem : MonoBehaviour
    {
        public ResidenceCraftBadgeTab badgeTab;

        public List<RecipeComponentView> badgeComponents;

        public SurvivorBadgesIcon badgeModel { get; set; }

        public int badgeIndex { get; set; }

        public List<CurrencyType> Currencies => GetCurrencies();

        public List<string> LogList { get; set; }

        public BadgeLog badgeLog { get; set; }

        public BaseModel.ModelRandom modelRandom { get; set; }

        public bool IsRecipe { get; set; }

        public UILabel RecipeLable;
        public UILabel IndexLable;
        public UILabel randomRateLable;

        public int MyState;
        public int MyCallCount;
        public int MyInitialSeed;

        public ShowTooltip toolTip;

        void Start()
        {
        }

        //OnClick
        public void SetCraftComponents() 
        {
            badgeTab.SetCraftComponents(GetCurrencies());
            bool IsSelected = GetComponent<UIButtonToggle>().IsToggled;
            BadgeCraft.Instance.SelectedBadge = IsSelected ? this : null;
            
            string text = !IsSelected ? "Residence.CraftButton.Text" : "Button.Edit";
            
            badgeTab.SetContentToCraftButton(LocalizationManager.GetText(text));
        }

        //OnCraft
        public void SetBadgeComponents(List<CurrencyType> currencies)
        {
            for (int i = 0; i < badgeComponents.Count; i++)
            {
                badgeComponents[i].Initialize(currencies[i], 1);
            }
        }

        public List<CurrencyType> GetCurrencies()
        {
            return badgeComponents.Select(x => x.SelectedCurrency).ToList();
        }

        [ContextMenu("Execute")]
        public void ShowRandom()
        {
            if (modelRandom != null)
            {
                MyState = modelRandom.State;
                MyCallCount = modelRandom.CallCount;
                MyInitialSeed = modelRandom.InitialSeed;
            }
        }
    }
}

