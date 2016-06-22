using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class camFollowHead : MonoBehaviour
{
	private TriggerCutScene cutScene;

	private GameObject planeCrashHull;

	public Transform headJnt;

	public float damp;

	private Transform thisTr;

	public Transform camTr;

	public Transform planePos;

	public Transform seatPos;

	public Transform aislePos;

	private playerScriptSetup setup;

	private playerHitReactions reactions;

	public SimpleMouseRotator mouse1;

	public SimpleMouseRotator mouse2;

	public bool oculus;

	public bool flying;

	public bool aisle;

	public bool shakeBlock;

	public bool followAnim;

	private Vector3 playerCamPos;

	private Vector3 ovrCamPos;

	private Quaternion prevRotation;

	public float offsetY;

	public float offsetZ;

	public bool lockXCam;

	public bool lockYCam;

	public bool smoothLock;

	public bool smoothUnLock;

	private bool storePrevRot;

	private bool reparent;

	private Vector3 pos;

	public float introWeight = 0.1f;

	private void Awake()
	{
		base.GetComponent<Animation>()["turbulanceLight"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>()["turbulanceLight"].speed = 1.2f;
		base.GetComponent<Animation>()["camShakePlane"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>()["camShakePlane"].speed = 2.4f;
		base.GetComponent<Animation>()["camShakePlaneStart"].wrapMode = WrapMode.Once;
		base.GetComponent<Animation>()["camShakePlaneStart"].speed = 1.7f;
		base.GetComponent<Animation>()["camShakePlaneGround"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>()["camShakePlaneGround"].speed = 3.5f;
		base.GetComponent<Animation>()["noShake"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>()["camShakeFall"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>()["camShakeFall"].speed = 2.5f;
		base.GetComponent<Animation>()["turbulanceLight"].layer = 1;
		base.GetComponent<Animation>()["camShakePlaneGround"].layer = 1;
		base.GetComponent<Animation>()["camShakePlane"].layer = 1;
		base.GetComponent<Animation>()["noShake"].layer = 1;
		base.GetComponent<Animation>()["camShakeFall"].layer = 1;
		this.setup = base.transform.root.GetComponentInChildren<playerScriptSetup>();
		this.thisTr = base.transform;
		this.mouse1 = base.GetComponentInChildren<SimpleMouseRotator>();
		this.mouse2 = base.transform.root.GetComponent<SimpleMouseRotator>();
		this.camTr = this.mouse1.transform;
	}

	private void Start()
	{
		this.offsetY = 0f;
		this.offsetZ = -0.05f;
		GameObject gameObject = GameObject.Find("PlayerPlanePosition");
		if (gameObject)
		{
			this.planePos = gameObject.transform;
		}
		GameObject gameObject2 = GameObject.Find("PlayerAislePosition");
		if (gameObject2)
		{
			this.aislePos = gameObject2.transform;
		}
		if (this.setup.playerCam)
		{
			this.playerCamPos = this.setup.playerCam.localPosition;
		}
		if (this.setup.OvrCam)
		{
			this.ovrCamPos = this.setup.OvrCam.localPosition;
		}
		base.Invoke("checkPlayerControl", 1f);
	}

	private void OnEnable()
	{
		base.Invoke("checkPlayerControl", 1f);
	}

	private void checkPlayerControl()
	{
		if (!Clock.planecrash)
		{
			LocalPlayer.CamRotator.enabled = true;
		}
		LocalPlayer.MainRotator.enabled = true;
	}

	private void enableFollowAnim()
	{
		this.followAnim = true;
	}

	private void disableFollowAnim()
	{
		this.followAnim = false;
	}

	[DebuggerHidden]
	public IEnumerator preCrashCameraShake()
	{
		camFollowHead.<preCrashCameraShake>c__Iterator53 <preCrashCameraShake>c__Iterator = new camFollowHead.<preCrashCameraShake>c__Iterator53();
		<preCrashCameraShake>c__Iterator.<>f__this = this;
		return <preCrashCameraShake>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator planeCameraShake()
	{
		camFollowHead.<planeCameraShake>c__Iterator54 <planeCameraShake>c__Iterator = new camFollowHead.<planeCameraShake>c__Iterator54();
		<planeCameraShake>c__Iterator.<>f__this = this;
		return <planeCameraShake>c__Iterator;
	}

	public void stopAllCameraShake()
	{
		base.GetComponent<Animation>().Stop("camShakePlaneStart");
		base.GetComponent<Animation>().Stop("camShakePlane");
		base.GetComponent<Animation>().Stop("camShakePlaneGround");
		base.GetComponent<Animation>().Stop("camShakeFall");
		base.GetComponent<Animation>().Stop("noShake");
		base.GetComponent<Animation>().Stop("turbulanceLight");
	}

	[DebuggerHidden]
	public IEnumerator startFallingShake()
	{
		camFollowHead.<startFallingShake>c__Iterator55 <startFallingShake>c__Iterator = new camFollowHead.<startFallingShake>c__Iterator55();
		<startFallingShake>c__Iterator.<>f__this = this;
		return <startFallingShake>c__Iterator;
	}

	private void Update()
	{
		this.doCamFollow();
	}

	private void LateUpdate()
	{
		this.updateCamPosition();
	}

	public void updateCamPosition()
	{
		if (this.flying)
		{
			if (!this.seatPos)
			{
				this.seatPos = Scene.TriggerCutScene.playerSeatPos;
			}
			if (this.seatPos)
			{
				this.thisTr.position = this.seatPos.position;
			}
			if (!this.shakeBlock)
			{
				this.shakeBlock = true;
			}
		}
		else if (this.aisle)
		{
			this.thisTr.position = this.aislePos.position;
			if (!this.shakeBlock)
			{
				this.shakeBlock = true;
			}
		}
		else
		{
			this.pos = this.headJnt.position;
			this.thisTr.position = this.pos;
			if (this.setup.playerCam)
			{
				this.setup.playerCam.localPosition = new Vector3(this.playerCamPos.x, this.playerCamPos.y + this.offsetY, this.playerCamPos.z + this.offsetZ);
			}
			if (this.setup.OvrCam)
			{
				this.setup.OvrCam.localPosition = new Vector3(this.ovrCamPos.x, this.ovrCamPos.y + this.offsetY, this.ovrCamPos.z + this.offsetZ);
			}
		}
		this.doCamFollow();
	}

	private void doCamFollow()
	{
		if (this.followAnim || this.lockYCam)
		{
			if (this.smoothLock)
			{
				Quaternion quaternion = this.camTr.rotation;
				quaternion = Quaternion.Slerp(quaternion, this.headJnt.rotation, Time.deltaTime * 5f);
				this.camTr.rotation = quaternion;
				if (this.camTr.rotation == this.headJnt.rotation)
				{
					this.smoothLock = false;
				}
			}
			else if (!this.smoothUnLock)
			{
				this.storePrevRot = false;
				this.thisTr.rotation = this.headJnt.rotation;
			}
			if (!this.smoothLock && !this.smoothUnLock)
			{
				this.thisTr.rotation = this.headJnt.rotation;
				this.camTr.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
		}
		if (this.smoothUnLock)
		{
			Vector3 vector = this.thisTr.localEulerAngles;
			vector = Vector3.Slerp(vector, Vector3.zero, Time.deltaTime * 2f);
			this.thisTr.localEulerAngles = vector;
			if (this.thisTr.localEulerAngles == Vector3.zero)
			{
				LocalPlayer.CamRotator.resetOriginalRotation = true;
				this.smoothUnLock = false;
				this.followAnim = false;
				LocalPlayer.CamRotator.enabled = true;
			}
		}
	}

	public void disableFlying()
	{
		UnityEngine.Debug.Log("disableFlying");
		base.GetComponent<Animation>().Stop("camShakePlane");
		base.GetComponent<Animation>().Stop("lightTurbulance");
		this.flying = false;
		this.aisle = false;
		LocalPlayer.AnimControl.playerCollider.enabled = false;
		LocalPlayer.AnimControl.playerHeadCollider.enabled = false;
		LocalPlayer.Transform.position = this.planePos.position;
		LocalPlayer.Transform.eulerAngles = new Vector3(LocalPlayer.Transform.eulerAngles.x, this.planePos.eulerAngles.y, LocalPlayer.Transform.eulerAngles.z);
		LocalPlayer.CamFollowHead.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		LocalPlayer.AnimControl.playerCollider.enabled = true;
		LocalPlayer.AnimControl.playerHeadCollider.enabled = true;
	}

	public void enableMouseControl(bool on)
	{
		if (on)
		{
			this.mouse1.enabled = true;
			this.mouse2.enabled = true;
		}
		else
		{
			this.mouse1.enabled = false;
			this.mouse2.enabled = false;
		}
	}

	public void enableAisle()
	{
		base.GetComponent<Animation>().Stop("camShakePlane");
		this.flying = false;
		this.shakeBlock = false;
		this.aisle = true;
		LocalPlayer.CamFollowHead.mouse2.rotationRange = new Vector2(0f, 70f);
		LocalPlayer.CamRotator.rotationRange = new Vector2(95f, 0f);
	}
}
