using System;
using UnityEngine;

[Serializable]
public class SurvivalBookSwitchPage : MonoBehaviour
{
	public GameObject MyPage;

	public GameObject Pages;

	public bool Tab;

	public GameObject Index;

	public bool IsIndex;

	public Color SwitchColor;

	public GameObject MyArrow;

	private Material MyMat;

	public override void Awake()
	{
		this.MyMat = this.gameObject.GetComponent<Renderer>().material;
		this.SwitchColor = this.MyMat.color;
		this.SwitchColor.a = (float)0;
		this.gameObject.GetComponent<Renderer>().material.color = this.SwitchColor;
	}

	public override bool IsOverCollider()
	{
		bool result = false;
		int i = 0;
		Camera[] allCameras = Camera.allCameras;
		int length = allCameras.Length;
		while (i < length)
		{
			Ray ray = allCameras[i].ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit = default(RaycastHit);
			if (this.GetComponent<Collider>().Raycast(ray, out raycastHit, allCameras[i].farClipPlane))
			{
				result = true;
				break;
			}
			i++;
		}
		return result;
	}

	public override void OnMouseOver()
	{
		this.SwitchColor.a = 0.4f;
		this.gameObject.GetComponent<Renderer>().material.color = this.SwitchColor;
	}

	public override void OnMouseExit()
	{
		this.SwitchColor.a = (float)0;
		this.gameObject.GetComponent<Renderer>().material.color = this.SwitchColor;
	}

	public override void Update()
	{
		if (Input.GetButtonDown("Fire1") && this.IsOverCollider())
		{
			this.Pages.SetActiveRecursively(false);
			this.Pages.SetActive(true);
			this.MyPage.SetActiveRecursively(true);
			if (!this.Tab)
			{
				this.Index.SetActive(false);
			}
			else if (!this.IsIndex)
			{
				this.Index.SetActive(false);
			}
		}
	}

	public override void Main()
	{
	}
}
