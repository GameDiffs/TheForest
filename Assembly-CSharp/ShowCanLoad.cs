using System;
using UnityEngine;

public class ShowCanLoad : MonoBehaviour
{
	public Color UsableColor;

	public Color DarkColor;

	private UILabel MyLabel;

	private void Start()
	{
		this.MyLabel = base.gameObject.GetComponent<UILabel>();
	}

	private void update()
	{
		if (LevelSerializer.CanResume)
		{
			this.MyLabel.color = this.UsableColor;
		}
		else
		{
			this.MyLabel.color = this.DarkColor;
		}
	}
}
