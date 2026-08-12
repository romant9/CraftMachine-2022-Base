namespace Client.Constants
{
	public class Layers
	{
		public const int Ui = 5;

		public const int UiTopCamera = 20;

		public const int UIPerspective = 23;

		public const int LoadingScreenCombat = 18;

		public const int LootBox = 19;

		public static int GetUILayerMask()
		{
			return 9437216;
		}

		public static bool LayerFoundInMask(int layer, int mask)
		{
			return mask == (mask | (1 << layer));
		}
	}
}
