using System;
using UnityEngine;

public class PlayerCamAheadLocation : MonoBehaviour
{
	public static Vector3 PlayerLoc;

	private void Awake()
	{
		this.Update();
	}

	private void Update()
	{
		PlayerCamAheadLocation.PlayerLoc = base.transform.position;
	}
}
