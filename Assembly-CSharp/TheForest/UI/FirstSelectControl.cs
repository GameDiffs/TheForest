using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI
{
	public class FirstSelectControl : MonoBehaviour
	{
		private void Update()
		{
			if ((UICamera.hoveredObject == null || !NGUITools.GetActive(UICamera.hoveredObject)) && TheForest.Utils.Input.GetAxis("Vertical") < -0.5f)
			{
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.hoveredObject = base.gameObject;
			}
		}
	}
}
