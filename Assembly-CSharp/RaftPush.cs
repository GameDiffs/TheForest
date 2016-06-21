using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class RaftPush : EntityBehaviour<IRaftState>
{
	public enum States
	{
		Idle,
		Auto,
		DriverStanding,
		DriverLocked
	}

	public enum MoveDirection
	{
		None,
		Forward,
		Backward
	}

	public Buoyancy _buoyancy;

	public GameObject _raft;

	public GameObject _oar;

	public Transform _driverPos;

	public SheenBillboard _canLockIcon;

	public float _distanceFromOceanCenter;

	public float _speed = 10f;

	public float _paddlingDelay = 1.8f;

	public float _maxVelocity = 10f;

	public float _dampSpeed = 5f;

	public float _rotateForce = 5000f;

	public float _torqueDamp = 0.15f;

	public ForceMode _forceMode;

	[ItemIdPicker(Item.Types.Equipment)]
	public int _paddleItemId;

	private Rigidbody _rb;

	public RaftPush.States _state;

	private Vector3 _direction;

	private FirstPersonCharacter _driver;

	private float _prevMass;

	private bool _shouldUnlock;

	private bool _isGrabbed;

	private bool stickToRaft;

	public bool InWater
	{
		get
		{
			return (!BoltNetwork.isRunning) ? this._buoyancy.InWater : base.state.InWater;
		}
	}

	private void Awake()
	{
		base.enabled = false;
		this._state = RaftPush.States.Idle;
		this._direction = Vector3.zero;
		this._canLockIcon.gameObject.SetActive(false);
		this._rb = base.transform.parent.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		bool flag = this._isGrabbed && this._state == RaftPush.States.DriverStanding && this.InWater;
		if (flag && BoltNetwork.isRunning && base.state.GrabbedBy != null)
		{
			flag = false;
		}
		if (!this._canLockIcon.gameObject.activeSelf.Equals(flag))
		{
			this._canLockIcon.gameObject.SetActive(flag);
		}
		if (this._shouldUnlock)
		{
			this._shouldUnlock = false;
			if (BoltNetwork.isRunning)
			{
				RaftGrab raftGrab = RaftGrab.Create(GlobalTargets.OnlyServer);
				raftGrab.Raft = base.GetComponentInParent<BoltEntity>();
				raftGrab.Player = null;
				raftGrab.Send();
			}
			else
			{
				this.offRaft();
			}
			return;
		}
		if (this._state == RaftPush.States.DriverLocked && !this.InWater)
		{
			this._shouldUnlock = true;
		}
		else
		{
			this._shouldUnlock = false;
		}
		if (this.stickToRaft)
		{
			LocalPlayer.FpCharacter.enabled = false;
			LocalPlayer.AnimControl.controller.useGravity = false;
			LocalPlayer.AnimControl.controller.isKinematic = true;
			Vector3 position = this._driverPos.position;
			position.y += LocalPlayer.AnimControl.playerCollider.height / 2f - LocalPlayer.AnimControl.playerCollider.center.y;
			LocalPlayer.Transform.position = position;
			LocalPlayer.Transform.rotation = this._driverPos.rotation;
			LocalPlayer.Animator.SetLayerWeightReflected(2, 1f);
		}
		if (TheForest.Utils.Input.GetButtonDown("Take"))
		{
			if (flag)
			{
				if (BoltNetwork.isRunning)
				{
					RaftGrab raftGrab2 = RaftGrab.Create(GlobalTargets.OnlyServer);
					raftGrab2.Raft = base.GetComponentInParent<BoltEntity>();
					raftGrab2.Player = LocalPlayer.Entity;
					raftGrab2.Send();
				}
				else
				{
					this.onRaft();
				}
			}
			else if (this._state == RaftPush.States.DriverLocked)
			{
				if (BoltNetwork.isRunning)
				{
					RaftGrab raftGrab3 = RaftGrab.Create(GlobalTargets.OnlyServer);
					raftGrab3.Raft = base.GetComponentInParent<BoltEntity>();
					raftGrab3.Player = null;
					raftGrab3.Send();
				}
				else
				{
					this.offRaft();
				}
			}
		}
		else if (this._state == RaftPush.States.DriverLocked)
		{
			RaftControl raftControl = null;
			bool flag2 = false;
			RaftPush.MoveDirection moveDirection = RaftPush.MoveDirection.None;
			float axis = TheForest.Utils.Input.GetAxis("Horizontal");
			if (!BoltNetwork.isRunning)
			{
				this.TurnRaft(axis);
			}
			if (TheForest.Utils.Input.GetButton("Fire1") || TheForest.Utils.Input.GetButton("AltFire"))
			{
				if (this.CheckDistanceFromOceanCenter())
				{
					moveDirection = ((!TheForest.Utils.Input.GetButton("Fire1")) ? RaftPush.MoveDirection.Backward : RaftPush.MoveDirection.Forward);
					if (!BoltNetwork.isRunning)
					{
						this.PushRaft(moveDirection);
					}
					this._driver.enablePaddleOnRaft(true);
					flag2 = true;
				}
			}
			else
			{
				this._driver.enablePaddleOnRaft(false);
			}
			if (BoltNetwork.isRunning && (!Mathf.Approximately(axis, 0f) || flag2))
			{
				if (BoltNetwork.isRunning)
				{
					raftControl = RaftControl.Create(GlobalTargets.OnlyServer);
				}
				raftControl.Rotation = axis;
				raftControl.Movement = (int)moveDirection;
				raftControl.Raft = base.GetComponentInParent<BoltEntity>();
				raftControl.Send();
			}
		}
		else if (this._state == RaftPush.States.Auto)
		{
			this._direction = Scene.OceanCeto.transform.position - base.transform.position;
			this._direction.Normalize();
			this._raft.GetComponent<Rigidbody>().AddForce(this._direction * this._speed * 5f, ForceMode.Impulse);
			if (this.CheckDistanceFromOceanCenter())
			{
				this._state = RaftPush.States.DriverLocked;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && this._state == RaftPush.States.Idle)
		{
			base.enabled = true;
			this._driver = LocalPlayer.FpCharacter;
			this._state = RaftPush.States.DriverStanding;
			this._canLockIcon.gameObject.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && (this._state == RaftPush.States.DriverStanding || this._state == RaftPush.States.Idle))
		{
			if (this._driver == LocalPlayer.FpCharacter)
			{
				this.offRaft();
			}
			base.enabled = false;
			this._state = RaftPush.States.Idle;
			this._canLockIcon.gameObject.SetActive(false);
		}
	}

	private void GrabEnter()
	{
		this._isGrabbed = true;
	}

	private void GrabExit()
	{
		this._isGrabbed = false;
	}

	private void onRaft()
	{
		if (Vector3.Angle(this._raft.transform.up, Vector3.up) > 90f)
		{
			this._raft.transform.localEulerAngles = new Vector3(0f, this._raft.transform.localEulerAngles.y, 0f);
		}
		PlayerInventory component = this._driver.GetComponent<PlayerInventory>();
		LocalPlayer.AnimControl.oarHeld.SetActive(true);
		LocalPlayer.AnimControl.currRaft = base.gameObject;
		this._oar.SetActive(false);
		LocalPlayer.Inventory.HideAllEquiped(false);
		LocalPlayer.FpCharacter.enabled = false;
		LocalPlayer.AnimControl.controller.useGravity = false;
		LocalPlayer.AnimControl.controller.isKinematic = true;
		LocalPlayer.AnimControl.controller.Sleep();
		LocalPlayer.AnimControl.playerCollider.enabled = false;
		LocalPlayer.AnimControl.playerHeadCollider.enabled = false;
		LocalPlayer.Transform.parent = this._raft.transform;
		Vector3 position = this._driverPos.position;
		position.y += LocalPlayer.AnimControl.playerCollider.height / 2f - LocalPlayer.AnimControl.playerCollider.center.y;
		LocalPlayer.Transform.position = position;
		LocalPlayer.Transform.rotation = this._driverPos.rotation;
		LocalPlayer.MainRotator.enabled = false;
		LocalPlayer.CamRotator.rotationRange = new Vector2(100f, 130f);
		this._driver.OnRaft();
		this._canLockIcon.gameObject.SetActive(false);
		this._state = RaftPush.States.DriverLocked;
		LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmBool("paddleBool").Value = true;
		this.stickToRaft = true;
	}

	private void offRaft()
	{
		PlayerInventory component = this._driver.GetComponent<PlayerInventory>();
		LocalPlayer.AnimControl.oarHeld.SetActive(false);
		LocalPlayer.AnimControl.currRaft = null;
		this._oar.SetActive(true);
		component.EquipPreviousWeaponDelayed();
		if (this.stickToRaft)
		{
			this.ShutDown();
		}
		base.GetComponent<Collider>().enabled = false;
		base.GetComponent<Collider>().enabled = true;
		if (BoltNetwork.isRunning && this.entity.isAttached && base.state.GrabbedBy == LocalPlayer.Entity)
		{
			RaftGrab raftGrab = RaftGrab.Create(GlobalTargets.OnlyServer);
			raftGrab.Raft = base.GetComponentInParent<BoltEntity>();
			raftGrab.Player = null;
			raftGrab.Send();
		}
	}

	private void ShutDown()
	{
		this._driver.OffRaft();
		this._driver = null;
		this._state = RaftPush.States.Idle;
		LocalPlayer.Transform.parent = null;
		LocalPlayer.AnimControl.controller.useGravity = true;
		LocalPlayer.AnimControl.controller.isKinematic = false;
		LocalPlayer.AnimControl.controller.WakeUp();
		LocalPlayer.FpCharacter.enabled = true;
		LocalPlayer.FpCharacter.Locked = false;
		LocalPlayer.MainRotator.resetOriginalRotation = true;
		LocalPlayer.MainRotator.enabled = true;
		LocalPlayer.AnimControl.playerCollider.enabled = true;
		LocalPlayer.AnimControl.playerHeadCollider.enabled = true;
		LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
		LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmBool("paddleBool").Value = false;
		LocalPlayer.Animator.SetBoolReflected("paddleBool", false);
		this.stickToRaft = false;
		LocalPlayer.Transform.localEulerAngles = new Vector3(0f, LocalPlayer.Transform.localEulerAngles.y, 0f);
		this._canLockIcon.gameObject.SetActive(false);
		base.enabled = false;
	}

	public void PushRaft(RaftPush.MoveDirection dir)
	{
		if (dir == RaftPush.MoveDirection.Forward)
		{
			this._direction = this._raft.transform.forward;
		}
		else
		{
			if (dir != RaftPush.MoveDirection.Backward)
			{
				return;
			}
			this._direction = this._raft.transform.forward * -1f;
		}
		this._direction.y = 0f;
		this._direction *= this._speed * 1.25f;
		Vector3 velocity = this._rb.velocity;
		Vector3 target = this._direction - velocity;
		Vector3 vector = Vector3.zero;
		Vector3 zero = Vector3.zero;
		vector = Vector3.SmoothDamp(vector, target, ref zero, this._dampSpeed);
		this._rb.AddForce(vector, this._forceMode);
		if (this._rb.velocity.magnitude > this._maxVelocity)
		{
			Vector3 vector2 = this._rb.velocity.normalized;
			vector2 *= this._maxVelocity;
			this._rb.velocity = vector2;
		}
	}

	public void TurnRaft(float axis)
	{
		float target = axis * this._rotateForce * Mathf.Clamp(this._rb.velocity.normalized.magnitude, 0.1f, 1f);
		float num = 0f;
		float num2 = 0f;
		num2 = Mathf.SmoothDamp(num2, target, ref num, this._torqueDamp);
		this._rb.AddTorque(0f, num2, 0f, ForceMode.Force);
	}

	private bool CheckDistanceFromOceanCenter()
	{
		return Vector3.Distance(base.transform.position, Scene.OceanCeto.transform.position) < this._distanceFromOceanCenter;
	}

	public override void Attached()
	{
		base.state.Transform.SetTransforms(base.transform.parent);
		base.state.AddCallback("GrabbedBy", new PropertyCallbackSimple(this.OnGrabbed));
	}

	private void OnGrabbed()
	{
		if (base.state.GrabbedBy == LocalPlayer.Entity)
		{
			if (this._state >= RaftPush.States.DriverStanding)
			{
				if (this._driver && this._driver == LocalPlayer.FpCharacter && this._state == RaftPush.States.DriverStanding)
				{
					this.onRaft();
				}
			}
			else
			{
				this.offRaft();
			}
		}
		else
		{
			if (this._state == RaftPush.States.DriverLocked)
			{
				this.offRaft();
			}
			this._oar.SetActive(base.state.GrabbedBy == null);
		}
	}
}
