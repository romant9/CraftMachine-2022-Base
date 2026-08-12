namespace TWDModel
{
	public class LeftGuildGroupCommand : TWDGroupCommand
	{
		public string LeaverId { get; set; }

		public string Reason { get; set; }

		public GuildLeaveType LeaveType { get; set; }
	}
}
