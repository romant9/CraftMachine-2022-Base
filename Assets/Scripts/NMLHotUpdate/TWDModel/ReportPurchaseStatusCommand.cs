using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ReportPurchaseStatusCommand : ModelCommand
	{
		public string bundleIdentifier { get; set; }

		public string trackingId { get; set; }

		public Metrics.BundleSource bundleSource { get; set; }

		public PurchaseConfirmationResult confirmationResult { get; set; }

		public PurchaseValidationResult validationResult { get; set; }

		public int numberRetries { get; set; }

		public string message { get; set; }

		public ReportPurchaseStatusCommand()
		{
		}

		public ReportPurchaseStatusCommand(string bundleIdentifier, string trackingId, Metrics.BundleSource bundleSource, PurchaseConfirmationResult confirmationResult, PurchaseValidationResult validationResult, string message)
		{
			this.bundleIdentifier = bundleIdentifier;
			this.trackingId = trackingId;
			this.bundleSource = bundleSource;
			this.confirmationResult = confirmationResult;
			this.validationResult = validationResult;
			this.message = message;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (validationResult != PurchaseValidationResult.None)
			{
				PurchaseAnalyticsHelper.SendValidationEvent(tWDModelManager, validationResult, trackingId, bundleSource);
				if (validationResult != PurchaseValidationResult.OK)
				{
					tWDModelManager.Debug.LogError("PurchaseStatus: validation error: '" + validationResult.ToString() + "', platform:" + JsonConvert.SerializeObject(tWDModelManager.Player.PcPlatform) + ", bundleId: '" + bundleIdentifier + "', message: '" + message + "', bundleSource: '" + bundleSource.ToString() + "'");
				}
			}
			else if (confirmationResult != PurchaseConfirmationResult.None)
			{
				PurchaseAnalyticsHelper.SendConfirmationEvent(tWDModelManager, confirmationResult, bundleSource);
				if (confirmationResult != PurchaseConfirmationResult.OK && confirmationResult != PurchaseConfirmationResult.Canceled && message != null && !message.Contains("Canceled by user") && !message.Contains("User canceled"))
				{
					tWDModelManager.Debug.LogError("PurchaseStatus: confirmation error: " + confirmationResult.ToString() + ", platform=" + JsonConvert.SerializeObject(tWDModelManager.Player.PcPlatform) + ", bundleId=" + bundleIdentifier + ", message=" + message);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
