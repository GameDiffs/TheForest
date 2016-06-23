using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class livingBirdsDemoScript : MonoBehaviour
{
	public lb_BirdController birdControl;

	public Camera camera1;

	public Camera camera2;

	private bool cameraDirections = true;

	private void Start()
	{
		this.birdControl = GameObject.Find("_livingBirdsController").GetComponent<lb_BirdController>();
		this.SpawnSomeBirds();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			this.camera1.transform.localEulerAngles += new Vector3(0f, 90f, 0f) * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			this.camera1.transform.localEulerAngles -= new Vector3(0f, 90f, 0f) * Time.deltaTime;
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 10f, 150f, 50f), "Pause"))
		{
			this.birdControl.SendMessage("Pause");
		}
		if (GUI.Button(new Rect(10f, 70f, 150f, 30f), "Scare All"))
		{
			this.birdControl.SendMessage("AllFlee");
		}
		if (GUI.Button(new Rect(10f, 110f, 150f, 50f), "Change Camera"))
		{
			this.ChangeCamera();
		}
		if (this.cameraDirections)
		{
			GUI.Label(new Rect(170f, 10f, 1014f, 20f), "USE ARROW KEYS TO PAN THE CAMERA");
		}
	}

	[DebuggerHidden]
	private IEnumerator SpawnSomeBirds()
	{
		livingBirdsDemoScript.<SpawnSomeBirds>c__Iterator218 <SpawnSomeBirds>c__Iterator = new livingBirdsDemoScript.<SpawnSomeBirds>c__Iterator218();
		<SpawnSomeBirds>c__Iterator.<>f__this = this;
		return <SpawnSomeBirds>c__Iterator;
	}

	private void ChangeCamera()
	{
		if (this.camera2.gameObject.activeSelf)
		{
			this.camera1.gameObject.SetActive(true);
			this.camera2.gameObject.SetActive(false);
			this.birdControl.SendMessage("ChangeCamera", this.camera1);
			this.cameraDirections = true;
		}
		else
		{
			this.camera1.gameObject.SetActive(false);
			this.camera2.gameObject.SetActive(true);
			this.birdControl.SendMessage("ChangeCamera", this.camera2);
			this.cameraDirections = false;
		}
	}
}
