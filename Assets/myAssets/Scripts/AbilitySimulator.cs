using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class AbilitySimulator : MonoBehaviour, IModifierCollection
{
    public static TWDModelManager manager => DataManager.Instance.ModelManager;
    public static PlayerModel Player => DataManager.Instance.Player;


    public AbilityModel AbilityUnderApplication { get; private set; }
    public ModifierCollection modifierCollection { get; private set; }
    private Dictionary<Faction, FactionLeaderModifiers> factionModifierCollections = new Dictionary<Faction, FactionLeaderModifiers>();
    public ModifierCollection survivorBuffsCollection { get; private set; }

    public ModifierCollection survivorGuildBattleBuffsCollection { get; private set; }

    public ModifierCollection featuredHeroBuffsCollection { get; private set; }



    void Start()
    {
        
    }

    void Update()
    {        
    }


    //ModifierCollection
    //public void VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
    //{
    //    List<ModelAction> list = new List<ModelAction>();
    //    for (int i = 0; i < actionModifiers.Count; i++)
    //    {
    //        ActionModifier actionModifier = actionModifiers[i];
    //        list.Clear();
    //        ActionListClearFlag actionListClearFlag = actionModifier.VisitActions(action, actor, list);
    //        if (base.manager.CurrentCommandLogEntry != null)
    //        {
    //            base.manager.CurrentCommandLogEntry.ActionModifier(actor, actionModifier, actionListClearFlag, list);
    //        }
    //        if (actionListClearFlag == ActionListClearFlag.Clear)
    //        {
    //            addedActions.Clear();
    //        }
    //        if (list.Count > 0)
    //        {
    //            addedActions.AddRange(list);
    //        }
    //    }
    //}

    private ModelAction PostChangeTurnAction()
    {
        var action = new TWDModel.PostChangeTurnAction();
        action.Visited = false;
        return action as ModelAction;
    }

    private ModelAction ActorMoveAction()
    {
        var actor = manager.CombatModel.GetAllActors().First();
        var action = new ModelAction(actor);
        action.Visited = false;
        return action as ModelAction;
    }

    private ModelAction ActorTriggerAction()
    {
        var actor = manager.CombatModel.GetAllActors().First();
        TriggerModel trigger = new TriggerModel();
        //путь
        var action = new TriggerAction(actor, trigger);
        action.Visited = false;
        return action as ModelAction;
    }

    private ModelAction ActorPreAttackAction()
    {
        var DamagerActor = manager.CombatModel.GetAllActors().First();
        var TargetActor = manager.CombatModel.GetAllActors()[3];

        var action = new PreAttackAction(DamagerActor, TargetActor);
        action.Visited = false;
        return action as ModelAction;
    }

    private ModelAction ActorAbilityAction()
    {
        ActorModel sourceActor = manager.CombatModel.GetAllActors().First();
        ActorModel targetActor = null;
        var ability = sourceActor.SelectedAbility;
        GridCoordinate cell = new GridCoordinate(12, 3);
        //AbilityName - Ability Pistol
        //DefinitionID - WeaponAbilityPistol_Blizzard

        var action = new AbilityAction(sourceActor, ability, cell, targetActor, OOTType.None);
        action.Visited = false;
        return action as ModelAction;
    }

    public void RunEmptyAction()
    {
        VisitActions(PostChangeTurnAction(), null, new List<ModelAction>());
    }

    public void RunMoveAction()
    {
        VisitActions(ActorMoveAction(), null, new List<ModelAction>());
    }

    public void RunTriggerAction()
    {
        VisitActions(ActorTriggerAction(), null, new List<ModelAction>());
    }

    public void RunPreAttackAction()
    {
        VisitActions(ActorPreAttackAction(), null, new List<ModelAction>());
    }

    public void RunAbilityAction()
    {
        VisitActions(ActorAbilityAction(), null, new List<ModelAction>());
    }

    public void VisitActions(ModelAction action, ActorModel nullActor, List<ModelAction> addedActions)
    {
        if (AbilityUnderApplication != null && AbilityUnderApplication.Definition.Type != 0)
        {
            AbilityUnderApplication.Modifiers.VisitActions(action, null, addedActions);
        }
        if (manager.CombatModel != null)
        {
            ActorModel activeActor = manager.CombatModel.ActiveActor;
            if (activeActor != null)
            {
                for (int i = 0; i < activeActor.Abilities.Count; i++)
                {
                    AbilityModel abilityModel = activeActor.Abilities[i];
                    if (abilityModel.Definition.Type == AbilityType.Passive)
                    {
                        abilityModel.Modifiers.VisitActions(action, activeActor, addedActions);
                    }
                }
                if (activeActor.Modifiers != null)
                {
                    activeActor.Modifiers.VisitActions(action, activeActor, addedActions);
                }
            }
            List<ActorModel> allActors = manager.CombatModel.GetAllActors();
            for (int j = 0; j < allActors.Count; j++)
            {
                ActorModel actorModel = allActors[j];
                if (actorModel == activeActor)
                {
                    continue;
                }
                for (int k = 0; k < actorModel.Abilities.Count; k++)
                {
                    AbilityModel abilityModel2 = actorModel.Abilities[k];
                    if (abilityModel2.Definition.Type == AbilityType.Passive)
                    {
                        abilityModel2.Modifiers.VisitActions(action, actorModel, addedActions);
                    }
                }
                if (actorModel.Modifiers != null)
                {
                    actorModel.Modifiers.VisitActions(action, actorModel, addedActions);
                }
            }
        }
        modifierCollection.VisitActions(action, null, addedActions);
        action.Visited = true;
    }

    #region VisitParameter
    bool IModifierCollection.VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
    {
        return VisitParameterWithAbility(AbilityUnderApplication, paramName, ref value, actor);
    }
    public bool VisitParameterWithAbility(AbilityModel ability, string paramName, ref FixedPoint value, ActorModel actor = null)
    {
        bool flag = false;
        FixedPoint oldValue = value;
        if (ability != null && ability.Definition.Type != 0)
        {
            oldValue = value;
            if (ability.Modifiers.VisitParameter(paramName, ref value, actor))
            {
                if (manager.CurrentCommandLogEntry != null)
                {
                    manager.CurrentCommandLogEntry.ParameterModifiedAbilityActive(paramName, oldValue, value, actor, ability);
                }
                flag = true;
            }
        }
        if (!flag)
        {
            return VisitParameter(paramName, ref value, ref oldValue, actor);
        }
        return true;
    }
    private bool VisitParameter(string paramName, ref FixedPoint value, ref FixedPoint oldValue, ActorModel actor = null)
    {
        bool result = false;
        if (actor == null && manager.CombatModel != null)
        {
            actor = manager.CombatModel.ActiveActor;
        }
        if (actor != null)
        {
            for (int i = 0; i < actor.Abilities.Count; i++)
            {
                AbilityModel abilityModel = actor.Abilities[i];
                if (abilityModel.Definition.Type != 0)
                {
                    continue;
                }
                oldValue = value;
                if (abilityModel.Modifiers.VisitParameter(paramName, ref value, actor))
                {
                    if (manager.CurrentCommandLogEntry != null)
                    {
                        manager.CurrentCommandLogEntry.ParameterModifiedAbilityPassive(paramName, oldValue, value, actor, abilityModel);
                    }
                    result = true;
                }
            }
            if (actor.Modifiers != null)
            {
                oldValue = value;
                if (actor.Modifiers.VisitParameter(paramName, ref value, actor))
                {
                    if (manager.CurrentCommandLogEntry != null)
                    {
                        manager.CurrentCommandLogEntry.ParameterModifiedActorPassive(paramName, oldValue, value, actor);
                    }
                    result = true;
                }
            }
            FactionLeaderModifiers value2 = null;
            if (factionModifierCollections.TryGetValue(actor.Faction, out value2) && value2 != null && value2.Leader != actor)
            {
                oldValue = value;
                if (value2 != null && value2.VisitParameter(paramName, ref value, actor))
                {
                    if (manager.CurrentCommandLogEntry != null)
                    {
                        manager.CurrentCommandLogEntry.ParameterModifiedFactionPassive(paramName, oldValue, value, actor);
                    }
                    result = true;
                }
            }
            if (survivorBuffsCollection != null)
            {
                oldValue = value;
                if (survivorBuffsCollection.VisitParameter(paramName, ref value, actor))
                {
                    if (manager.CurrentCommandLogEntry != null)
                    {
                        manager.CurrentCommandLogEntry.ParameterModifiedFactionPassive(paramName, oldValue, value, actor);
                    }
                    result = true;
                }
            }
            if (survivorGuildBattleBuffsCollection != null)
            {
                oldValue = value;
                if (survivorGuildBattleBuffsCollection.VisitParameter(paramName, ref value, actor))
                {
                    if (manager.CurrentCommandLogEntry != null)
                    {
                        manager.CurrentCommandLogEntry.ParameterModifiedFactionPassive(paramName, oldValue, value, actor);
                    }
                    result = true;
                }
            }
            if (featuredHeroBuffsCollection != null)
            {
                oldValue = value;
                if (featuredHeroBuffsCollection.VisitParameter(paramName, ref value, actor))
                {
                    if (manager.CurrentCommandLogEntry != null)
                    {
                        manager.CurrentCommandLogEntry.ParameterModifiedFactionPassive(paramName, oldValue, value, actor);
                    }
                    result = true;
                }
            }
        }
        return result;
    }
    #endregion

    #region ExecuteAction
    public bool ExecuteAction(ModelAction action)
    {
        bool flag = true;

        List<ModelAction> additionalActions = new List<ModelAction>();
        Player.AbilityManager.VisitActions(action, null, additionalActions);
        if (Player.GetAttackTargetMissionModel() is MapMissionModel mapMissionModel && (mapMissionModel.IsInWeeklyChallenge || mapMissionModel.IsInApocalyptiWeeklyChallenge))
        {
            mapMissionModel.VisitActions(action, null, additionalActions);
        }

        try
        {
            if (action.CanExecute())
            {
                bool flag2 = action.Execute(manager);
                flag = flag && flag2;
                if (!flag)
                {
                    DebugTWD.Log("TWDModelManager.ExecuteAction failed for action '" + action.GetType().Name + "'!", DebugType.BattleBase);
                }
            }
        }
        catch (Exception exception)
        {
            string text = exception.Message + exception.StackTrace;
            DebugTWD.LogError("TWDModelManager.ExecuteAction exception: " + text);
            flag = false;
        }

        Dictionary<Type, List<ModelAction>> groupedActions = GetGroupedActions(additionalActions);
        FilterAndSortGroupedActions(ref groupedActions, ref additionalActions);
        foreach (List<ModelAction> value in groupedActions.Values)
        {
            for (int i = 0; i < value.Count; i++)
            {
                ModelAction action2 = value[i];
                bool flag3 = ExecuteAction(action2);
                flag = flag && flag3;
            }
        }
        additionalActions.StableSort((ModelAction a, ModelAction b) => a.SortOrder().CompareTo(b.SortOrder()));
        for (int j = 0; j < additionalActions.Count; j++)
        {
            ModelAction action3 = additionalActions[j];
            bool flag4 = ExecuteAction(action3);
            flag = flag && flag4;
        }
        return flag;
    }
    private Dictionary<Type, List<ModelAction>> GetGroupedActions(List<ModelAction> additionalActions)
    {
        Dictionary<Type, List<ModelAction>> dictionary = new Dictionary<Type, List<ModelAction>>();
        for (int i = 0; i < (additionalActions?.Count ?? 0); i++)
        {
            ModelAction modelAction = additionalActions[i];
            List<ModelAction> value = null;
            if (modelAction.HasOrderWhenGrouped())
            {
                if (!dictionary.TryGetValue(modelAction.GetType(), out value))
                {
                    value = new List<ModelAction>();
                    dictionary.Add(modelAction.GetType(), value);
                }
                value.Add(modelAction);
            }
        }
        return dictionary;
    }
    private void FilterAndSortGroupedActions(ref Dictionary<Type, List<ModelAction>> groupedActions, ref List<ModelAction> additionalActions)
    {
        foreach (List<ModelAction> value in groupedActions.Values)
        {
            value.StableSort((ModelAction a, ModelAction b) => a.SortOrder().CompareTo(b.SortOrder()));
            for (int i = 0; i < (value?.Count ?? 0); i++)
            {
                if (additionalActions.Contains(value[i]))
                {
                    additionalActions.Remove(value[i]);
                }
            }
        }
    }

    #endregion

    void IModifierCollection.RegisterModifier(ModelModifier modifier)
    {
        modifierCollection.RegisterModifier(modifier);
    }

    void IModifierCollection.RemoveModifier(ModelModifier modifier)
    {
        modifierCollection.RemoveModifier(modifier);
    }

    bool IModifierCollection.HasModifier(ModelModifier modifier)
    {
        return modifierCollection.HasModifier(modifier);
    }

    int IModifierCollection.GetCount()
    {
        return modifierCollection.GetCount();
    }

    ModelModifier IModifierCollection.GetModifier(int index)
    {
        return modifierCollection.GetModifier(index);
    }
}
