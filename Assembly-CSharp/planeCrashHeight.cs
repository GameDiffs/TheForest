using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class planeCrashHeight : MonoBehaviour
{
	public sceneTracker sceneInfo;

	public GameObject[] markers;

	public GameObject[] finalCrashPos;

	public TriggerCutScene cutScene;

	public GameObject hullForward;

	public Transform seatPos;

	public Transform aislePos;

	public GameObject navCollision;

	private Vector3 pos;

	private float terrainPosY;

	public Transform Tr;

	public Transform top;

	public Transform rootTr;

	private Transform crashMarker;

	public float hitDist;

	public float smoothSpeed;

	public bool doHeight;

	private int layer;

	private int layerMask;

	private RaycastHit hit;

	private RaycastHit topHit;

	public bool overrideCrashSite;

	public int setCrashSite = 10;

	private void Awake()
	{
		if (LevelSerializer.IsDeserializing)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	public void beginPlaneCrash()
	{
		this.setPlanePosition();
		this.startOnPlane();
		LocalPlayer.CamFollowHead.enableMouseControl(false);
	}

	public void setPlanePosition()
	{
		this.layer = 26;
		this.layerMask = 1 << this.layer;
		GameObject[] array = PlaneCrashLocations.markers;
		UnityEngine.Random.seed = Convert.ToInt32(DateTime.UtcNow.ToUnixTimestamp());
		if (this.overrideCrashSite)
		{
			PlaneCrashLocations.crashSite = this.setCrashSite;
		}
		else
		{
			PlaneCrashLocations.crashSite = UnityEngine.Random.Range(0, array.Length);
		}
		UnityEngine.Debug.Log("setPlanePosition site=" + PlaneCrashLocations.crashSite);
		this.crashMarker = array[PlaneCrashLocations.crashSite].transform;
		this.crashMarker.gameObject.SetActive(true);
		this.rootTr.position = new Vector3(this.crashMarker.position.x, 500f, this.crashMarker.position.z);
		this.rootTr.eulerAngles = new Vector3(this.rootTr.eulerAngles.x, this.crashMarker.eulerAngles.y, this.rootTr.eulerAngles.z);
		if (Physics.Raycast(this.rootTr.position, Vector3.down, out this.topHit, 10000f, this.layerMask))
		{
			this.rootTr.position = new Vector3(this.rootTr.position.x, this.hit.point.y, this.rootTr.position.z);
		}
		if (BoltNetwork.isServer)
		{
			Transform transform = PlaneCrashLocations.finalPositions[PlaneCrashLocations.crashSite].transform;
			CoopServerInfo.Instance.state.PlanePosition = transform.position;
			CoopServerInfo.Instance.state.PlaneRotation = transform.rotation;
		}
	}

	private void startOnPlane()
	{
		LocalPlayer.CamFollowHead.flying = true;
		base.StartCoroutine("setSeatCam");
	}

	public void setupCameraShake()
	{
		base.Invoke("startPreCrashShake", 1f);
	}

	public void startCrashCameraShake()
	{
		LocalPlayer.CamFollowHead.StartCoroutine("planeCameraShake");
		base.Invoke("hideExtraBits", 10f);
	}

	private void startPreCrashShake()
	{
		LocalPlayer.CamFollowHead.StartCoroutine("preCrashCameraShake");
	}

	public void skipPlaneCrash()
	{
		base.CancelInvoke("startPlaneCrash");
		LocalPlayer.CamFollowHead.StopCoroutine("planeCameraShake");
		LocalPlayer.CamFollowHead.stopAllCameraShake();
		this.hideExtraBits();
	}

	[DebuggerHidden]
	public IEnumerator setSeatCam()
	{
		planeCrashHeight.<setSeatCam>c__IteratorD1 <setSeatCam>c__IteratorD = new planeCrashHeight.<setSeatCam>c__IteratorD1();
		<setSeatCam>c__IteratorD.<>f__this = this;
		return <setSeatCam>c__IteratorD;
	}

	[DebuggerHidden]
	public IEnumerator setAisleCam2()
	{
		return new planeCrashHeight.<setAisleCam2>c__IteratorD2();
	}

	private void hideExtraBits()
	{
		this.hullForward.SetActive(false);
	}

	public void doNavMesh()
	{
		if (this.navCollision != null)
		{
			this.navCollision.SetActive(true);
			base.Invoke("disableNavCollision", 1f);
		}
	}

	private void disableNavCollision()
	{
		this.navCollision.SetActive(false);
	}

	public void resetMouseRotation()
	{
		LocalPlayer.CamFollowHead.mouse2.rotationRange = new Vector2(0f, 999f);
	}

	private void Update()
	{
		if (this.doHeight)
		{
			this.top.eulerAngles = new Vector3(this.top.eulerAngles.x, this.crashMarker.eulerAngles.y, this.top.eulerAngles.z);
			Vector3 origin = new Vector3(this.top.position.x, this.top.position.y + 1000f, this.top.position.z);
			if (Physics.Raycast(origin, Vector3.down, out this.topHit, 5000f, this.layerMask))
			{
				this.top.rotation = Quaternion.Lerp(this.top.rotation, Quaternion.LookRotation(Vector3.Cross(this.top.right, this.topHit.normal), this.topHit.normal), Time.deltaTime * this.smoothSpeed);
			}
		}
		this.pos = new Vector3(this.Tr.position.x, this.Tr.position.y + 2000f, this.Tr.position.z);
		if (Physics.Raycast(this.pos, Vector3.down, out this.hit, 8000f, this.layerMask))
		{
			this.hitDist = this.hit.distance;
			if (this.hit.distance < 2008f && !this.doHeight)
			{
				this.doHeight = true;
			}
			if (this.doHeight)
			{
				this.terrainPosY = this.hit.point.y + 4f;
				this.top.position = new Vector3(this.top.position.x, this.terrainPosY, this.top.position.z);
			}
		}
	}

	private void LateUpdate()
	{
		if (this.doHeight)
		{
			this.terrainPosY = this.hit.point.y + 4f;
			this.top.position = new Vector3(this.top.position.x, this.terrainPosY, this.top.position.z);
		}
	}
}
