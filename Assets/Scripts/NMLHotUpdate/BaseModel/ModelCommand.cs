using System.Collections;
using System.Collections.Generic;

namespace BaseModel
{
	public class ModelCommand : IModelCommand
	{
		public int SequenceId { get; set; }

		public long Time { get; set; }

		public int ModelId { get; set; }

		public Dictionary<string, string> ModelState { get; set; }

		public virtual ModelRespondCode respondCode { get; protected set; }

		public ModelCommand()
		{
		}

		public ModelCommand(ModelObject model)
		{
			ModelId = model.ModelId;
		}

		public ModelCommand(int modelId)
		{
			ModelId = modelId;
		}

		public ModelCommand(int modelId, long time, int sequenceId)
		{
			ModelId = modelId;
			Time = time;
			SequenceId = sequenceId;
		}

		public void SetParameters(long time, int sequenceId)
		{
			Time = time;
			SequenceId = sequenceId;
		}

		public virtual IModelCommandRespond Execute(ModelManager manager)
		{
			return null;
		}

		public virtual IEnumerator ExecuteForClient(ModelManager manager)
		{
			yield break;
		}
	}
}
