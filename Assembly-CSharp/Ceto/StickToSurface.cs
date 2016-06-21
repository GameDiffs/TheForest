using System;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Buoyancy/StickToSurface")]
	public class StickToSurface : MonoBehaviour
	{
		private void Start()
		{
		}

		private void Update()
		{
			if (Ocean.Instance == null)
			{
				return;
			}
			Vector3 position = base.transform.position;
			position.y = Ocean.Instance.QueryWaves(position.x, position.z);
			base.transform.position = position;
		}
	}
}
