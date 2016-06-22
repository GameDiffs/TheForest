using System;
using UnityEngine;

[Serializable]
public class edit_class
{
	public string text;

	public string default_text;

	public bool edit;

	public bool disable_edit;

	public Rect rect;

	public edit_class()
	{
		this.text = string.Empty;
		this.default_text = string.Empty;
	}
}
