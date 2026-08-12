using UnityEngine;
using UnityEngine.Playables;

public class MeshRendererShaderMixer : PlayableBehaviour
{
	public string ShaderVarName;

	public ShaderControlType VariableType;

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		Renderer renderer = playerData as Renderer;
		if (renderer == null)
		{
			return;
		}
		float num = 0f;
		Vector4 zero = Vector4.zero;
		Color black = Color.black;
		int inputCount = playable.GetInputCount();
		float num2 = 0f;
		for (int i = 0; i < inputCount; i++)
		{
			float inputWeight = playable.GetInputWeight(i);
			ShaderPlayable behaviour = ((ScriptPlayable<ShaderPlayable>)playable.GetInput(i)).GetBehaviour();
			num += behaviour.FloatVal * inputWeight;
			zero += behaviour.VectorVal * inputWeight;
			black += behaviour.ColorVal * inputWeight;
			num2 += inputWeight;
		}
		if (!(num2 < 0.5f))
		{
			Material material = ((!Application.isPlaying) ? renderer.sharedMaterial : renderer.material);
			switch (VariableType)
			{
			case ShaderControlType.SetFloat:
				material.SetFloat(ShaderVarName, num);
				break;
			case ShaderControlType.SetVector:
				material.SetVector(ShaderVarName, zero);
				break;
			case ShaderControlType.SetColor:
				material.SetColor(ShaderVarName, black);
				break;
			}
		}
	}
}
