using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class CaveTriggers : MonoBehaviour
{
	public bool IsEntryControlled;

	public bool IsOutsideArea;

	public List<Transform> playersEnteringTrigger = new List<Transform>();

	public List<Transform> playersExitingTrigger = new List<Transform>();

	private void Awake()
	{
		base.enabled = false;
	}

	private void Update()
	{
		for (int i = this.playersEnteringTrigger.Count - 1; i >= 0; i--)
		{
			Transform transform = this.playersEnteringTrigger[i];
			Vector3 position = base.transform.InverseTransformPoint(transform.position);
			position.x = 0f;
			position.y = 0f;
			Vector3 a = base.transform.TransformPoint(position);
			float num = Vector3.Distance(a, base.transform.position);
			if (num < 4.9f)
			{
				this.playersEnteringTrigger.RemoveAt(i);
				this.playersExitingTrigger.Add(transform);
				if (!this.IsOutsideArea)
				{
					if (position.z < 0f)
					{
						transform.gameObject.SendMessage("InACave", SendMessageOptions.DontRequireReceiver);
					}
				}
				else if (transform == LocalPlayer.Transform)
				{
					LocalPlayer.Stats.IgnoreCollisionWithTerrain(true);
				}
			}
			else if (Vector3.Distance(transform.position, base.transform.position) > 7f)
			{
				this.playersEnteringTrigger.RemoveAt(i);
			}
		}
		for (int j = this.playersExitingTrigger.Count - 1; j >= 0; j--)
		{
			Transform transform2 = this.playersExitingTrigger[j];
			Vector3 position2 = base.transform.InverseTransformPoint(transform2.position);
			position2.x = 0f;
			position2.y = 0f;
			Vector3 a2 = base.transform.TransformPoint(position2);
			float num2 = Vector3.Distance(a2, base.transform.position);
			if (num2 > 6.48f)
			{
				this.playersExitingTrigger.RemoveAt(j);
				if (!this.IsOutsideArea && position2.z < 0f)
				{
					transform2.SendMessage("NotInACave", SendMessageOptions.DontRequireReceiver);
				}
				if (transform2 == LocalPlayer.Transform)
				{
					LocalPlayer.Stats.IgnoreCollisionWithTerrain(false);
				}
			}
		}
		if (this.playersEnteringTrigger.Count == 0 && this.playersExitingTrigger.Count == 0)
		{
			base.enabled = false;
		}
		CaveTriggers.CheckPlayersInCave();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.IsEntryControlled && other.gameObject.tag.StartsWith("Player") && !this.playersEnteringTrigger.Contains(other.transform) && !this.playersExitingTrigger.Contains(other.transform))
		{
			base.transform.InverseTransformPoint(other.transform.position).y = 0f;
			base.enabled = true;
			this.playersEnteringTrigger.Add(other.transform);
		}
		if (other.gameObject.CompareTag("PlayerNet") && !this.playersEnteringTrigger.Contains(other.transform) && !this.playersExitingTrigger.Contains(other.transform))
		{
			base.transform.InverseTransformPoint(other.transform.position).y = 0f;
			base.enabled = true;
			this.playersEnteringTrigger.Add(other.transform);
		}
		if (this.IsOutsideArea && other.gameObject.CompareTag("Multisled"))
		{
			TerrainCollider terrainCollider = null;
			if (Terrain.activeTerrain)
			{
				terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
			}
			if (!terrainCollider)
			{
				return;
			}
			Physics.IgnoreCollision(terrainCollider, other, true);
		}
	}

	[DebuggerHidden]
	private IEnumerator CaveDoorRoutine()
	{
		CaveTriggers.<CaveDoorRoutine>c__Iterator15D <CaveDoorRoutine>c__Iterator15D = new CaveTriggers.<CaveDoorRoutine>c__Iterator15D();
		<CaveDoorRoutine>c__Iterator15D.<>f__this = this;
		return <CaveDoorRoutine>c__Iterator15D;
	}

	public static void CheckPlayersInCave()
	{
		if (Scene.CaveGrounds != null)
		{
			bool flag = Scene.SceneTracker.allPlayersInCave.Count > 0;
			for (int i = 0; i < Scene.CaveGrounds.Length; i++)
			{
				GameObject gameObject = Scene.CaveGrounds[i];
				if (gameObject && gameObject.activeSelf != flag)
				{
					gameObject.SetActive(flag);
				}
			}
		}
	}
}
