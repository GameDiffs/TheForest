using System;
using TheForest.Utils;
using UnityEngine;

public class CameraMouseEvents : MonoBehaviour
{
	public LayerMask layerMask = -1;

	private GameObject selectedObject;

	private bool isMouseDown;

	private void Awake()
	{
		if (!base.GetComponent<Camera>())
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Update()
	{
		RaycastHit raycastHit = default(RaycastHit);
		Ray ray = base.GetComponent<Camera>().ScreenPointToRay(TheForest.Utils.Input.mousePosition);
		GameObject gameObject = (!Physics.Raycast(ray, out raycastHit, 100f, this.layerMask)) ? null : raycastHit.collider.gameObject;
		if (gameObject != this.selectedObject)
		{
			if (this.selectedObject)
			{
				if (this.isMouseDown)
				{
					this.selectedObject.SendMessage("OnMouseUpCollider", SendMessageOptions.DontRequireReceiver);
					this.isMouseDown = false;
				}
				this.selectedObject.SendMessage("OnMouseExitCollider", SendMessageOptions.DontRequireReceiver);
			}
			if (gameObject)
			{
				gameObject.SendMessage("OnMouseEnterCollider", SendMessageOptions.DontRequireReceiver);
			}
			this.selectedObject = gameObject;
		}
		if (this.selectedObject)
		{
			this.selectedObject.SendMessage("OnMouseOverCollider", SendMessageOptions.DontRequireReceiver);
			if (TheForest.Utils.Input.GetButtonDown("Fire1"))
			{
				this.selectedObject.SendMessage("OnMouseDownCollider", SendMessageOptions.DontRequireReceiver);
				this.isMouseDown = true;
			}
			if (TheForest.Utils.Input.GetButtonUp("Fire1"))
			{
				this.selectedObject.SendMessage("OnMouseUpCollider", SendMessageOptions.DontRequireReceiver);
				this.isMouseDown = false;
			}
		}
	}
}
