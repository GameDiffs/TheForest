using System;
using UnityEngine;

public class SimpleActivatorMenu : MonoBehaviour
{
	public GUIText camSwitchButton;

	public GameObject[] objects;

	private int currentActiveObject;

	private void OnEnable()
	{
		this.currentActiveObject = 0;
		this.camSwitchButton.text = this.objects[this.currentActiveObject].name;
	}

	private void Update()
	{
		if (CrossPlatformInput.GetButtonDown("NextCamera"))
		{
			int num = (this.currentActiveObject + 1 < this.objects.Length) ? (this.currentActiveObject + 1) : 0;
			for (int i = 0; i < this.objects.Length; i++)
			{
				this.objects[i].SetActive(i == num);
			}
			this.currentActiveObject = num;
			this.camSwitchButton.text = this.objects[this.currentActiveObject].name;
		}
	}
}
