using System;
using UnityEngine;

public class GetSliderValue : MonoBehaviour
{
	private UILabel lbl;

	public string addChar = "%";

	private void Awake()
	{
		this.lbl = base.gameObject.GetComponent<UILabel>();
	}

	private void OnSliderChange(float val)
	{
		int num = (int)Mathf.Round(val * 100f);
		this.lbl.text = num.ToString() + this.addChar;
	}
}
