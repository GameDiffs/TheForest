using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class enableEffigy : MonoBehaviour
{
	public GameObject[] fires;

	public GameObject effigyRange;

	public GameObject BirdMarkers;

	public GameObject Renderers;

	public GameObject LightBillboardIcon;

	public float duration;

	[ItemIdPicker]
	public int _boneItemId;

	[ItemIdPicker]
	public int _skullItemId;

	[ItemIdPicker]
	public int _rockItemId;

	[Header("FMOD")]
	public string lightEvent = "event:/fire/fire_built_start";

	[SerializeThis]
	public bool lightBool;

	[SerializeThis]
	private SerializableTimer timer = new SerializableTimer();

	private void Awake()
	{
		base.enabled = false;
		this.timer.Start();
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonAfterDelay("Take", 0.5f) && !this.lightBool)
		{
			this.LightBillboardIcon.SetActive(false);
			LocalPlayer.Inventory.SpecialItems.SendMessage("LightTheFire", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void GrabEnter()
	{
		base.enabled = !this.lightBool;
		if (base.enabled)
		{
			this.LightBillboardIcon.SetActive(true);
		}
	}

	private void GrabExit()
	{
		base.enabled = false;
		this.LightBillboardIcon.SetActive(false);
	}

	private void OnSerializing()
	{
		this.timer.OnSerializing();
	}

	[DebuggerHidden]
	private IEnumerator OnDeserialized()
	{
		enableEffigy.<OnDeserialized>c__Iterator56 <OnDeserialized>c__Iterator = new enableEffigy.<OnDeserialized>c__Iterator56();
		<OnDeserialized>c__Iterator.<>f__this = this;
		return <OnDeserialized>c__Iterator;
	}

	private void receiveLightFire()
	{
		if (!this.lightBool && base.enabled)
		{
			this.lightEffigy();
			this.LightBillboardIcon.SetActive(false);
			base.Invoke("die", this.duration);
			base.enabled = false;
			this.lightBool = true;
		}
	}

	private void lightEffigy()
	{
		UnityEngine.Debug.Log("lightEffigy:1");
		if (BoltNetwork.isRunning)
		{
			UnityEngine.Debug.Log("lightEffigy:2");
			LightEffigy lightEffigy = LightEffigy.Create(GlobalTargets.OnlyServer);
			lightEffigy.Effigy = base.GetComponentInParent<BoltEntity>();
			lightEffigy.Send();
		}
		else
		{
			this.lightEffigyReal();
		}
	}

	public void lightEffigyReal()
	{
		GameObject[] array = this.fires;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				gameObject.SetActive(true);
			}
		}
		this.effigyRange.SetActive(true);
		this.BirdMarkers.SetActive(false);
		FMODCommon.PlayOneshot(this.lightEvent, base.transform);
	}

	public void dieReal()
	{
		this.BirdMarkers.SetActive(true);
		this.effigyRange.SetActive(false);
		this.lightBool = false;
		Transform transform = BoltNetwork.isRunning ? ItemDatabase.ItemById(this._boneItemId)._pickupPrefabMP : ItemDatabase.ItemById(this._boneItemId)._pickupPrefab;
		Transform transform2 = BoltNetwork.isRunning ? ItemDatabase.ItemById(this._skullItemId)._pickupPrefabMP : ItemDatabase.ItemById(this._skullItemId)._pickupPrefab;
		Transform transform3 = BoltNetwork.isRunning ? ItemDatabase.ItemById(this._rockItemId)._pickupPrefabMP : ItemDatabase.ItemById(this._rockItemId)._pickupPrefab;
		foreach (Transform transform4 in this.Renderers.transform)
		{
			if (transform && (transform4.name.StartsWith("Arm") || transform4.name.StartsWith("Leg")))
			{
				Transform transform5 = BoltNetwork.isRunning ? BoltNetwork.Instantiate(transform.gameObject).transform : UnityEngine.Object.Instantiate<Transform>(transform);
				transform5.position = transform4.position;
				transform5.rotation = transform4.rotation;
			}
			else if (transform2 && transform4.name.StartsWith("Head"))
			{
				Transform transform6 = BoltNetwork.isRunning ? BoltNetwork.Instantiate(transform2.gameObject).transform : UnityEngine.Object.Instantiate<Transform>(transform2);
				transform6.position = transform4.position;
				transform6.rotation = transform4.rotation;
			}
			else if (transform3 && transform4.name.StartsWith("Rock"))
			{
				Transform transform7 = BoltNetwork.isRunning ? BoltNetwork.Instantiate(transform3.gameObject).transform : UnityEngine.Object.Instantiate<Transform>(transform3);
				transform7.position = transform4.position;
				transform7.rotation = transform4.rotation;
			}
		}
		UnityEngine.Object.Destroy(base.GetComponentInParent<PrefabIdentifier>().gameObject);
	}

	private void die()
	{
		if (BoltNetwork.isRunning)
		{
			if (BoltNetwork.isServer)
			{
				base.GetComponentInParent<BoltEntity>().GetState<IBuildingEffigyState>().Lit = false;
			}
		}
		else
		{
			this.dieReal();
		}
	}
}
