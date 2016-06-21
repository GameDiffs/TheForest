using System;
using UnityEngine;

public class PlayerCamLocation : MonoBehaviour
{
	public static Vector3 PlayerLoc;

	private void Awake()
	{
		this.Update();
	}

	private void Update()
	{
		PlayerCamLocation.PlayerLoc = base.transform.position;
	}
}
