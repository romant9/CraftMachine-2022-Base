using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class TdMetrics : ICloneable
	{
		private string eventType;

		private readonly TWDModelManager manager;

		private Dictionary<string, object> properties { get; set; }

		public TdMetrics(TWDModelManager manager)
		{
			this.manager = manager;
			properties = new Dictionary<string, object>();
		}

		public TdMetrics SetEventType(string eventType)
		{
			this.eventType = eventType;
			return this;
		}

		public void Reset()
		{
			eventType = string.Empty;
			properties.Clear();
		}

		public TdMetrics AddProperty(string name, object value)
		{
			properties.Add(name, value);
			return this;
		}

		public TdMetrics SetProperties(Dictionary<string, object> properties)
		{
			if (this.properties.Count > 0)
			{
				foreach (KeyValuePair<string, object> property in properties)
				{
					if (!this.properties.ContainsKey(property.Key))
					{
						this.properties.Add(property.Key, property.Value);
					}
				}
			}
			else
			{
				this.properties = properties;
			}
			return this;
		}

		public void Send()
		{
			if (OfflineManager.IsUseSendMetrics)
			{
				manager.SendTdMetricsEvent(eventType, properties);
			}
			Reset();
		}

		public void SendUser()
		{
			if (OfflineManager.IsUseSendMetrics)
			{
				manager.SendTdUserMetricsEvent(eventType, properties);
			}
			Reset();
		}

		public object Clone()
		{
			return new TdMetrics(manager)
			{
				properties = new Dictionary<string, object>()
			};
		}
	}
}
