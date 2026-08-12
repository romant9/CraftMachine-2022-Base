using System;

namespace TWDModel
{
	[Serializable]
	public class RandomChanceNode : NodeBase
	{
		[GraphItVariable("Random chance between 0 and 100.")]
		public int RandomChance = 50;

		[GraphItExportData("Last Random Value", "")]
		public int LastRandomNumber { get; set; }

		public RandomChanceNode()
		{
		}

		public RandomChanceNode(RandomChanceNode node)
			: base(node)
		{
			RandomChance = node.RandomChance;
			LastRandomNumber = node.LastRandomNumber;
		}

		public override NodeBase RecordValue()
		{
			return new RandomChanceNode(this);
		}

		[GraphItInput("In", "")]
		public void In()
		{
			LastRandomNumber = base.manager.Player.PlayerRandom.GetRandomInRange(1, 100);
			if (LastRandomNumber <= RandomChance)
			{
				Success();
			}
			else
			{
				Fail();
			}
		}

		[GraphItOutput("Success", "")]
		public void Success()
		{
			Fire("Success");
		}

		[GraphItOutput("Fail", "")]
		public void Fail()
		{
			Fire("Fail");
		}
	}
}
