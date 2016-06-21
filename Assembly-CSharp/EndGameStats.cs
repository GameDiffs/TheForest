using System;
using TheForest.Utils;
using UnityEngine;

public class EndGameStats : MonoBehaviour
{
	private UILabel MyLabel;

	private void Awake()
	{
		this.MyLabel = base.gameObject.GetComponent<UILabel>();
	}

	private void Update()
	{
		this.MyLabel.text = "DAYS SURVIVED  " + Mathf.FloorToInt(LocalPlayer.Stats.DaySurvived);
	}
}
