using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseModel;
using TWDModel;
using UnityEngine;

namespace TwdCustomMod
{    
    public class CraftTools
    {
        public GameEconomyData GameData { get; set; }

        public int GetBadgeRarity(BadgeRarityResult badgeRarityResult, FixedPoint roll, out int maxRarity)
        {
            List<KeyValuePair<FixedPoint, int>> list = CreateBadgeRarityProbabilities(badgeRarityResult);
            maxRarity = 0;
            int result = 0;
            bool flag = false;
            foreach (KeyValuePair<FixedPoint, int> item in list)
            {
                if (item.Key > 0L)
                {
                    maxRarity = item.Value;
                }

                if (item.Key >= roll && !flag)
                {
                    result = item.Value;
                    flag = true;
                }
            }

            return result;
        }

        public void CreateBonusCondition(BadgeBonusDefinition bonusDef, ModelRandom random, ref BadgeModel badgeModel)
        {
            Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(BaseBonusCondition), bonusDef.ConditionClassName);
            if (!string.IsNullOrEmpty(bonusDef.ConditionClassName) && type == null)
            {
                DebugTWD.Log("Failed to instantiate condition class " + bonusDef.ConditionClassName);
            }

            List<string> list = new List<string> { bonusDef.ConstructionParameters[0] };
            if (bonusDef.ConstructionParameters.Count > 1)
            {
                list.Add(random.GetRandomElement(bonusDef.ConstructionParameters.GetRange(1, bonusDef.ConstructionParameters.Count - 1), remove: false));
            }

            badgeModel.BonusCondition = ((type != null) ? (ReflectionUtils.Instantiate(type, list) as BaseBonusCondition) : null);
            badgeModel.BonusParameters = list;
        }

