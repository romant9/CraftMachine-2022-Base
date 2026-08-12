using UnityEngine;

public class LootCardAnimationOver : MonoBehaviour
{
	public void RevealAnimationOver()
	{
		base.transform.parent.GetComponent<LootCard>().RevealAnimationOver();
	}
}
