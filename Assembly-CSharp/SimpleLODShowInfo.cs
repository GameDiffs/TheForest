using System;
using UnityEngine;

public class SimpleLODShowInfo : MonoBehaviour
{
	public LODSwitcher lodSwitcher;

	public float offsetY = 2f;

	private void OnGUI()
	{
		if (this.lodSwitcher != null)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position + new Vector3(0f, this.offsetY, 0f));
			if (vector.z > 0f)
			{
				GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
				gUIStyle.normal.textColor = Color.black;
				gUIStyle.alignment = TextAnchor.LowerCenter;
				GUI.Label(new Rect(vector.x - 50f, (float)Screen.height - vector.y - 20f, 100f, 20f), "LOD " + this.lodSwitcher.GetLODLevel(), gUIStyle);
			}
		}
	}
}
