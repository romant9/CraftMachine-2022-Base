using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class TraitActiveVisualizationsManager : IDisposable
{
	private struct TraitVisualizationEntry : IEquatable<TraitVisualizationEntry>
	{
		public readonly ActorModel ActorModel;

		public readonly string Trait;

		public TraitVisualizationEntry(ActorModel actorModel, string trait)
		{
			ActorModel = actorModel;
			Trait = trait;
		}

		public bool Equals(TraitVisualizationEntry other)
		{
			if (ActorModel == other.ActorModel)
			{
				return Trait == other.Trait;
			}
			return false;
		}
	}

	private readonly TraitActiveVisaulizationResources resources;

	private readonly IDictionary<TraitVisualizationEntry, TraitActiveVisualization> activeVisualizations;

	private readonly CombatModel combatModel;

	public TraitActiveVisualizationsManager(CombatModel combat)
	{
		resources = UnityUtils.LoadFromAssetBundle<TraitActiveVisaulizationResources>("TraitActiveVisaulizationResources", "scriptableobjects");
		activeVisualizations = new Dictionary<TraitVisualizationEntry, TraitActiveVisualization>();
		combatModel = combat;
		if (combatModel.Survivors != null)
		{
			foreach (ActorModel survivor in combatModel.Survivors)
			{
				InitializeActor(survivor);
			}
		}
		if (combatModel.Raiders == null)
		{
			return;
		}
		foreach (ActorModel raider in combatModel.Raiders)
		{
			InitializeActor(raider);
		}
	}

	public void Dispose()
	{
		if (combatModel.Survivors != null)
		{
			foreach (ActorModel survivor in combatModel.Survivors)
			{
				survivor.Changed -= OnActorChange;
			}
		}
		if (combatModel.Raiders == null)
		{
			return;
		}
		foreach (ActorModel raider in combatModel.Raiders)
		{
			raider.Changed -= OnActorChange;
		}
	}

	private void InitializeActor(ActorModel actorModel)
	{
		actorModel.Changed += OnActorChange;
		TraitActiveVisaulizationResourceEntry[] array = resources.resources;
		foreach (TraitActiveVisaulizationResourceEntry traitActiveVisaulizationResourceEntry in array)
		{
			RefreshActorTraitState(actorModel, traitActiveVisaulizationResourceEntry.Identifier);
		}
	}

	private void OnActorChange(ModelObject model, string changed, object args)
	{
		ActorModel actor = (ActorModel)model;
		switch (changed)
		{
		case "actorTraitGained":
		case "actorLostTrait":
			RefreshActorTraitState(actor, ((TraitDefinition)args).Identifier);
			break;
		case "actorKilledEvent":
		{
			TraitActiveVisaulizationResourceEntry[] array = resources.resources;
			foreach (TraitActiveVisaulizationResourceEntry traitActiveVisaulizationResourceEntry in array)
			{
				RefreshActorTraitState(actor, traitActiveVisaulizationResourceEntry.Identifier);
			}
			break;
		}
		}
	}

	private void RefreshActorTraitState(ActorModel actor, string trait)
	{
		TraitActiveVisaulizationResourceEntry traitActiveVisaulizationResourceEntry = resources.GetResources(trait);
		if (traitActiveVisaulizationResourceEntry == null)
		{
			return;
		}
		TraitVisualizationEntry key = new TraitVisualizationEntry(actor, trait);
		TraitActiveVisualization visualization;
		bool flag = activeVisualizations.TryGetValue(key, out visualization);
		bool flag2 = !actor.IsDead && actor.HasTrait(trait);
		if (flag2 && !flag)
		{
			TraitActiveVisualization component = Helpers.InstantiateToParent(UnityUtils.LoadFromAssetBundle<PrefabResource>(traitActiveVisaulizationResourceEntry.ResourceAddress, "scriptableobjects").GetPrefab(), GetActorView(actor).gameObject).GetComponent<TraitActiveVisualization>();
			activeVisualizations[key] = component;
		}
		else if (!flag2 && flag)
		{
			activeVisualizations.Remove(key);
			VisualizationQueue.Instance.Add(new CustomVisualizationTask(delegate
			{
				Helpers.DestroyOrCache(visualization);
			}));
		}
	}

	private static ActorView GetActorView(ActorModel actorModel)
	{
		return GameManager.Instance.GetViews<CombatView>()[0].GetActorViewFromModel(actorModel);
	}
}
