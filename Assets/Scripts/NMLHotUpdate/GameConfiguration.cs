using UnityEngine;

public class GameConfiguration
{
	private static GameConfiguration _gameConfiguration;

	private BuildGameConfiguration _buildGameConfiguration;

	public BuildGameConfiguration Config => _buildGameConfiguration;

	public static GameConfiguration Instance
	{
		get
		{
			_gameConfiguration ??= new GameConfiguration();
			return _gameConfiguration;
		}
	}

	public GameConfiguration()
	{
		string text = "BuildClientConfiguration";
		if (Application.identifier.Contains("-lv"))
		{
			text += "Korea";
		}
		_buildGameConfiguration = UnityUtils.LoadFromAssetBundle<BuildGameConfiguration>(text, "scriptableobjects");
		if (_buildGameConfiguration == null)
		{
			Debug.LogError("Game configuration was not loaded");
		}
	}

	public void Save()
	{
	}
}
