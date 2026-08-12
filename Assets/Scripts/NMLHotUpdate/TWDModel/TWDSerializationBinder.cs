using System;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace TWDModel
{
	public class TWDSerializationBinder : DefaultSerializationBinder
	{
		private static readonly string ClientAssemblyName = "Assembly-CSharp";

		private static readonly string ClientHotUpdateAssemblyName = "NMLHotUpdate";

		private static readonly string ServerAssemblyName = "Driller.Games.WalkingDead";

		private static readonly string DashboardAssemblyName = "LiveOps.Tools.Shared";

		private static readonly string DashboardAssemblyNameOld = "LiveOps.Tools.Games.WalkingDead";

		public override Type BindToType(string assemblyName, string typeName)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			if (executingAssembly.FullName.StartsWith(ClientAssemblyName) && (assemblyName == ClientHotUpdateAssemblyName || assemblyName == ServerAssemblyName || assemblyName == DashboardAssemblyName || assemblyName == DashboardAssemblyNameOld))
			{
				assemblyName = ClientAssemblyName;
			}
			else if (executingAssembly.FullName.StartsWith(ClientHotUpdateAssemblyName) && (assemblyName == ClientAssemblyName || assemblyName == ServerAssemblyName || assemblyName == DashboardAssemblyName || assemblyName == DashboardAssemblyNameOld))
			{
				assemblyName = ClientHotUpdateAssemblyName;
			}
			else if (executingAssembly.FullName.StartsWith(ServerAssemblyName) && (assemblyName == ClientHotUpdateAssemblyName || assemblyName == ClientAssemblyName || assemblyName == DashboardAssemblyName || assemblyName == DashboardAssemblyNameOld))
			{
				assemblyName = ServerAssemblyName;
			}
			else if (executingAssembly.FullName.StartsWith(DashboardAssemblyName) && (assemblyName == ClientHotUpdateAssemblyName || assemblyName == ServerAssemblyName || assemblyName == ClientAssemblyName || assemblyName == DashboardAssemblyNameOld))
			{
				assemblyName = DashboardAssemblyName;
			}
			return base.BindToType(assemblyName, typeName);
		}
	}
}
