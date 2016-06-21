using System;
using UnityEngine;

namespace TheForest.World
{
	public class RefreshMossSpread : MonoBehaviour
	{
		private void OnEnable()
		{
			base.GetComponent<Renderer>().sharedMaterial.SetFloat("_MossSpread", Mathf.Clamp01((float)Clock.Day / 10f) * 0.7f + 0.3f);
		}
	}
}
