using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(UIWidget))]
public class AnimatedColor : MonoBehaviour
{
	public Color color = Color.white;

	private UIWidget mWidget;

	private void OnEnable()
	{
		this.mWidget = base.GetComponent<UIWidget>();
		this.LateUpdate();
	}

	private void LateUpdate()
	{
		this.mWidget.color = this.color;
	}
}
