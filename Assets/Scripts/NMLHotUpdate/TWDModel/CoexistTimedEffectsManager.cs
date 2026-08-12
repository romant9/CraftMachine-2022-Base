using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class CoexistTimedEffectsManager : TWDModelObject, IDestructibleCombatModel
	{
		public ModelList<CoexistTimedEffectAbstract> CoexistTimedEffects { get; set; }

		public CoexistTimedEffectsManager()
		{
		}

		public CoexistTimedEffectsManager(CoexistTimedEffectsManager coexistTimedEffectsManager)
		{
		}

		public void StartTimedEffect(CoexistTimedEffectAbstract newCoexistTimedEffect, object args = null)
		{
			if (newCoexistTimedEffect != null)
			{
				if (CoexistTimedEffects == null)
				{
					CoexistTimedEffects = new ModelList<CoexistTimedEffectAbstract>();
					CoexistTimedEffects.Initialize();
					CoexistTimedEffects.SetManager(base.manager);
				}
				CoexistTimedEffectAbstract coexistTimedEffectAbstract = CoexistTimedEffects.Find((CoexistTimedEffectAbstract x) => x.CoexistTimedEffectType == newCoexistTimedEffect.CoexistTimedEffectType);
				if (coexistTimedEffectAbstract == null)
				{
					newCoexistTimedEffect.Initialize();
					newCoexistTimedEffect.SetManager(base.manager);
					newCoexistTimedEffect.Start();
					CoexistTimedEffects.Add(newCoexistTimedEffect);
					newCoexistTimedEffect.PostNewTimedEffect();
					UpdateModelObjects();
				}
				else
				{
					coexistTimedEffectAbstract.UpdateTimedEffect(newCoexistTimedEffect);
				}
			}
		}

		public void IncrementTimedEffects(Faction faction)
		{
			if (CoexistTimedEffects == null || CoexistTimedEffects.Count <= 0)
			{
				return;
			}
			List<CoexistTimedEffectAbstract> list = new List<CoexistTimedEffectAbstract>();
			foreach (CoexistTimedEffectAbstract coexistTimedEffect in CoexistTimedEffects)
			{
				if (coexistTimedEffect.TurnCheck && coexistTimedEffect.InstigatorFaction == faction)
				{
					coexistTimedEffect.Counter++;
					if (coexistTimedEffect.Counter >= coexistTimedEffect.Duration)
					{
						list.Add(coexistTimedEffect);
					}
				}
			}
			bool flag = false;
			foreach (CoexistTimedEffectAbstract item in list)
			{
				CoexistTimedEffects.Remove(item);
				item.PostFinishTimedEffect();
				flag = true;
			}
			if (flag)
			{
				UpdateModelObjects();
			}
		}

		public CoexistTimedEffectAbstract GetCoexistTimedEffect(CoexistTimedEffectType coexistTimedEffectType)
		{
			if (CoexistTimedEffects == null)
			{
				return null;
			}
			return CoexistTimedEffects.Find((CoexistTimedEffectAbstract x) => x.CoexistTimedEffectType == coexistTimedEffectType);
		}

		public T GetCoexistTimedEffect<T>(CoexistTimedEffectType coexistTimedEffectType) where T : CoexistTimedEffectAbstract
		{
			if (CoexistTimedEffects == null)
			{
				return null;
			}
			return CoexistTimedEffects.Find((CoexistTimedEffectAbstract x) => x.CoexistTimedEffectType == coexistTimedEffectType) as T;
		}

		public void RemoveCoexistTimedEffectByCoexistTimedEffectTypeList(List<CoexistTimedEffectType> removeCoexistTimedEffectTypes)
		{
			if (CoexistTimedEffects == null || CoexistTimedEffects.Count == 0)
			{
				return;
			}
			bool flag = false;
			List<CoexistTimedEffectAbstract> list = new List<CoexistTimedEffectAbstract>();
			foreach (CoexistTimedEffectAbstract coexistTimedEffect in CoexistTimedEffects)
			{
				if (removeCoexistTimedEffectTypes.Contains(coexistTimedEffect.CoexistTimedEffectType))
				{
					list.Add(coexistTimedEffect);
					flag = true;
				}
			}
			foreach (CoexistTimedEffectAbstract item in list)
			{
				CoexistTimedEffects.Remove(item);
				item.PostFinishTimedEffect();
			}
			if (flag)
			{
				UpdateModelObjects();
			}
		}

		public void SetupForCombat()
		{
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null)
			{
				turnManager.FactionChanged -= OnFactionChanged;
				turnManager.FactionChanged += OnFactionChanged;
			}
		}

		public void OnFactionChanged(Faction currentFaction, Faction newFaction)
		{
			if (CoexistTimedEffects == null || CoexistTimedEffects.Count == 0)
			{
				return;
			}
			foreach (CoexistTimedEffectAbstract coexistTimedEffect in CoexistTimedEffects)
			{
				coexistTimedEffect.OnFactionChanged(currentFaction, newFaction);
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Start()
		{
			base.Start();
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null)
			{
				turnManager.FactionChanged -= OnFactionChanged;
				turnManager.FactionChanged += OnFactionChanged;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			CoexistTimedEffects = new ModelList<CoexistTimedEffectAbstract>();
			CoexistTimedEffects.Initialize();
			CoexistTimedEffects.SetManager(base.manager);
		}

		public void ClearCoexistTimedEffects()
		{
			if (CoexistTimedEffects != null)
			{
				CoexistTimedEffects.Clear();
			}
		}

		public void Destroy()
		{
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null)
			{
				turnManager.FactionChanged -= OnFactionChanged;
			}
		}
	}
}
