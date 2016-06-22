using System;
using UnityEngine;

[Serializable]
public class SelectPageNumber : MonoBehaviour
{
	public GameObject MyPageNew;

	private Color SwitchColor;

	public bool Index;

	public bool Tab;

	public GameObject Pages;

	public GameObject IndexPage;

	private FMOD_StudioEventEmitter PageSound;

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

	public override void Awake()
	{
		GameObject gameObject = GameObject.FindWithTag("PaperSfx");
		this.PageSound = (FMOD_StudioEventEmitter)gameObject.GetComponent(typeof(FMOD_StudioEventEmitter));
		Material material = this.gameObject.GetComponent<Renderer>().material;
		this.SwitchColor = material.color;
		this.SwitchColor.a = (float)0;
		this.gameObject.GetComponent<Renderer>().material.color = this.SwitchColor;
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
			this.OnClick();
		}
	}

	public override void OnClick()
	{
		this.MyPageNew.SetActiveRecursively(true);
		this.PageSound.Play();
		this.SwitchColor.a = (float)0;
		this.gameObject.GetComponent<Renderer>().material.color = this.SwitchColor;
		if (!this.Index && !this.Tab)
		{
			this.transform.parent.gameObject.SetActiveRecursively(false);
		}
		else if (this.Index)
		{
			this.Pages.SetActiveRecursively(false);
			this.Pages.SetActive(true);
		}
		else if (this.Tab)
		{
			this.IndexPage.SetActive(false);
			this.Pages.SetActiveRecursively(false);
			this.Pages.SetActive(true);
			this.MyPageNew.SetActiveRecursively(true);
		}
	}

	public override void Main()
	{
	}
}
