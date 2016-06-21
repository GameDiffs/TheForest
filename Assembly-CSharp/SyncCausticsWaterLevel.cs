using System;
using UnityEngine;

[AddComponentMenu("Relief Terrain/Helpers/Sync Caustics Water Level"), ExecuteInEditMode]
public class SyncCausticsWaterLevel : MonoBehaviour
{
	public GameObject refGameObject;

	public float yOffset;

	private void Update()
	{
		if (this.refGameObject && this.refGameObject.GetComponent<Renderer>())
		{
			this.refGameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("TERRAIN_CausticsWaterLevel", base.transform.position.y + this.yOffset);
		}
		else
		{
			Shader.SetGlobalFloat("TERRAIN_CausticsWaterLevel", base.transform.position.y + this.yOffset);
		}
	}
}
