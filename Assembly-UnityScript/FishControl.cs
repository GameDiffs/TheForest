using System;
using UnityEngine;

[Serializable]
public class FishControl : MonoBehaviour
{
	[NonSerialized]
	public static bool CanBeCaught = true;

	public override void GotCaught()
	{
		FishControl.CanBeCaught = false;
		this.Invoke("ResetCaught", (float)30);
	}

	public override void ResetCaught()
	{
		FishControl.CanBeCaught = true;
	}

	public override void Main()
	{
	}
}
