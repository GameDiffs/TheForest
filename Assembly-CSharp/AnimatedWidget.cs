using System;
using UnityEngine;

[ExecuteInEditMode]
public class AnimatedWidget : MonoBehaviour
{
	public float width = 1f;

	public float height = 1f;

	private UIWidget mWidget;

	private void OnEnable()
	{
		this.mWidget = base.GetComponent<UIWidget>();
		this.LateUpdate();
	}

	private void LateUpdate()
	{
		if (this.mWidget != null)
		{
			this.mWidget.width = Mathf.RoundToInt(this.width);
			this.mWidget.height = Mathf.RoundToInt(this.height);
		}
	}
}