        public List<KeyValuePair<FixedPoint, int>> CreateBadgeRarityProbabilities(BadgeRarityResult result)
        {
            FixedPoint fixedPoint = 0L;
            List<KeyValuePair<FixedPoint, int>> list = new List<KeyValuePair<FixedPoint, int>>();
            if (result.Common > 0L)
            {
                list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Common, 0));
                fixedPoint += result.Common;
            }

            if (result.Uncommon > 0L)
            {
                list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Uncommon, 1));
                fixedPoint += result.Uncommon;
            }

            if (result.Rare > 0L)
            {
                list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Rare, 2));
                fixedPoint += result.Rare;
            }

            if (result.Epic > 0L)
            {
                list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Epic, 3));
                fixedPoint += result.Epic;
            }

            if (result.Legendary > 0L)
            {
                list.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + result.Legendary, 4));
                fixedPoint += result.Legendary;
            }

            return list;
        }

        public string GetEffect(List<CurrencyType> components, ModelRandom random, out int effectState)
        {
            effectState = 0;
            string text = string.Empty;
            BadgeRecipe badgeRecipe = GameData.BadgeRecipes.FirstOrDefault((BadgeRecipe recipe) => recipe.CanBeBuiltWith(components));
            if (badgeRecipe != null)
            {
                int chanceToCraftRecipe = GetChanceToCraftRecipe(components);
                if (random.Next(100) < chanceToCraftRecipe)
                {
                    effectState = random.State;
                    text = GetRandomEffect(random, badgeRecipe);
                }
            }

            if (string.IsNullOrEmpty(text))
            {
                List<BadgeRecipe> list = GameData.BadgeRecipes.Where((BadgeRecipe recipe) => recipe != badgeRecipe).ToList();
                int index = random.Next(list.Count);
                text = GetRandomEffect(random, list[index]);
            }

            return text;
        }

        public string GetRandomEffect(ModelRandomMod random, BadgeRecipe badgeRecipe)
        {
            List<string> list = badgeRecipe.Results.Split(',').ToList();
            return random.GetRandomElement(list, remove: false);
        }

        public string GetRandomEffect(BaseModel.ModelRandom random, BadgeRecipe recipe)
        {
            List<string> list = recipe.Results.Split(',').ToList();
            return random.GetRandomElement(list, remove: false);
        }

        public int GetChanceToCraftRecipe(List<CurrencyType> components)
        {
            int num = 0;
            foreach (CurrencyType component in components)
            {
                string currencyTypeString = component.ToString();
                BadgeEffectChances badgeEffectChances = GameData.BadgeEffectChances.FirstOrDefault((BadgeEffectChances badgeEffectChance) => badgeEffectChance.ComponentId == currencyTypeString);
                if (badgeEffectChances != null)
                {
                    num += badgeEffectChances.Chance;
                }
            }

            return num;
        }

        public List<string> CreateBadgeGatchaDeckOfIds<T>(string typeIndex, T[] listToBeUsed) where T : TypeIndexDefinition
        {
            List<string> list = new List<string>();
            for (int i = 0; i < ((listToBeUsed != null) ? listToBeUsed.Length : 0); i++)
            {
                list.Add(listToBeUsed[i].ID);
                list.Add(listToBeUsed[i].ID);
                if (!string.IsNullOrEmpty(typeIndex) && listToBeUsed[i].TypeIndex == typeIndex)
                {
                    list.Add(listToBeUsed[i].ID);
                }
            }

            return list;
        }

        public void CraftComplete(TWDModelResult result, ResidenceCraftBadgeTab tab)
        {
            GameManager manager = GameManager.Instance;

            if (result == TWDModelResult.OK)
            {
                //manager.CheckConnectionReachability(true, "CraftBadgeCommand");
                if (manager.playerModel.LastCraftedBadge != null)
                {
                    BadgeReceivePopup.OpenForBadge(manager.playerModel.LastCraftedBadge);
                }
            }
            tab.UpdateUI();
        }

        public ModelRandomMod GetDedicatedRandom(string identifier, TWDModelManager manager)
        {          
            int seed = (int)ModelHelpers.MD5SumLong(manager.Player.HashedId + identifier);
            
            return new ModelRandomMod(seed);
        }

        public string CreateBonusTypeIndex(List<CurrencyType> usedComponents)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            CurrencyType[] array = new CurrencyType[4]
            {
            CurrencyType.Metal0,
            CurrencyType.Cloth0,
            CurrencyType.Chemicals0,
            CurrencyType.Food0
            };
            for (int i = 0; i < (usedComponents?.Count ?? 0); i++)
            {
                CurrencyType componentBaseCurrency = ComponentHelper.GetComponentBaseCurrency(usedComponents[i]);
                int key = Array.IndexOf(array, componentBaseCurrency) + 1;
                if (dictionary.TryGetValue(key, out int value))
                {
                    dictionary[key] = value + 1;
                }
                else
                {
                    dictionary.Add(key, 1);
                }
            }

            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
            list.AddRange(dictionary);
            list.StableSort(delegate (KeyValuePair<int, int> a, KeyValuePair<int, int> b)
            {
                KeyValuePair<int, int> keyValuePair = a;
                KeyValuePair<int, int> keyValuePair2 = b;
                return keyValuePair.Value.CompareTo(keyValuePair2.Value) * -1;
            });
            ResetKeysToZeroForSameValues(ref list);
            StringBuilder stringBuilder = new StringBuilder();
            for (int j = 0; j < list.Count && j < 2; j++)
            {
                stringBuilder.Append(list[j].Key);
            }

            if (stringBuilder.Length < 2)
            {
                stringBuilder.Append('0', 2 - stringBuilder.Length);
            }

            return stringBuilder.ToString();
        }

        private void ResetKeysToZeroForSameValues(ref List<KeyValuePair<int, int>> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                KeyValuePair<int, int> keyValuePair = list[i];
                bool flag = false;
                for (int j = i + 1; j < list.Count; j++)
                {
                    KeyValuePair<int, int> keyValuePair2 = list[j];
                    if (keyValuePair.Value == keyValuePair2.Value)
                    {
                        list[j] = new KeyValuePair<int, int>(0, keyValuePair.Value);
                        flag = true;
                    }
                }

                if (flag)
                {
                    list[i] = new KeyValuePair<int, int>(0, keyValuePair.Value);
                }
            }
        }

    }
}
