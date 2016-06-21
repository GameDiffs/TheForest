using System;
using UnityEngine;

public class turtleAnimEvents : MonoBehaviour
{
	private animalAI ai;

	private void Start()
	{
		this.ai = base.GetComponent<animalAI>();
	}

	private void enableTurning()
	{
		this.ai.fsmRotateSpeed.Value = 0.5f;
	}

	private void disableTurning()
	{
		this.ai.fsmRotateSpeed.Value = 0f;
	}
}
