using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class GoldRadioCallDenifition
	{
		public int Identifier;

		public int Type;

		public List<string> Class;

		public string TabPic;

		public string DetailText;

		public List<string> Star4Rate;

		public List<string> Star4Show;

		public List<string> Star3Rate;

		public List<string> Star3Show;

		public List<string> Star2Rate;

		public List<string> Star2Show;

		public List<string> Star1Rate;

		public List<string> Star1Show;

		public List<string> OtherRate;

		public List<string> OtherShow;

		public List<string> UPShow;
	}
}
