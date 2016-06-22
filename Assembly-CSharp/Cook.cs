using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class Cook : EntityBehaviour<ICookingState>
{
	private enum Status
	{
		Cooking,
		Cooked,
		Burnt
	}

	public LayerMask PlaceOnLayers;

	public Material Cooked;

	public Material Burnt;

	public EatCooked EatTrigger;

	public GameObject DissolvedPrefab;

	public Renderer _targetRenderer;

	public GameObject _billboardSheen;

	public GameObject _billboardPickup;

	public float _cookDuration = 25f;

	public float _overcookDuration = 60f;

	public float _dissolveDelay = 2f;

	public bool _scaleDisolve = true;

	[SerializeThis]
	public DecayingInventoryItemView.DecayStates _decayState;

	private float _startTime;

	[SerializeThis]
	private float _doneTime;

	private void Start()
	{
		if (!LevelSerializer.IsDeserializing)
		{
			if (DecayingInventoryItemView.LastUsed)
			{
				this._decayState = DecayingInventoryItemView.LastUsed._prevState;
				this._targetRenderer.sharedMaterial = DecayingInventoryItemView.LastUsed._decayStatesMats[(int)this._decayState];
				DecayingInventoryItemView.LastUsed = null;
			}
			if (base.GetComponent<Collider>() is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = (CapsuleCollider)base.GetComponent<Collider>();
				Vector3 vector = base.transform.forward * capsuleCollider.height;
				Vector3 vector2 = base.transform.position;
				Vector3 vector3 = base.transform.position - vector / 2f;
				RaycastHit raycastHit;
				if (Physics.SphereCast(vector2, capsuleCollider.radius, -base.transform.up, out raycastHit, 15f, this.PlaceOnLayers))
				{
					UnityEngine.Debug.DrawLine(vector2, raycastHit.point, Color.red, 10f);
					vector2 = raycastHit.point;
				}
				if (Physics.SphereCast(vector3, capsuleCollider.radius, -base.transform.up, out raycastHit, 15f, this.PlaceOnLayers))
				{
					UnityEngine.Debug.DrawLine(vector3, raycastHit.point, Color.green, 10f);
					vector3 = raycastHit.point;
				}
				else
				{
					vector3 = vector2 - vector;
				}
				base.transform.position = Vector3.Lerp(vector2, vector3, 0.5f) - capsuleCollider.center;
				base.transform.LookAt(vector2);
			}
			if (!BoltNetwork.isRunning || (this.entity.isAttached && this.entity.isOwner))
			{
				base.CancelInvoke("CookMe");
				base.CancelInvoke("OverCooked");
				base.Invoke("CookMe", this._cookDuration);
				base.Invoke("OverCooked", this._overcookDuration);
				this._startTime = Time.time;
				this._doneTime = 0f;
			}
		}
	}

	private void OnSerializing()
	{
		this._doneTime += Time.time - this._startTime;
		this._startTime = Time.time;
	}

	[DebuggerHidden]
	private IEnumerator OnDeserialized()
	{
		Cook.<OnDeserialized>c__Iterator17B <OnDeserialized>c__Iterator17B = new Cook.<OnDeserialized>c__Iterator17B();
		<OnDeserialized>c__Iterator17B.<>f__this = this;
		return <OnDeserialized>c__Iterator17B;
	}

	private void CookMe()
	{
		GameStats.CookedFood.Invoke();
		if (this.EatTrigger)
		{
			this.EatTrigger.gameObject.SetActive(true);
		}
		if (this.Cooked)
		{
			this._targetRenderer.sharedMaterial = this.Cooked;
		}
		this._billboardSheen.SetActive(true);
		if (this.entity.isAttached && this.entity.isOwner)
		{
			base.state.Status = 1;
		}
	}

	private void OverCooked()
	{
		GameStats.BurntFood.Invoke();
		if (this.EatTrigger)
		{
			this.EatTrigger.gameObject.SetActive(true);
			this.EatTrigger.Burnt = true;
		}
		if (this.Burnt)
		{
			this._targetRenderer.sharedMaterial = this.Burnt;
		}
		if (this.DissolvedPrefab)
		{
			this.DisolveBurnt();
		}
		if (this.entity.isAttached && this.entity.isOwner)
		{
			base.state.Status = 2;
		}
	}

	private void DisolveBurnt()
	{
		if (!BoltNetwork.isRunning || (this.entity.isAttached && this.entity.isOwner))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.DissolvedPrefab, base.transform.position, base.transform.rotation) as GameObject;
			if (this._scaleDisolve)
			{
				gameObject.transform.localScale.Scale(this._targetRenderer.transform.localScale);
			}
			if (BoltNetwork.isRunning && gameObject.GetComponent<BoltEntity>())
			{
				BoltNetwork.Attach(gameObject);
			}
			if (BoltNetwork.isRunning)
			{
				BoltNetwork.Destroy(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	public override void Attached()
	{
		if (!this.entity.isOwner)
		{
			base.state.AddCallback("Status", new PropertyCallbackSimple(this.OnStatusUpdate));
			this.OnStatusUpdate();
		}
	}

	private void OnStatusUpdate()
	{
		Cook.Status status = (Cook.Status)base.state.Status;
		if (status != Cook.Status.Cooked)
		{
			if (status == Cook.Status.Burnt)
			{
				this.OverCooked();
			}
		}
		else
		{
			this.CookMe();
		}
	}
}
