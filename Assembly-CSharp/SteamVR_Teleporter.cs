using System;
using UnityEngine;

public class SteamVR_Teleporter : MonoBehaviour
{
	public enum TeleportType
	{
		TeleportTypeUseTerrain,
		TeleportTypeUseCollider,
		TeleportTypeUseZeroY
	}

	public bool teleportOnClick;

	public SteamVR_Teleporter.TeleportType teleportType = SteamVR_Teleporter.TeleportType.TeleportTypeUseZeroY;

	private Transform reference;

	private void Start()
	{
		Transform component = UnityEngine.Object.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
		this.reference = component.parent.parent;
		if (base.GetComponent<SteamVR_TrackedController>() == null)
		{
			Debug.LogError("SteamVR_Teleporter must be on a SteamVR_TrackedController");
			return;
		}
		base.GetComponent<SteamVR_TrackedController>().TriggerClicked += new ClickedEventHandler(this.DoClick);
		if (this.teleportType == SteamVR_Teleporter.TeleportType.TeleportTypeUseTerrain)
		{
			this.reference.position = new Vector3(this.reference.position.x, Terrain.activeTerrain.SampleHeight(this.reference.position), this.reference.position.z);
		}
	}

	private void Update()
	{
	}

	private void DoClick(object sender, ClickedEventArgs e)
	{
		if (this.teleportOnClick)
		{
			float y = this.reference.position.y;
			Plane plane = new Plane(Vector3.up, -y);
			Ray ray = new Ray(base.transform.position, base.transform.forward);
			bool flag = false;
			float d = 0f;
			if (this.teleportType == SteamVR_Teleporter.TeleportType.TeleportTypeUseCollider)
			{
				TerrainCollider component = Terrain.activeTerrain.GetComponent<TerrainCollider>();
				RaycastHit raycastHit;
				flag = component.Raycast(ray, out raycastHit, 1000f);
				d = raycastHit.distance;
			}
			else if (this.teleportType == SteamVR_Teleporter.TeleportType.TeleportTypeUseCollider)
			{
				RaycastHit raycastHit2;
				Physics.Raycast(ray, out raycastHit2);
				d = raycastHit2.distance;
			}
			else
			{
				flag = plane.Raycast(ray, out d);
			}
			if (flag)
			{
				Vector3 position = ray.origin + ray.direction * d - new Vector3(this.reference.GetChild(0).localPosition.x, 0f, this.reference.GetChild(0).localPosition.z);
				this.reference.position = position;
			}
		}
	}
}
