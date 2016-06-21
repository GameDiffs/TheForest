using System;
using TheForest.Utils;
using UnityEngine;

public class CaveDoor : MonoBehaviour
{
	private GameObject Ocean;

	private Collider terrain;

	private void Awake()
	{
		this.Ocean = GameObject.FindWithTag("Ocean");
		this.terrain = GameObject.FindWithTag("TerrainMain").GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.Ocean.SetActive(false);
			Physics.IgnoreCollision(other, this.terrain, true);
			LocalPlayer.Stats.InACave();
			LocalPlayer.AiInfo.InACave();
		}
		else if (other.gameObject.CompareTag("PlayerNet"))
		{
			other.SendMessage("InACave");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			if (base.transform.InverseTransformPoint(other.transform.position).z < 0f)
			{
				this.Ocean.SetActive(true);
				if (Terrain.activeTerrain.GetComponent<Collider>().enabled)
				{
					Collider[] components = other.GetComponents<Collider>();
					for (int i = 0; i < components.Length; i++)
					{
						Collider collider = components[i];
						if (collider.enabled)
						{
							Physics.IgnoreCollision(other, this.terrain, false);
						}
					}
				}
				Physics.IgnoreCollision(other, this.terrain, false);
				LocalPlayer.Stats.NotInACave();
				LocalPlayer.AiInfo.NotInACave();
			}
		}
		else if (other.gameObject.CompareTag("PlayerNet"))
		{
			other.SendMessage("NotInACave");
		}
	}
}
