using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public class DisableGameObject : MonoBehaviour
	{
		private void Update()
		{
			base.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			base.gameObject.SetActive(false);
		}
	}
}
