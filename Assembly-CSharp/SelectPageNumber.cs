using System;
using TheForest.Utils;
using UnityEngine;

public class SelectPageNumber : MonoBehaviour
{
	public GameObject MyPageNew;

	public bool Index;

	public bool Tab;

	public GameObject Pages;

	public GameObject IndexPage;

	public Vector3 HighlightOffset;

	public Material HighlightMaterial;

	private GameObject HighlightedPage;

	private bool Highlighted;

	private bool PageIsActive;

	private bool HasMouseOverPrev;

	private bool HasMouseOver;

	private Color SwitchColor;

	private Material MyMat;

	private MaterialPropertyBlock MyMatPropertyBlock;

	public int BranchOvered
	{
		get;
		set;
	}

	public bool SelfOvered
	{
		get;
		set;
	}

	public bool IsOvered
	{
		get
		{
			return this.SelfOvered || this.BranchOvered > 0;
		}
	}

	private bool IsOverCollider()
	{
		bool result = false;
		for (int i = 0; i < Camera.allCameras.Length; i++)
		{
			Camera camera = Camera.allCameras[i];
			Ray ray = camera.ScreenPointToRay(TheForest.Utils.Input.mousePosition);
			RaycastHit raycastHit;
			if (base.GetComponent<Collider>().Raycast(ray, out raycastHit, camera.farClipPlane))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void Awake()
	{
		if (!this.MyMat)
		{
			this.MyMat = base.gameObject.GetComponent<Renderer>().sharedMaterial;
		}
		this.SwitchColor = this.MyMat.color;
		this.SwitchColor.a = 0f;
		this.SwitchMatColor(this.SwitchColor);
	}

	private void SwitchMatColor(Color color)
	{
		if (this.MyMatPropertyBlock == null)
		{
			this.MyMatPropertyBlock = new MaterialPropertyBlock();
		}
		Renderer component = base.gameObject.GetComponent<Renderer>();
		component.GetPropertyBlock(this.MyMatPropertyBlock);
		this.MyMatPropertyBlock.SetColor("_color", this.SwitchColor);
		component.SetPropertyBlock(this.MyMatPropertyBlock);
	}

	private void OnMouseOver()
	{
		this.HasMouseOver = true;
	}

	private void OnMouseExit()
	{
		this.HasMouseOver = false;
	}

	private void Update()
	{
		this.SelfOvered = this.IsOverCollider();
		if ((TheForest.Utils.Input.GetButtonDown("Fire1") || (TheForest.Utils.Input.IsGamePad && TheForest.Utils.Input.GetButtonDown("Take"))) && this.SelfOvered)
		{
			this.OnClick();
		}
		bool flag = this.MyPageNew && this.MyPageNew.activeSelf;
		if (flag != this.PageIsActive || this.HasMouseOverPrev != this.HasMouseOver)
		{
			this.SwitchColor.a = ((!flag) ? ((!this.HasMouseOver) ? 0f : 0.2f) : 0.6f);
			this.SwitchMatColor(this.SwitchColor);
			this.PageIsActive = flag;
			this.HasMouseOverPrev = this.HasMouseOver;
		}
		if (this.Highlighted && ((this.HighlightedPage && this.HighlightedPage.activeSelf) || (!this.HighlightedPage && flag)))
		{
			this.Unhighlight();
		}
	}

	private void OnDisable()
	{
		this.BranchOvered = 0;
	}

	private void OnClick()
	{
		LocalPlayer.Sfx.PlayTurnPage();
		if (!this.PageIsActive)
		{
			this.SwitchColor.a = 0f;
			base.gameObject.GetComponent<Renderer>().material.color = this.SwitchColor;
		}
		if (!this.Index && !this.Tab)
		{
			base.transform.parent.gameObject.SetActive(false);
			this.MyPageNew.SetActive(true);
			LocalPlayer.AnimatedBook.sharedMaterial = this.MyPageNew.GetComponent<Renderer>().sharedMaterial;
		}
		else if (this.Index)
		{
			this.TurnOffAllPages();
			this.MyPageNew.SetActive(true);
			LocalPlayer.AnimatedBook.sharedMaterial = this.MyPageNew.GetComponent<Renderer>().sharedMaterial;
		}
		else if (this.Tab)
		{
			this.TurnOffAllPages();
			this.IndexPage.SetActive(false);
			if (this.Highlighted && this.HighlightedPage)
			{
				this.HighlightedPage.SetActive(true);
			}
			else
			{
				this.MyPageNew.SetActive(true);
			}
			LocalPlayer.AnimatedBook.sharedMaterial = this.MyPageNew.GetComponent<Renderer>().sharedMaterial;
		}
		this.Unhighlight();
	}

	public bool Highlight(GameObject highlightedPage = null)
	{
		if (!this.Highlighted)
		{
			this.Highlighted = true;
			if (this.HighlightMaterial)
			{
				if (!this.MyMat)
				{
					this.MyMat = base.gameObject.GetComponent<Renderer>().sharedMaterial;
				}
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this.HighlightMaterial;
			}
			base.transform.localPosition += this.HighlightOffset;
			this.HighlightedPage = highlightedPage;
		}
		return this.HighlightedPage == highlightedPage;
	}

	private void Unhighlight()
	{
		if (this.Highlighted)
		{
			this.Highlighted = false;
			if (this.HighlightMaterial && this.MyMat)
			{
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this.MyMat;
			}
			base.transform.localPosition -= this.HighlightOffset;
		}
	}

	private void TurnOffAllPages()
	{
		for (int i = this.Pages.transform.childCount - 1; i >= 0; i--)
		{
			this.Pages.transform.GetChild(i).gameObject.SetActive(false);
		}
	}
}
