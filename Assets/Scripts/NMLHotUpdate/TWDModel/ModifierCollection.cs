using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class ModifierCollection : TWDModelObject, IModifierCollection
	{
		private class Enumerator : IEnumerator
		{
			private int index = -1;

			private ModifierCollection collection;

			public object Current
			{
				get
				{
					if (index >= 0)
					{
						if (index < collection.actionModifiers.Count)
						{
							return collection.actionModifiers[index];
						}
						index -= collection.actionModifiers.Count;
						if (index < collection.paramModifiers.Count)
						{
							return collection.paramModifiers[index];
						}
					}
					return null;
				}
			}

			public Enumerator(ModifierCollection collection)
			{
				this.collection = collection;
			}

			public bool MoveNext()
			{
				index++;
				return index - collection.actionModifiers.Count < collection.paramModifiers.Count;
			}

			public void Reset()
			{
				index = -1;
			}
		}

		private ModelList<ActionModifier> actionModifiers;

		private ModelList<ParameterModifier> paramModifiers;

		private Dictionary<string, List<ParameterModifier>> paramModifiersByName = new Dictionary<string, List<ParameterModifier>>();

		private List<ParameterModifier> unindexedParamModifiers = new List<ParameterModifier>();

		public int GetCount()
		{
			return actionModifiers.Count() + paramModifiers.Count();
		}

		public ModelModifier GetModifier(int index)
		{
			if (index >= 0)
			{
				if (index < actionModifiers.Count)
				{
					return actionModifiers[index];
				}
				index -= actionModifiers.Count;
				if (index < paramModifiers.Count)
				{
					return paramModifiers[index];
				}
			}
			return null;
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			actionModifiers = new ModelList<ActionModifier>();
			actionModifiers.SetManager(base.manager);
			actionModifiers.Initialize();
			paramModifiers = new ModelList<ParameterModifier>();
			paramModifiers.SetManager(base.manager);
			paramModifiers.Initialize();
		}

		private void IndexParameterModifier(ParameterModifier paramModifier)
		{
			string[] parameterNames = paramModifier.GetParameterNames();
			if (parameterNames != null)
			{
				for (int i = 0; i < parameterNames.Length; i++)
				{
					if (!paramModifiersByName.TryGetValue(parameterNames[i], out var value))
					{
						value = new List<ParameterModifier>();
						paramModifiersByName[parameterNames[i]] = value;
					}
					value.Add(paramModifier);
				}
			}
			else
			{
				unindexedParamModifiers.Add(paramModifier);
			}
		}

		private void UnindexParameterModifier(ParameterModifier paramModifier)
		{
			string[] parameterNames = paramModifier.GetParameterNames();
			if (parameterNames != null)
			{
				for (int i = 0; i < parameterNames.Length; i++)
				{
					if (paramModifiersByName.TryGetValue(parameterNames[i], out var value))
					{
						value.Remove(paramModifier);
						if (value.Count == 0)
						{
							paramModifiersByName.Remove(parameterNames[i]);
						}
					}
				}
			}
			else
			{
				unindexedParamModifiers.Remove(paramModifier);
			}
		}

		public void RegisterModifier(ModelModifier modifier)
		{
			if (modifier is ActionModifier)
			{
				actionModifiers.Add(modifier as ActionModifier);
			}
			else
			{
				if (!(modifier is ParameterModifier))
				{
					throw new Exception("Unsupported model modifier type!");
				}
				ParameterModifier parameterModifier = modifier as ParameterModifier;
				paramModifiers.Add(parameterModifier);
				IndexParameterModifier(parameterModifier);
			}
			modifier.OnAdded(this);
		}

		public void RemoveModifier(ModelModifier modifier)
		{
			if (modifier == null)
			{
				return;
			}
			bool flag = false;
			if (modifier is ActionModifier)
			{
				int num = actionModifiers.IndexOf(modifier as ActionModifier);
				if (num >= 0)
				{
					actionModifiers.RemoveAt(num);
					flag = true;
				}
			}
			else if (modifier is ParameterModifier)
			{
				ParameterModifier parameterModifier = modifier as ParameterModifier;
				int num2 = paramModifiers.IndexOf(parameterModifier);
				if (num2 >= 0)
				{
					paramModifiers.RemoveAt(num2);
					UnindexParameterModifier(parameterModifier);
					flag = true;
				}
			}
			if (!flag)
			{
				throw new Exception("Trying to remove modifier from collection it does not belong to. Modifier type: " + modifier.GetType().Name);
			}
			modifier.OnRemoved(this);
		}

		public bool HasModifier(ModelModifier modifier)
		{
			if (modifier == null)
			{
				return false;
			}
			if (modifier is ActionModifier)
			{
				return actionModifiers.Contains(modifier as ActionModifier);
			}
			if (modifier is ParameterModifier)
			{
				return paramModifiers.Contains(modifier as ParameterModifier);
			}
			return false;
		}

		public bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			bool result = false;
			int num = 0;
			if (paramModifiersByName.TryGetValue(paramName, out var value2))
			{
				num = value2.Count;
				for (int i = 0; i < num; i++)
				{
					if (value2[i].VisitParameter(paramName, ref value, actor))
					{
						result = true;
					}
				}
			}
			num = unindexedParamModifiers.Count;
			for (int j = 0; j < num; j++)
			{
				if (unindexedParamModifiers[j].VisitParameter(paramName, ref value, actor))
				{
					result = true;
				}
			}
			return result;
		}

		public void VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			List<ActionModifier> models = actionModifiers.Models;
			List<ModelAction> list = new List<ModelAction>();
			for (int i = 0; i < models.Count; i++)
			{
				ActionModifier actionModifier = models[i];
				list.Clear();
				ActionListClearFlag actionListClearFlag = actionModifier.VisitActions(action, actor, list);
				if (base.manager.CurrentCommandLogEntry != null)
				{
					base.manager.CurrentCommandLogEntry.ActionModifier(actor, actionModifier, actionListClearFlag, list);
				}
				if (actionListClearFlag == ActionListClearFlag.Clear)
				{
					addedActions.Clear();
				}
				if (list.Count > 0)
				{
					addedActions.AddRange(list);
				}
			}
		}
	}
}
