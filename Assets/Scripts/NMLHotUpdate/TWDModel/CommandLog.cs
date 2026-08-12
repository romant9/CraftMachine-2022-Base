using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class CommandLog
	{
		public List<CommandLogEntry> CommandLogEntries;

		public CommandLogEntry CurrentCommandLogEntry
		{
			get
			{
				return null;
			}
			private set
			{
			}
		}

		public void StartCommandExecution(ModelCommand command, IModelObject modelObject)
		{
			if (CurrentCommandLogEntry == null)
			{
				CurrentCommandLogEntry = new CommandLogEntry
				{
					Command = command,
					ModelObject = modelObject
				};
			}
		}

		public void EndCommandExecution(bool success)
		{
			if (CurrentCommandLogEntry != null)
			{
				if (CommandLogEntries == null)
				{
					CommandLogEntries = new List<CommandLogEntry>();
				}
				CurrentCommandLogEntry.Success = success;
				CommandLogEntries.Add(CurrentCommandLogEntry);
				CurrentCommandLogEntry = null;
			}
		}

		public void ServerCommandResponse(int sequenceId, int responseCode)
		{
			if (CommandLogEntries == null)
			{
				return;
			}
			for (int num = CommandLogEntries.Count - 1; num >= 0; num--)
			{
				ModelCommand command = CommandLogEntries[num].Command;
				if (command != null && command.SequenceId == sequenceId)
				{
					CommandLogEntries[num].ServerResponseCode = responseCode;
				}
			}
		}
	}
}
