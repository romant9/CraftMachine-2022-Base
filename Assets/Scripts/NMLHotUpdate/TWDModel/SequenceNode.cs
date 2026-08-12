using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class SequenceNode : NodeBase
	{
		private const int OutputCount = 5;

		[GraphItVariable("Shuffle output order.")]
		public bool Shuffle;

		public List<string> OutputOrder;

		public int CurrentIndex;

		public SequenceNode()
		{
		}

		public SequenceNode(SequenceNode node)
			: base(node)
		{
			Shuffle = node.Shuffle;
			CurrentIndex = node.CurrentIndex;
			OutputOrder = ((node.OutputOrder == null) ? null : new List<string>(node.OutputOrder));
		}

		public override NodeBase RecordValue()
		{
			return new SequenceNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			CurrentIndex = 0;
			OutputOrder = null;
		}

		private void StartNewSequence()
		{
			if (OutputOrder == null)
			{
				OutputOrder = new List<string>();
			}
			else
			{
				OutputOrder.Clear();
			}
			for (int i = 0; i < 5; i++)
			{
				string text = "Action " + (i + 1);
				if (HasConnections(text))
				{
					OutputOrder.Add(text);
				}
			}
			if (Shuffle)
			{
				UtilsArray.ShuffleList(OutputOrder, base.manager.Player.PlayerRandom);
			}
			CurrentIndex = 0;
		}

		[GraphItInput("Run All", "")]
		public void RunAll()
		{
			StartNewSequence();
			for (int i = 0; i < OutputOrder.Count; i++)
			{
				Next();
			}
			Completed();
		}

		[GraphItInput("Next", "")]
		public void Next()
		{
			if (OutputOrder == null || CurrentIndex < 0)
			{
				StartNewSequence();
			}
			if (OutputOrder != null && CurrentIndex >= 0)
			{
				if (CurrentIndex < OutputOrder.Count)
				{
					Fire(OutputOrder[CurrentIndex]);
					CurrentIndex++;
				}
				if (CurrentIndex >= OutputOrder.Count)
				{
					Completed();
				}
			}
		}

		[GraphItOutput("Action 1", "")]
		public void Action_1()
		{
			Fire("Action 1");
		}

		[GraphItOutput("Action 2", "")]
		public void Action_2()
		{
			Fire("Action 2");
		}

		[GraphItOutput("Action 3", "")]
		public void Action_3()
		{
			Fire("Action 3");
		}

		[GraphItOutput("Action 4", "")]
		public void Action_4()
		{
			Fire("Action 4");
		}

		[GraphItOutput("Action 5", "")]
		public void Action_5()
		{
			Fire("Action 5");
		}

		[GraphItOutput("Completed", "")]
		public void Completed()
		{
			CurrentIndex = -1;
			Fire("Completed");
		}
	}
}
