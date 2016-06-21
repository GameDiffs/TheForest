using System;
using TheForest.Utils;
using UnityEngine;

public class jumpObject : MonoBehaviour
{
	private sceneTracker sceneInfo;

	private Transform thisTr;

	public Transform jumpPos;

	public bool occupied;

	private void Awake()
	{
		this.sceneInfo = Scene.SceneTracker;
		this.thisTr = base.transform;
	}

	private void OnDeserialized()
	{
		this.sceneInfo.addToJump(this.thisTr);
		this.occupied = false;
	}

	private void Start()
	{
		this.sceneInfo.addToJump(this.thisTr);
		this.occupied = false;
	}

	private void OnEnable()
	{
		this.sceneInfo.addToJump(this.thisTr);
		this.occupied = false;
	}

	private void OnDisable()
	{
		this.sceneInfo.removeFromJump(this.thisTr);
	}
}
