using System;
using UnityEngine;

[ExecuteInEditMode]
public class ext_Switch : MonoBehaviour
{
	public string leftText = "Off";

	public string rightText = "On";

	public bool state;

	public UILabel lbl_left;

	public UILabel lbl_right;

	public UISprite thumb;

	private void Start()
	{
		this.thumb = base.gameObject.GetComponentInChildren<UISprite>();
		this.thumb.gameObject.GetComponent<ext_SwitchButtonEvent>().SetState(this.state);
	}

	private void Update()
	{
	}
}
