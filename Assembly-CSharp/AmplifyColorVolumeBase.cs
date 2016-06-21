using AmplifyColor;
using System;
using UnityEngine;

[AddComponentMenu("")]
public class AmplifyColorVolumeBase : MonoBehaviour
{
	public Texture2D LutTexture;

	public float EnterBlendTime = 1f;

	public int Priority;

	public bool ShowInSceneView = true;

	[HideInInspector]
	public VolumeEffectContainer EffectContainer = new VolumeEffectContainer();

	private void OnDrawGizmos()
	{
		if (this.ShowInSceneView)
		{
			BoxCollider component = base.GetComponent<BoxCollider>();
			if (component != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawIcon(base.transform.position, "lut-volume.png", true);
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawWireCube(component.center, component.size);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		if (component != null)
		{
			Color green = Color.green;
			green.a = 0.2f;
			Gizmos.color = green;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawCube(component.center, component.size);
		}
	}
}
