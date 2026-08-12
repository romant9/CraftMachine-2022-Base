using System.Collections.Generic;

namespace BaseModel.ContentTypes
{
	public sealed class MediationData : ContentTypeBase
	{
		public List<WeightData> Weights { get; set; }

		public List<CapData> Caps { get; set; }
	}
}
