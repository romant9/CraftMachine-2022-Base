using System.Collections.Generic;

namespace TWDModel
{
	public class Migration7170 : TWDModelMigration
	{
		public Migration7170()
		{
			base.Version = "7.17.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			Deal7170Camp(player, manager);
			if (true)
			{
				MigrationUtils.DeleteCombatModel(player);
			}
			return true;
		}

		private static void Deal7170Camp(PlayerModel player, TWDModelManager manager)
		{
			if (player == null || player.CampMover == null || player.Camp == null || player.CampMover.BackgroundName != "A" || manager.GameEconomyData == null)
			{
				return;
			}
			CampType campType = player.CampMover.GetCampType(player.Camp.Level);
			CampSubtype campSubtype = FindBackgroundSubtype(campType, "A");
			if (campType == null || campSubtype == null)
			{
				return;
			}
			bool flag = false;
			RectData[] array = campSubtype.ValidBuildingPositions ?? new RectData[0];
			RectData[] array2 = player.CampMover.ValidBuildingPositions ?? new RectData[0];
			List<RectData> list = new List<RectData>(array2);
			for (int i = 0; i < array.Length; i++)
			{
				if (!ContainsRect(array2, array[i]))
				{
					list.Add(array[i]);
					flag = true;
				}
			}
			if (flag)
			{
				player.CampMover.ValidBuildingPositions = list.ToArray();
			}
			int num = manager.GameEconomyData.ScaleToGrid(campSubtype.Size.X);
			int num2 = manager.GameEconomyData.ScaleToGrid(campSubtype.Size.Y);
			if (player.Camp.GridWidth != num || player.Camp.GridHeight != num2)
			{
				player.Camp.SetGridSize(num, num2);
				player.Camp.UpdateGridPosition();
			}
		}

		private static CampSubtype FindBackgroundSubtype(CampType campType, string background)
		{
			if (campType?.CampSubtypes == null)
			{
				return null;
			}
			for (int i = 0; i < campType.CampSubtypes.Count; i++)
			{
				CampSubtype campSubtype = campType.CampSubtypes[i];
				if (campSubtype != null && campSubtype.Background == background)
				{
					return campSubtype;
				}
			}
			return null;
		}

		private static bool ContainsRect(RectData[] rects, RectData target)
		{
			if (rects == null || target == null)
			{
				return false;
			}
			foreach (RectData rectData in rects)
			{
				if (rectData != null && rectData.X == target.X && rectData.Y == target.Y && rectData.Width == target.Width && rectData.Height == target.Height)
				{
					return true;
				}
			}
			return false;
		}
	}
}
