using System;
using TheForest.Utils;
using UnityEngine;

public class CoopTrapGUIDisable : MonoBehaviour
{
	private void OnDestroy()
	{
		if (BoltNetwork.isRunning && Scene.HudGui)
		{
			Scene.HudGui.AddIcon.SetActive(false);
		}
	}
}
