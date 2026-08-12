namespace Client.Connectivity
{
	public class SignalRClientStatistics
	{
		public const int RTTSampleCount = 10;

		public int ConnectCount;

		public int DisconnectCount;

		public int ReconnectCount;

		public int ErrorCount;

		public string LastError;

		public ErrorType LastErrorType;

		public bool HasCommandExecutionError;

		public long LastSendTimeTicks;

		public long[] RTTs;

		public int RTTIndex;

		public int RTTCount;

		public long LastRTT
		{
			get
			{
				int num = RTTIndex - 1;
				if (num < 0)
				{
					num = RTTs.Length - 1;
				}
				return RTTs[num];
			}
		}

		public long AverageRTT
		{
			get
			{
				if (RTTCount > 0)
				{
					long num = 0L;
					for (int i = 0; i < RTTCount; i++)
					{
						num += RTTs[i];
					}
					return num / RTTCount;
				}
				return 0L;
			}
		}

		public SignalRClientStatistics()
		{
			RTTs = new long[10];
			RTTIndex = 0;
			RTTCount = 0;
		}

		public void Clear()
		{
			ConnectCount = 0;
			DisconnectCount = 0;
			ReconnectCount = 0;
			ErrorCount = 0;
			LastError = "";
			LastErrorType = ErrorType.None;
			LastSendTimeTicks = 0L;
			HasCommandExecutionError = false;
		}

		public void SetLastRTT(long rtt)
		{
			RTTs[RTTIndex] = rtt;
			RTTIndex++;
			RTTCount = ((RTTIndex > RTTCount) ? RTTIndex : RTTCount);
			if (RTTIndex >= 10)
			{
				RTTIndex = 0;
			}
		}

		public override string ToString()
		{
			return "Connects = " + ConnectCount + ", Disconnects = " + DisconnectCount + ", Reconnects = " + ReconnectCount + ", LastRTT = " + LastRTT + "ms, AverageRTT = " + AverageRTT + ", ErrorMsg = " + LastError + ", ErrorType = " + LastErrorType;
		}
	}
}
