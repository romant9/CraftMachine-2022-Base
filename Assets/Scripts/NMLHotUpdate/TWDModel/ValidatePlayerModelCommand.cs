using System;
using System.IO;
using System.Reflection;
using BaseModel;

namespace TWDModel
{
	public class ValidatePlayerModelCommand : ModelCommand
	{
		public string ModelJSON;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.OK;
			string environmentVariable = Environment.GetEnvironmentVariable("LOCALAPPDATA");
			if (environmentVariable != null)
			{
				TWDModelManager tWDModelManager = manager as TWDModelManager;
				bool flag = Assembly.GetExecutingAssembly().FullName.StartsWith("Assembly-CSharp");
				int num = 0;
				string text = string.Format("TWDModel-{0:000}-{1}-{2}.txt", num, "list", flag ? "client" : "server");
				string text2 = string.Format("TWDModel-{0:000}-{1}-{2}.txt", num, "json", flag ? "client" : "server");
				string text3 = environmentVariable + "/NextGames";
				Directory.CreateDirectory(text3);
				using (StreamWriter streamWriter = new StreamWriter(new FileStream(text3 + "/" + text, FileMode.Create, FileAccess.Write)))
				{
					streamWriter.Write(tWDModelManager.GetDebugModelsList());
					streamWriter.Flush();
				}
				using StreamWriter streamWriter2 = new StreamWriter(new FileStream(text3 + "/" + text2, FileMode.Create, FileAccess.Write));
				streamWriter2.Write(tWDModelManager.GetDebugJSON());
				streamWriter2.Flush();
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
