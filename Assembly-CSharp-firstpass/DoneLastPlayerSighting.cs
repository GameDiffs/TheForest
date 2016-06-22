using System;
using UnityEngine;

public class DoneLastPlayerSighting : MonoBehaviour
{
	public static bool PlayerLight;

	public int TotalCount;

	public bool Morning;

	public Vector3 position = new Vector3(1000f, 1000f, 1000f);

	public Vector3 HomePosition = new Vector3(1000f, 1000f, 1000f);

	public void PlayerHasLight()
	{
		DoneLastPlayerSighting.PlayerLight = true;
	}

	public void Awake()
	{
	}

	public void WentLight()
	{
		this.Morning = true;
	}

	public void WentDark()
	{
		this.Morning = false;
	}

	public void PlayerHasNoLight()
	{
		DoneLastPlayerSighting.PlayerLight = false;
	}

	public void AddedEnemy()
	{
		this.TotalCount++;
	}

	public void KilledEnemy()
	{
		this.TotalCount--;
	}
}
