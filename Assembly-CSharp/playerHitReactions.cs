using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class playerHitReactions : MonoBehaviour
{
	private FirstPersonCharacter fps;

	private Animator animator;

	private playerScriptSetup setup;

	public GameObject camAnim;

	private Animation camAnimation;

	public Vector3 hitDir;

	public bool kingHitBool;

	private SimpleMouseRotator camRotator;

	private Rigidbody _rb;

	private int explodeHash = Animator.StringToHash("explode");

	private float walkSpeed;

	private float runSpeed;

	private float strafeSpeed;

	private void Awake()
	{
		this.camRotator = GameObject.FindWithTag("MainCamera").GetComponent<SimpleMouseRotator>();
		this.fps = LocalPlayer.FpCharacter;
		this.animator = base.transform.GetComponentInChildren<Animator>();
		this.setup = base.GetComponentInChildren<playerScriptSetup>();
		this.walkSpeed = this.fps.walkSpeed;
		this.runSpeed = this.fps.runSpeed;
		this.strafeSpeed = this.fps.strafeSpeed;
		this.camAnimation = this.camAnim.GetComponent<Animation>();
		this._rb = base.GetComponent<Rigidbody>();
	}

	public void doBirdOnHand()
	{
		if (LocalPlayer.Inventory.IsLeftHandEmpty())
		{
			this.setup.pmControl.SendEvent("toOnHand");
		}
	}

	public void enableParryState()
	{
		this.kingHitBool = true;
		this.animator.SetBoolReflected("parryBool", true);
		if (this.camAnim)
		{
			this.camAnimation.Play("camShake1", PlayMode.StopAll);
		}
		base.Invoke("cancelParry", 0.5f);
		base.Invoke("disableParryState", 2.5f);
	}

	private void cancelParry()
	{
		this.animator.SetBoolReflected("parryBool", false);
	}

	private void disableParryState()
	{
		this.kingHitBool = false;
	}

	public void enableHitState()
	{
		this.camAnimation.Play("camShake1", PlayMode.StopAll);
	}

	public void explodeForce(Transform pos)
	{
	}

	[DebuggerHidden]
	public IEnumerator enableExplodeCamera()
	{
		playerHitReactions.<enableExplodeCamera>c__IteratorDB <enableExplodeCamera>c__IteratorDB = new playerHitReactions.<enableExplodeCamera>c__IteratorDB();
		<enableExplodeCamera>c__IteratorDB.<>f__this = this;
		return <enableExplodeCamera>c__IteratorDB;
	}

	public void disableExplodeCamera()
	{
	}

	public void enableExplodeShake(float dist)
	{
		if (dist > 100f)
		{
			return;
		}
		float weight;
		if (dist < 20f)
		{
			weight = 1f;
		}
		else
		{
			weight = 20f / dist;
		}
		this.camAnimation["explodeShake"].layer = 1;
		this.camAnimation["noShake"].layer = 0;
		this.camAnimation.Play("noShake");
		this.camAnimation.Play("explodeShake", PlayMode.StopSameLayer);
		this.camAnimation["explodeShake"].weight = weight;
	}

	public void enableFootShake(float dist, float mag)
	{
		if (LocalPlayer.AnimControl.swimming)
		{
			return;
		}
		if (dist < 30f)
		{
			float weight = (1f - dist / 30f) * mag;
			this.camAnimation["explodeShake"].layer = 1;
			this.camAnimation["noShake"].layer = 0;
			this.camAnimation.Play("noShake");
			this.camAnimation.Play("explodeShake", PlayMode.StopSameLayer);
			this.camAnimation["explodeShake"].weight = weight;
			return;
		}
	}

	public void enableWeaponHitState()
	{
		this.camAnimation.Play("camShake2", PlayMode.StopAll);
	}

	private void enableBlockState()
	{
	}

	public void disableBlockState()
	{
	}

	private void enableBlockHitState()
	{
	}

	private void disableBlockHitState()
	{
	}

	public void enableControllerFreeze()
	{
		this.fps.walkSpeed = 1.2f;
		this.fps.runSpeed = 1.2f;
		this.fps.strafeSpeed = 1.2f;
		this.fps.hitByEnemy = true;
	}

	[DebuggerHidden]
	public IEnumerator setControllerSpeed(float speed)
	{
		playerHitReactions.<setControllerSpeed>c__IteratorDC <setControllerSpeed>c__IteratorDC = new playerHitReactions.<setControllerSpeed>c__IteratorDC();
		<setControllerSpeed>c__IteratorDC.speed = speed;
		<setControllerSpeed>c__IteratorDC.<$>speed = speed;
		<setControllerSpeed>c__IteratorDC.<>f__this = this;
		return <setControllerSpeed>c__IteratorDC;
	}

	public void disableControllerFreeze()
	{
		this.fps.walkSpeed = this.walkSpeed;
		this.fps.runSpeed = this.runSpeed;
		this.fps.strafeSpeed = this.strafeSpeed;
		this.fps.hitByEnemy = false;
		LocalPlayer.Ridigbody.drag = 0f;
	}

	[DebuggerHidden]
	public IEnumerator enablePushBack(float strength)
	{
		playerHitReactions.<enablePushBack>c__IteratorDD <enablePushBack>c__IteratorDD = new playerHitReactions.<enablePushBack>c__IteratorDD();
		<enablePushBack>c__IteratorDD.<>f__this = this;
		return <enablePushBack>c__IteratorDD;
	}

	[DebuggerHidden]
	private IEnumerator enableWeaponImpactState()
	{
		return new playerHitReactions.<enableWeaponImpactState>c__IteratorDE();
	}

	[DebuggerHidden]
	private IEnumerator enableTreeImpactState()
	{
		return new playerHitReactions.<enableTreeImpactState>c__IteratorDF();
	}

	[DebuggerHidden]
	private IEnumerator enableWeaponBreakState()
	{
		return new playerHitReactions.<enableWeaponBreakState>c__IteratorE0();
	}

	private void setSeatedCam()
	{
		if (this.camRotator)
		{
			this.camRotator.enabled = true;
		}
	}

	public void lookAtExplosion(Vector3 pos)
	{
		Vector3 worldPosition = pos;
		worldPosition.y = base.transform.position.y;
		base.transform.LookAt(worldPosition, base.transform.up);
	}
}
