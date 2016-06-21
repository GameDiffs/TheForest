using System;
using TheForest.Utils;
using UnityEngine;

public class mutantNetAnimatorControl : MonoBehaviour
{
	private Animator animator;

	private AnimatorStateInfo currState0;

	private AnimatorStateInfo nextState0;

	private CoopMutantTransformDelayer ctd;

	private CoopMutantSetup cms;

	private Transform rootTr;

	private int deathOnGroundHash = Animator.StringToHash("deathOnGround");

	private int deathHash = Animator.StringToHash("death");

	private int scrambleHash = Animator.StringToHash("scramble");

	private int idleHash = Animator.StringToHash("idle");

	private int inTrapHash = Animator.StringToHash("inTrap");

	private Vector3 currPos;

	private Vector3 lastPos;

	private Vector3 moveDir;

	private float terrainPos;

	private float updateTimer;

	private float closestDist = float.PositiveInfinity;

	public int closestPlayerInt;

	public float playerDist;

	private Transform footPivot;

	private Vector3 lookPos;

	public GameObject currentTarget;

	private float transitionTime;

	public Transform footJnt;

	private bool inNooseTrap;

	private void Start()
	{
		this.animator = base.transform.GetComponent<Animator>();
		this.ctd = base.transform.parent.GetComponent<CoopMutantTransformDelayer>();
		this.rootTr = base.transform.parent;
		this.cms = base.transform.parent.GetComponent<CoopMutantSetup>();
	}

	private void Update()
	{
		if (Time.time > this.updateTimer)
		{
			if (!Scene.SceneTracker)
			{
				return;
			}
			if (Scene.SceneTracker.allPlayers.Count == 0)
			{
				return;
			}
			this.closestDist = float.PositiveInfinity;
			if (Scene.SceneTracker.allPlayers.Count > 0)
			{
				for (int i = 0; i < Scene.SceneTracker.allPlayers.Count; i++)
				{
					if (Scene.SceneTracker.allPlayers[i])
					{
						float sqrMagnitude = (base.transform.position - Scene.SceneTracker.allPlayers[i].transform.position).sqrMagnitude;
						if (sqrMagnitude < this.closestDist)
						{
							this.closestDist = sqrMagnitude;
							this.closestPlayerInt = i;
						}
					}
				}
			}
			this.currentTarget = Scene.SceneTracker.allPlayers[this.closestPlayerInt];
			this.updateTimer = Time.time + 1f;
		}
	}

	private void OnAnimatorMove()
	{
		this.currState0 = this.animator.GetCurrentAnimatorStateInfo(0);
		this.nextState0 = this.animator.GetNextAnimatorStateInfo(0);
		if (this.currState0.tagHash == this.inTrapHash)
		{
			if (this.cms.nooseTrapPivot)
			{
				this.inNooseTrap = true;
				Vector3 position = this.cms.nooseTrapPivot.position;
				position.y = this.rootTr.position.y;
				this.rootTr.position = Vector3.Lerp(this.rootTr.position, position, Time.deltaTime * 15f);
				if (this.cms.nooseFixer)
				{
					this.cms.nooseFixer.sprungRopeGo.SetActive(true);
					this.cms.nooseFixer.looseRopeGo.SetActive(false);
					this.cms.nooseFixer.sprungNooseJoint.transform.parent = this.footJnt;
					this.cms.nooseFixer.sprungNooseJoint.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
					this.cms.nooseFixer.sprungNooseJoint.transform.localPosition = Vector3.zero;
				}
			}
		}
		else if (this.inNooseTrap && this.cms.nooseFixer)
		{
			this.cms.nooseFixer.sprungNooseJoint.transform.parent = this.cms.nooseTrapPivot.transform;
			this.inNooseTrap = false;
		}
	}

	private void LateUpdate()
	{
		if (this.inNooseTrap && this.cms.nooseFixer)
		{
			this.cms.nooseFixer.sprungNooseJoint.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
			this.cms.nooseFixer.sprungNooseJoint.transform.localPosition = Vector3.zero;
		}
	}

	private void OnAnimatorIK(int layer)
	{
		if (!this.animator)
		{
			return;
		}
		if (!this.animator.enabled)
		{
			return;
		}
		if (this.currentTarget)
		{
			Collider component = this.currentTarget.GetComponent<Collider>();
			if (component)
			{
				this.lookPos = component.bounds.center;
			}
			else
			{
				this.lookPos = this.currentTarget.transform.position;
			}
			this.lookPos.y = this.lookPos.y + 1.3f;
		}
		Vector3 vector = Vector3.zero;
		vector = Vector3.Slerp(vector, this.lookPos, Time.time * 3f);
		this.animator.SetLookAtPosition(vector);
		if (this.animator)
		{
			if (this.currState0.tagHash == this.deathHash)
			{
				this.animator.SetLookAtWeight(0f, 0.1f, 0.6f, 1f, 0.9f);
			}
			else if (this.currState0.tagHash == this.idleHash && !this.animator.IsInTransition(0))
			{
				this.animator.SetLookAtWeight(1f, 0.2f, 0.8f, 1f, 0.9f);
			}
			else if (this.currState0.tagHash == this.idleHash && this.animator.IsInTransition(0))
			{
				this.animator.SetLookAtWeight(1f, 0.2f, 0.8f, 1f, 0.9f);
			}
			else if (this.currState0.tagHash == this.idleHash && this.nextState0.tagHash != this.idleHash && this.animator.IsInTransition(0))
			{
				this.transitionTime = this.animator.GetAnimatorTransitionInfo(0).normalizedTime;
				this.animator.SetLookAtWeight(1f - this.transitionTime, 0.2f, 0.8f, 1f, 0.9f);
			}
			else if (this.currState0.tagHash != this.idleHash && !this.animator.IsInTransition(0))
			{
				this.animator.SetLookAtWeight(0f, 0.2f, 0.8f, 1f, 0.9f);
			}
			else if (this.nextState0.tagHash == this.idleHash && this.currState0.tagHash != this.idleHash && this.animator.IsInTransition(0))
			{
				this.transitionTime = this.animator.GetAnimatorTransitionInfo(0).normalizedTime;
				this.animator.SetLookAtWeight(this.transitionTime, 0.2f, 0.8f, 1f, 0.9f);
			}
		}
	}
}
