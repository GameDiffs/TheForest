using Bolt;
using System;
using UnityEngine;

public class CoopSuitcase : EntityBehaviour<ISuitcaseState>, IPriorityCalculator
{
	private const float correctionDelay = 0.5f;

	public Transform Trigger;

	public Transform Interior;

	public Transform ClientTransform;

	public Rigidbody Rigidbody;

	public Collider DisableOnClientCollider;

	private float lerpTimer;

	private float correctionStartTime;

	private Vector3 clientTransformPrevPos;

	public GameObject[] DisableOnOpen;

	public GameObject[] FlarePickups;

	public GameObject ClothPickup;

	bool IPriorityCalculator.Always
	{
		get
		{
			return true;
		}
	}

	float IPriorityCalculator.CalculateStatePriority(BoltConnection connection, int skipped)
	{
		return CoopUtils.CalculatePriorityFor(connection, this.entity, 1f, skipped);
	}

	float IPriorityCalculator.CalculateEventPriority(BoltConnection connection, Bolt.Event evnt)
	{
		return CoopUtils.CalculatePriorityFor(connection, this.entity, 1f, 1);
	}

	public void PushedByClient(Vector3 direction)
	{
		this.correctionStartTime = Time.time + 0.5f;
		ClientSuitcasePush clientSuitcasePush = ClientSuitcasePush.Create(GlobalTargets.Everyone);
		clientSuitcasePush.Suitcase = this.entity;
		clientSuitcasePush.Direction = direction * 10f;
		clientSuitcasePush.Send();
		this.Rigidbody.velocity = direction * 10f;
		base.enabled = true;
	}

	private void Awake()
	{
		if (!BoltNetwork.isClient)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (BoltNetwork.isClient && this.entity.IsAttached() && this.ClientTransform && this.correctionStartTime < Time.time)
		{
			if (this.clientTransformPrevPos != this.ClientTransform.position)
			{
				this.lerpTimer = 0f;
				this.clientTransformPrevPos = this.ClientTransform.position;
			}
			if (this.lerpTimer < 1f)
			{
				this.lerpTimer += Time.deltaTime / 0.5f;
				this.lerpTimer = Mathf.Clamp01(this.lerpTimer);
				base.transform.position = Vector3.Slerp(base.transform.position, this.ClientTransform.position, this.lerpTimer);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.ClientTransform.rotation, this.lerpTimer);
			}
		}
	}

	public override void Attached()
	{
		if (BoltNetwork.isClient)
		{
			this.ClientTransform = new GameObject(base.gameObject.name + "_CLIENTTRANSFORM").GetComponent<Transform>();
			base.state.Transform.SetTransforms(this.ClientTransform);
		}
		else
		{
			base.state.Transform.SetTransforms(base.transform);
		}
		if (this.entity.isOwner)
		{
			if (this.ClothPickup && !this.ClothPickup.activeSelf)
			{
				base.state.ClothPickedUp = true;
			}
			base.state.Open = this.Interior.gameObject.activeSelf;
		}
		if (this.ClothPickup)
		{
			base.state.AddCallback("ClothPickedUp", new PropertyCallbackSimple(this.OnClothPickup));
		}
		if (this.FlarePickups.Length > 0)
		{
			base.state.AddCallback("FlaresPickedUp", new PropertyCallbackSimple(this.OnFlaresPickedUp));
		}
		base.state.AddCallback("Open", new PropertyCallbackSimple(this.OnOpen));
	}

	private void OnClothPickup()
	{
		if (this.ClothPickup)
		{
			this.ClothPickup.SetActive(false);
			if (this.entity.isOwner)
			{
				base.SendMessage("UpdateGreebleData");
			}
		}
	}

	private void OnFlaresPickedUp()
	{
		for (int i = 0; i < this.FlarePickups.Length; i++)
		{
			int num = 1 << i;
			if ((base.state.FlaresPickedUp & num) != 0)
			{
				this.FlarePickups[i].transform.parent.GetChild(i).gameObject.SetActive(false);
			}
		}
	}

	private void OnOpen()
	{
		if (base.state.Open)
		{
			if (this.Trigger.gameObject.activeSelf)
			{
				this.Trigger.SendMessage("Open_Perform");
				this.Trigger.gameObject.SetActive(false);
			}
			for (int i = 0; i < this.DisableOnOpen.Length; i++)
			{
				this.DisableOnOpen[i].SetActive(false);
			}
		}
	}
}
