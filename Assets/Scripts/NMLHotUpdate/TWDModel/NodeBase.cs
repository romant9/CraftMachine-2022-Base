using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public abstract class NodeBase : TWDModelObject, INodeBase
	{
		public int guidHash = Guid.NewGuid().GetHashCode();

		public List<NodeConnection> nodeConnections = new List<NodeConnection>();

		public List<NodeConnection> nodeDataConnections = new List<NodeConnection>();

		private Dictionary<string, List<NodeDelegate_Void>> runtimeDelegateMap = new Dictionary<string, List<NodeDelegate_Void>>();

		private Dictionary<string, List<RuntimeExportPinInfo>> runtimePropertyInfoMap = new Dictionary<string, List<RuntimeExportPinInfo>>();

		[JsonIgnore]
		public int GuidHash
		{
			get
			{
				return guidHash;
			}
			set
			{
				guidHash = value;
			}
		}

		public int GraphHash { get; set; }

		[JsonIgnore]
		public List<NodeConnection> NodeConnections => nodeConnections;

		[JsonIgnore]
		public List<NodeConnection> NodeDataConnections => nodeDataConnections;

		[JsonIgnore]
		public Dictionary<string, List<NodeDelegate_Void>> RuntimeDelegateMap => runtimeDelegateMap;

		[JsonIgnore]
		public Dictionary<string, List<RuntimeExportPinInfo>> RuntimePropertyInfoMap => runtimePropertyInfoMap;

		[JsonIgnore]
		public bool IsClientOnly => false;

		public event NodeChangedHandler NodeChanged;

		public void Fire(string id)
		{
			INodeBaseHelpers.Fire(this, id);
		}

		public bool HasConnections(string id)
		{
			return INodeBaseHelpers.HasConnections(this, id);
		}

		public object Import(string id)
		{
			return INodeBaseHelpers.Import(this, id);
		}

		public List<object> ImportValues(string id)
		{
			return INodeBaseHelpers.ImportValues(this, id);
		}

		public void Export(string id, object value)
		{
			INodeBaseHelpers.Export(this, id, value);
		}

		public void ExportValues(string id, object value)
		{
			INodeBaseHelpers.ExportValues(this, id, value);
		}

		public virtual void Update()
		{
		}

		public override bool IsValid()
		{
			return true;
		}

		public void NotifyChanged(string eventName, object args)
		{
			this.NodeChanged?.Invoke(eventName, args);
		}

		public NodeBase(NodeBase node)
		{
			guidHash = node.guidHash;
			nodeConnections = new List<NodeConnection>(node.nodeConnections);
			nodeDataConnections = new List<NodeConnection>(node.nodeDataConnections);
			GraphHash = node.GraphHash;
		}

		public NodeBase()
		{
		}

		public abstract NodeBase RecordValue();

		public virtual void ClearListener()
		{
		}
	}
}
