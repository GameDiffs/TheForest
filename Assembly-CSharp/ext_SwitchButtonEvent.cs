using System;
using UnityEngine;

public class ext_SwitchButtonEvent : MonoBehaviour
{
	public float xOffset = 25f;

	[HideInInspector]
	public bool state;

	[HideInInspector]
	public TweenPosition tweenPos;

	private void Start()
	{
		this.tweenPos = base.gameObject.GetComponent<TweenPosition>();
		this.tweenPos.ResetToBeginning();
	}

	private void OnClick()
	{
		this.state = !this.state;
		this.tweenPos.Play(this.state);
		base.transform.parent.gameObject.GetComponent<ext_Switch>().state = this.state;
	}

	public void SetState(bool s)
	{
		this.state = s;
		this.tweenPos.Play(this.state);
	}
}
