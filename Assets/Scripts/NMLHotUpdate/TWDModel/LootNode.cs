using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class LootNode : NodeBase
	{
		[IgnoreModelProperty]
		public LootModel LootModel { get; set; }

		[GraphItExportData("LootOpened", "")]
		public bool LootOpened { get; set; }

		[JsonIgnore]
		[GraphItExportData("Last Instigator", "Actor that performed the last operation on this door.")]
		public ActorModel LastInstigator { get; set; }

		[JsonIgnore]
		[GraphItImportData("Instigator", "Actor that performs the operation on this door.")]
		public ActorModel Instigator
		{
			get
			{
				object obj = Import("Instigator");
				if (obj is ActorModel result)
				{
					return result;
				}
				if (obj is List<ActorModel> { Count: >0 } list)
				{
					return list[0];
				}
				return null;
			}
		}

		public LootNode()
		{
		}

		public LootNode(LootNode node)
			: base(node)
		{
			LootModel = node.LootModel;
			LootOpened = node.LootOpened;
			LastInstigator = node.LastInstigator;
		}

		public override NodeBase RecordValue()
		{
			return new LootNode(this);
		}

		public override void Start()
		{
			LootModel lootModel = LootModel;
			LootModel = null;
			base.Start();
			LootOpened = false;
			LootModel = lootModel;
			base.manager.RegisterDelayedEventListener(LootModel, OnLootChanged);
		}

		public override void ClearListener()
		{
			base.ClearListener();
			base.manager.UnregisterDelayedEventListener(LootModel, OnLootChanged);
		}

		private void OnLootChanged(ModelObject model, string changed, object args)
		{
			if (changed == "IsOpened")
			{
				LastInstigator = args as ActorModel;
				if (LootModel.IsOpened)
				{
					Opened();
				}
			}
		}

		[GraphItInput("Open", "Open loot container.")]
		public void Open()
		{
			LootModel.Loot(Instigator);
		}

		[GraphItOutput("Opened", "Loot container was opened.")]
		public void Opened()
		{
			LootOpened = true;
			Fire("Opened");
		}
	}
}
