using UnityEngine;
using UnityEngine.Playables;

public class UIShaderMixer : PlayableBehaviour
{
	public string ShaderVarName;

	public ShaderControlType VariableType;

	private float finalFloat;

	private Vector4 finalVector4 = Vector4.zero;

	private Color finalColor = Color.black;

	private UIWidget uiWidget;

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		uiWidget = playerData as UIWidget;
		if (!(uiWidget == null))
		{
			int inputCount = playable.GetInputCount();
			float num = 0f;
			finalColor = Color.clear;
			finalVector4 = Vector4.zero;
			finalFloat = 0f;
			for (int i = 0; i < inputCount; i++)
			{
				float inputWeight = playable.GetInputWeight(i);
				ShaderPlayable behaviour = ((ScriptPlayable<ShaderPlayable>)playable.GetInput(i)).GetBehaviour();
				finalFloat += behaviour.FloatVal * inputWeight;
				finalVector4 += behaviour.VectorVal * inputWeight;
				finalColor += behaviour.ColorVal * inputWeight;
				num += inputWeight;
			}
			if (!(num < 0.5f))
			{
				uiWidget.onRender = OnRenderWidget;
			}
		}
	}

	private void OnRenderWidget(Material material)
	{
		switch (VariableType)
		{
		case ShaderControlType.SetFloat:
			material.SetFloat(ShaderVarName, finalFloat);
			break;
		case ShaderControlType.SetVector:
			material.SetVector(ShaderVarName, finalVector4);
			break;
		case ShaderControlType.SetColor:
			if (string.IsNullOrEmpty(ShaderVarName))
			{
				uiWidget.color = finalColor;
			}
			else
			{
				material.SetColor(ShaderVarName, finalColor);
			}
			break;
		}
	}
}
