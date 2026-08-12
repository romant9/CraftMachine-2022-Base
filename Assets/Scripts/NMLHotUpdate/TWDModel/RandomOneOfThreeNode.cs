using System;

namespace TWDModel
{
	[Serializable]
	public class RandomOneOfThreeNode : NodeBase
	{
		[GraphItExportData("Last Random Value", "")]
		public int LastRandomNumber { get; set; }

		public RandomOneOfThreeNode()
		{
		}

		public RandomOneOfThreeNode(RandomOneOfThreeNode node)
			: base(node)
		{
			LastRandomNumber = node.LastRandomNumber;
		}

		public override NodeBase RecordValue()
		{
			return new RandomOneOfThreeNode(this);
		}

		[GraphItInput("In", "")]
		public void In()
		{
			LastRandomNumber = base.manager.Player.PlayerRandom.GetRandomInRange(1, 100);
			if (LastRandomNumber <= 33)
			{
				First();
			}
			else if (LastRandomNumber >= 66)
			{
				Second();
			}
			else
			{
				Third();
			}
		}

		[GraphItOutput("First", "")]
		public void First()
		{
			Fire("First");
		}

		[GraphItOutput("Second", "")]
		public void Second()
		{
			Fire("Second");
		}

		[GraphItOutput("Third", "")]
		public void Third()
		{
			Fire("Third");
		}
	}
}
