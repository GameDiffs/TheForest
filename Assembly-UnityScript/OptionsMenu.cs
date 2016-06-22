using System;
using UnityEngine;

[Serializable]
public class OptionsMenu : MonoBehaviour
{
	private bool Open;

	public GameObject OptionsMenu;

	public GameObject PlayerCam;

	public GameObject Player;

	public override void Update()
	{
		if (Input.GetButtonDown("Esc") && !this.Open)
		{
			Time.timeScale = (float)0;
			this.PlayerCam.SetActive(false);
			this.Player.SendMessage("LockView");
			this.OptionsMenu.SetActive(true);
			this.Open = true;
		}
		else if (Input.GetButtonDown("Esc") && this.Open)
		{
			Time.timeScale = (float)1;
			this.PlayerCam.SetActive(true);
			this.Player.SendMessage("UnLockView");
			this.OptionsMenu.SetActive(false);
			this.Open = false;
		}
	}

	public override void Main()
	{
	}
}
