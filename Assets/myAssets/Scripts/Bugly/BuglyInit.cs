using System.Collections.Generic;
using UnityEngine;

public class BuglyInit : MonoBehaviour
{
	private const string BuglyAppID = "YOUR APP ID GOES HERE";

	private void Awake()
	{
		BuglyAgent.ConfigDebugMode(enable: false);
		BuglyAgent.ConfigDefault(null, null, null, 0L);
		BuglyAgent.ConfigAutoReportLogLevel(LogSeverity.LogError);
		BuglyAgent.ConfigAutoQuitApplication(autoQuit: false);
		BuglyAgent.RegisterLogCallback(null);
		BuglyAgent.InitWithAppId("YOUR APP ID GOES HERE");
		BuglyAgent.EnableExceptionHandler();
		BuglyAgent.SetLogCallbackExtrasHandler(MyLogCallbackExtrasHandler);
		Object.Destroy(this);
	}

	private static Dictionary<string, string> MyLogCallbackExtrasHandler()
	{
		BuglyAgent.PrintLog(LogSeverity.Log, "extra handler");
		Dictionary<string, string> result = new Dictionary<string, string>
		{
			{
				"ScreenSolution",
				$"{Screen.width}x{Screen.height}"
			},
			{
				"deviceModel",
				SystemInfo.deviceModel
			},
			{
				"deviceName",
				SystemInfo.deviceName
			},
			{
				"deviceType",
				SystemInfo.deviceType.ToString()
			},
			{
				"deviceUId",
				SystemInfo.deviceUniqueIdentifier
			},
			{
				"gDId",
				$"{SystemInfo.graphicsDeviceID}"
			},
			{
				"gDName",
				SystemInfo.graphicsDeviceName
			},
			{
				"gDVdr",
				SystemInfo.graphicsDeviceVendor
			},
			{
				"gDVer",
				SystemInfo.graphicsDeviceVersion
			},
			{
				"gDVdrID",
				$"{SystemInfo.graphicsDeviceVendorID}"
			},
			{
				"graphicsMemorySize",
				$"{SystemInfo.graphicsMemorySize}"
			},
			{
				"systemMemorySize",
				$"{SystemInfo.systemMemorySize}"
			},
			{
				"UnityVersion",
				Application.unityVersion
			}
		};
		BuglyAgent.PrintLog(LogSeverity.LogInfo, "Package extra data");
		return result;
	}
}
