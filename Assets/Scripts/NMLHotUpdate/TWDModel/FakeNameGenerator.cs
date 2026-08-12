using BaseModel;

namespace TWDModel
{
	public static class FakeNameGenerator
	{
		private static readonly string[] firstNames = new string[61]
		{
			"Odin", "Ninja", "Cathey", "Cheezy", "Doof", "Pops", "Eetu", "MC", "MrT", "Foobar",
			"Negan", "Arttu", "Mika", "Jussi", "Joonas", "MiCky", "Peter", "Caio", "Janne", "Mikko",
			"Daniel", "Mixu", "Juha", "Chris", "Esa", "Mitchell", "Toni", "Zoe", "Carlo", "Jan",
			"Ralf", "Joakim", "Pascal", "Nikolina", "Rik", "Ana", "Rebecca", "Anis", "Rebecca", "James",
			"Eliza", "Sylvain", "Jarmo", "Agata", "Michonne", "Daryl", "Merle", "Rick", "Judith", "Andrea",
			"Milton", "Sophia", "Beth", "Lori", "Shane", "Carl", "Karl", "Dale", "Glenn", "Amy",
			"Governor"
		};

		private static readonly string[] secondNames = new string[26]
		{
			" The Hammer", " Hunter", " Wolf", "san", " Shooter", " LEET", " Alt", " Bones", " Destroyer", " Scout",
			" UrMum", " Creamer", " Tha Killer", " Walker", "_", "B00", " GunFreak", " Sniper", " RAT", " Colt",
			" Blood", " Knife", " Grimes", " Dickson", " The Governor", " Greene"
		};

		public static string GetFakeName(PlayerModel player)
		{
			return GetFakeName(player.manager, player.Name);
		}

		public static string GetFakeName(TWDModelManager manager, string playerId)
		{
			ModelRandom modelRandom = new ModelRandom((int)ModelHelpers.MD5SumLong(playerId));
			_ = manager.GameEconomyData;
			string text = "";
			if (modelRandom.Next() < 0.4f)
			{
				int randomInRange = modelRandom.GetRandomInRange(0, SurvivorNames.FemaleNames.Length - 1);
				text = SurvivorNames.FemaleNames[randomInRange];
			}
			else if (modelRandom.Next() < 0.8f)
			{
				int randomInRange2 = modelRandom.GetRandomInRange(0, SurvivorNames.MaleNames.Length - 1);
				text = SurvivorNames.MaleNames[randomInRange2];
			}
			else
			{
				int randomInRange3 = modelRandom.GetRandomInRange(0, firstNames.Length - 1);
				text = firstNames[randomInRange3];
			}
			if (modelRandom.Next() < 0.06f)
			{
				int randomInRange4 = modelRandom.GetRandomInRange(0, secondNames.Length - 1);
				text += secondNames[randomInRange4];
				if (modelRandom.Next() < 0.3f)
				{
					text = text.Replace(' ', '_');
				}
			}
			if (modelRandom.Next() < 0.3f)
			{
				text = Leetify(modelRandom, text);
			}
			if (modelRandom.Next() < 0.06f)
			{
				text = text.ToUpperInvariant();
			}
			if (modelRandom.Next() < 0.15f)
			{
				text = text.ToLowerInvariant();
			}
			if (modelRandom.Next() < 0.1f)
			{
				text = text.Replace('s', 'z');
			}
			if (modelRandom.Next() < 0.1f)
			{
				text += modelRandom.GetRandomInRange(0, 20);
			}
			if (modelRandom.Next() < 0.03f)
			{
				text = text.Insert(0, modelRandom.GetRandomInRange(0, 20).ToString());
			}
			return text;
		}

		private static string Leetify(ModelRandom random, string name)
		{
			if (random.Next() < 0.3f)
			{
				name = name.Replace('o', '0');
			}
			if (random.Next() < 0.3f)
			{
				name = name.Replace('i', '1');
			}
			if (random.Next() < 0.3f)
			{
				name = name.Replace('l', '1');
			}
			if (random.Next() < 0.3f)
			{
				name = name.Replace('s', '5');
			}
			if (random.Next() < 0.05f)
			{
				name = name.Replace('z', '2');
			}
			if (random.Next() < 0.05f)
			{
				name = name.Replace('e', '3');
			}
			if (random.Next() < 0.05f)
			{
				name = name.Replace('t', '7');
			}
			if (random.Next() < 0.05f)
			{
				name = name.Insert(0, "Da");
			}
			if (random.Next() < 0.1f)
			{
				name = name.Insert(0, "X");
			}
			if (random.Next() < 0.1f)
			{
				name = name.Insert(0, "_");
				name += "_";
			}
			return name;
		}
	}
}
