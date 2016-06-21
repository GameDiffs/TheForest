using System;
using UnityEngine;

public class addToPlayerTargets : MonoBehaviour
{
	private sceneTracker scene;

	private void Start()
	{
		this.scene = GameObject.FindWithTag("Ai").GetComponent<sceneTracker>();
		if (!this.scene.allPlayers.Contains(base.gameObject))
		{
			this.scene.allPlayers.Add(base.gameObject);
		}
	}
}
