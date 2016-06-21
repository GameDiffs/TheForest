using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class survivalBookController : MonoBehaviour
{
	public GameObject survivalBookReal;

	public GameObject survivalBookAnimated;

	public Animator animator;

	public bool bookIsOpen;

	public bool realBookOpen;

	private bool initBool;

	private void Start()
	{
		this.animator = base.transform.GetComponent<Animator>();
		this.animator.SetBoolReflected("bookHeld", true);
		base.Invoke("initMe", 1f);
	}

	private void initMe()
	{
		this.initBool = true;
	}

	private void Update()
	{
		if (!this.initBool)
		{
			return;
		}
		AnimatorStateInfo nextAnimatorStateInfo = LocalPlayer.Animator.GetNextAnimatorStateInfo(1);
		AnimatorStateInfo currentAnimatorStateInfo = LocalPlayer.Animator.GetCurrentAnimatorStateInfo(1);
		if (LocalPlayer.Animator.GetBool("bookHeld"))
		{
			LocalPlayer.Animator.SetLayerWeightReflected(1, 1f);
			LocalPlayer.Animator.SetLayerWeightReflected(2, 0f);
			LocalPlayer.Animator.SetLayerWeightReflected(3, 0f);
			LocalPlayer.Animator.SetLayerWeightReflected(4, 1f);
			LocalPlayer.Animator.SetBoolReflected("clampSpine", true);
		}
		if (currentAnimatorStateInfo.IsName("upperBody.bookIdle") && !this.realBookOpen)
		{
			LocalPlayer.CamFollowHead.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			LocalPlayer.FpCharacter.LockView(false);
			this.survivalBookReal.SetActive(true);
			this.survivalBookAnimated.SetActive(false);
			this.survivalBookReal.SendMessage("CheckPage");
			this.realBookOpen = true;
			this.bookIsOpen = true;
			if (LocalPlayer.Inventory)
			{
				LocalPlayer.Inventory.BlockTogglingInventory = false;
			}
		}
		if (currentAnimatorStateInfo.IsName("upperBody.bookIdle") && this.bookIsOpen)
		{
			this.survivalBookReal.SetActive(true);
			this.survivalBookAnimated.SetActive(false);
		}
		if (!LocalPlayer.Animator.GetBool("bookHeld") && this.bookIsOpen)
		{
			this.animator.SetBoolReflected("bookHeld", false);
			LocalPlayer.Animator.SetBoolReflected("clampSpine", false);
			LocalPlayer.MainRotator.resetOriginalRotation = true;
			if (LocalPlayer.Inventory.CurrentView != PlayerInventory.PlayerViews.Inventory)
			{
				LocalPlayer.FpCharacter.UnLockView();
			}
			this.bookIsOpen = false;
			LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
			LocalPlayer.CamRotator.xOffset = 0f;
			base.Invoke("setCloseBook", 1f);
			this.survivalBookReal.SendMessage("CloseBook", SendMessageOptions.DontRequireReceiver);
			this.survivalBookAnimated.SetActive(true);
			this.survivalBookReal.SetActive(false);
		}
	}

	public void setCloseBook()
	{
		if (LocalPlayer.Inventory)
		{
			LocalPlayer.Inventory.BlockTogglingInventory = false;
		}
		this.bookIsOpen = false;
		this.realBookOpen = false;
		base.gameObject.SetActive(false);
	}

	public void setOpenBook()
	{
		if (!this.animator)
		{
			this.animator = base.transform.GetComponent<Animator>();
		}
		this.animator.SetBool("bookHeld", true);
		LocalPlayer.Animator.SetBoolReflected("clampSpine", true);
		LocalPlayer.CamRotator.rotationRange = new Vector2(0f, 0f);
		LocalPlayer.CamRotator.xOffset = -20f;
		float normalizedTime = LocalPlayer.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime;
		this.animator.CrossFade("Base Layer.toBookIdle", 0f, 0, normalizedTime);
		if (LocalPlayer.Inventory)
		{
			LocalPlayer.Inventory.BlockTogglingInventory = true;
		}
	}

	private void OnDisable()
	{
		LocalPlayer.Animator.SetBoolReflected("clampSpine", false);
		this.bookIsOpen = false;
		this.realBookOpen = false;
		if (LocalPlayer.Inventory)
		{
			LocalPlayer.Inventory.BlockTogglingInventory = false;
		}
	}
}
