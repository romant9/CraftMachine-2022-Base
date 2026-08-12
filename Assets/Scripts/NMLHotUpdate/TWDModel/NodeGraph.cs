using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	[Serializable]
	public class NodeGraph : TWDModelObject
	{
		public int GuidHash { get; set; }

		public ModelList<NodeBase> Nodes { get; set; }

		public NodeGraph()
		{
			Nodes = new ModelList<NodeBase>();
		}

		public override void Initialize()
		{
			base.Initialize();
			for (int i = 0; i < Nodes.Count; i++)
			{
				Nodes[i].GraphHash = GuidHash;
				Nodes[i].Initialize();
			}
		}

		public override void Start()
		{
			base.Start();
			List<INodeBase> list = new List<INodeBase>();
			int count = Nodes.Count;
			for (int i = 0; i < count; i++)
			{
				NodeBase item = Nodes[i];
				list.Add(item);
			}
			for (int j = 0; j < Nodes.Count; j++)
			{
				Nodes[j].GraphHash = GuidHash;
				INodeBaseHelpers.SetupRuntimeConnections(Nodes[j], list);
			}
		}

		public INodeBase GetNode(int nodeGuidHash)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				if (Nodes[i].guidHash == nodeGuidHash)
				{
					return Nodes[i];
				}
			}
			return null;
		}

		public void Backup(List<NodeBase> records)
		{
			foreach (NodeBase node in Nodes)
			{
				node.ClearListener();
			}
			Nodes.Clear();
			foreach (NodeBase record in records)
			{
				record.SetManager(base.manager);
				record.Start();
				Nodes.Add(record);
			}
			List<INodeBase> list = new List<INodeBase>();
			int count = Nodes.Count;
			for (int i = 0; i < count; i++)
			{
				NodeBase item = Nodes[i];
				list.Add(item);
			}
			for (int j = 0; j < Nodes.Count; j++)
			{
				Nodes[j].GraphHash = GuidHash;
				INodeBaseHelpers.SetupRuntimeConnections(Nodes[j], list);
			}
		}

		public void AddNode(NodeBase nodeBase)
		{
			Nodes.Add(nodeBase);
		}

		public void RemoveNode(NodeBase nodeBase)
		{
			Nodes.Remove(nodeBase);
			for (int i = 0; i < Nodes.Count; i++)
			{
				INodeBaseHelpers.RemoveConnectionsTo(Nodes[i], nodeBase);
			}
		}

		public void Update()
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				Nodes[i].Update();
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
