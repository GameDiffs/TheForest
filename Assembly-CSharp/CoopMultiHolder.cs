using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.World;
using TheForest.Utils;
using UnityEngine;

public class CoopMultiHolder : EntityBehaviour<IMultiHolderState>
{
	private Rigidbody rb;

	private float awakeTime;

	[SerializeField]
	private activateSledPush sledPush;

	[SerializeField]
	private MultiHolder multiholder;

	[SerializeField]
	private Renderer[] renderers;

	[SerializeField]
	private GameObject[] rendererRoots;

	[SerializeField]
	private Transform bodyPosition0;

	[SerializeField]
	private Transform bodyPosition1;

	[SerializeField]
	private Transform bodyPosition2;

	private BoltEntity body0;

	private BoltEntity body1;

	private BoltEntity body2;

	private void Awake()
	{
		if (!BoltNetwork.isRunning)
		{
			UnityEngine.Object.Destroy(this);
		}
		this.awakeTime = Time.time;
	}

	[DebuggerHidden]
	private IEnumerator ClampVelocityRoutine()
	{
		CoopMultiHolder.<ClampVelocityRoutine>c__Iterator1B <ClampVelocityRoutine>c__Iterator1B = new CoopMultiHolder.<ClampVelocityRoutine>c__Iterator1B();
		<ClampVelocityRoutine>c__Iterator1B.<>f__this = this;
		return <ClampVelocityRoutine>c__Iterator1B;
	}

	public override void Attached()
	{
		this.rb = base.GetComponentInChildren<Rigidbody>();
		if (this.entity.isOwner)
		{
			base.StartCoroutine(this.ClampVelocityRoutine());
		}
		else
		{
			this.rb.isKinematic = true;
			this.rb.useGravity = false;
		}
		base.state.Transform.SetTransforms(base.transform);
		base.state.AddCallback("LogCount", new PropertyCallbackSimple(this.LogCountChanged));
		base.state.AddCallback("Replaces", new PropertyCallbackSimple(this.ReplacesChanged));
		base.state.AddCallback("GrabbedBy", new PropertyCallbackSimple(this.GrabbedByChanged));
		base.state.AddCallback("ContentType", new PropertyCallbackSimple(this.ContentTypeChanged));
		base.state.AddCallback("Body0", this.BodyCallback(0));
		base.state.AddCallback("Body1", this.BodyCallback(1));
		base.state.AddCallback("Body2", this.BodyCallback(2));
		base.StartCoroutine(this.PhysicsToggle());
	}

	private void SetPickupTrigger(BoltEntity body, bool state)
	{
		if (body)
		{
			CoopMutantDummy componentInChildren = body.GetComponentInChildren<CoopMutantDummy>();
			if (componentInChildren)
			{
				componentInChildren.PickupTrigger.SetActive(state);
			}
			else
			{
				CoopMutantDummy componentInChildren2 = body.GetComponentInChildren<CoopMutantDummy>();
				if (componentInChildren2)
				{
					componentInChildren2.PickupTrigger.SetActive(state);
				}
			}
		}
	}

	private PropertyCallbackSimple BodyCallback(int index)
	{
		return delegate
		{
			switch (index)
			{
			case 0:
				if (this.state.Body0)
				{
					this.body0 = this.state.Body0;
					this.SetPickupTrigger(this.body0, false);
				}
				else if (this.body0)
				{
					this.SetPickupTrigger(this.body0, true);
				}
				break;
			case 1:
				if (this.state.Body1)
				{
					this.body1 = this.state.Body1;
					this.SetPickupTrigger(this.body1, false);
				}
				else if (this.body1)
				{
					this.SetPickupTrigger(this.body1, true);
				}
				break;
			case 2:
				if (this.state.Body2)
				{
					this.body2 = this.state.Body2;
					this.SetPickupTrigger(this.body2, false);
				}
				else if (this.body2)
				{
					this.SetPickupTrigger(this.body2, true);
				}
				break;
			}
		};
	}

	private void ContentTypeChanged()
	{
		if (!this.entity.isOwner)
		{
			this.multiholder._contentTypeActual = (MultiHolder.ContentTypes)base.state.ContentType;
			this.multiholder.ItemCountChangedMP();
		}
	}

	private void LogCountChanged()
	{
		if (base.state.IsReal)
		{
			if (base.state.GrabbedBy)
			{
				if (base.state.GrabbedBy.isOwner)
				{
					MultiHolder[] componentsInChildren = base.state.GrabbedBy.GetComponentsInChildren<MultiHolder>(true);
					if (componentsInChildren.Length > 0)
					{
						componentsInChildren[0]._contentActual = base.state.LogCount;
						componentsInChildren[0].ItemCountChangedMP();
					}
				}
			}
			else
			{
				MultiHolder[] componentsInChildren2 = base.GetComponentsInChildren<MultiHolder>(true);
				if (componentsInChildren2.Length > 0)
				{
					componentsInChildren2[0].ItemCountChangedMP();
				}
			}
		}
		else
		{
			MultiHolder[] componentsInChildren3 = base.GetComponentsInChildren<MultiHolder>(true);
			if (componentsInChildren3.Length > 0)
			{
				componentsInChildren3[0].ItemCountChangedMP();
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator PhysicsToggle()
	{
		return new CoopMultiHolder.<PhysicsToggle>c__Iterator1C();
	}

	public override void Detached()
	{
		if (base.state.Replaces)
		{
			base.state.Replaces.GetComponent<CoopMultiHolder>().sledPush.Interraction(true);
			base.state.Replaces.GetComponent<CoopMultiHolder>().multiholder.gameObject.SetActive(true);
			if (base.state.Replaces.isOwner)
			{
				base.state.Replaces.GetState<IMultiHolderState>().GrabbedBy = null;
				MultiHolder[] componentsInChildren = base.state.Replaces.GetComponentsInChildren<MultiHolder>(true);
				if (componentsInChildren.Length > 0)
				{
					componentsInChildren[0]._contentActual = base.state.LogCount;
					componentsInChildren[0]._contentTypeActual = (MultiHolder.ContentTypes)base.state.ContentType;
					componentsInChildren[0].ItemCountChangedMP();
				}
				base.state.Replaces.transform.position = base.transform.position;
				base.state.Replaces.transform.rotation = base.transform.rotation;
				base.state.Replaces.GetState<IMultiHolderState>().Body0 = base.state.Body0;
				base.state.Replaces.GetState<IMultiHolderState>().Body1 = base.state.Body1;
				base.state.Replaces.GetState<IMultiHolderState>().Body2 = base.state.Body2;
			}
			base.state.Replaces.GetComponent<CoopMultiHolder>().Hide(false, false);
		}
	}

	private void ReplacesChanged()
	{
		if (base.state.Replaces)
		{
			if (base.state.Replaces.isOwner)
			{
				base.state.Replaces.GetState<IMultiHolderState>().ReplacedBy = this.entity;
			}
			base.state.Replaces.GetComponent<CoopMultiHolder>().sledPush.Interraction(false);
			base.state.Replaces.GetComponent<CoopMultiHolder>().multiholder.gameObject.SetActive(false);
			this.sledPush.Interraction(this.entity.isOwner);
			this.multiholder.gameObject.SetActive(!this.entity.isOwner);
			base.state.Replaces.GetComponent<CoopMultiHolder>().Hide(true, true);
			if (this.entity.isOwner)
			{
				base.GetComponentInChildren<activateSledPush>().enableSled();
			}
		}
	}

	private void GrabbedByChanged()
	{
		if (base.state.GrabbedBy)
		{
			if (base.state.GrabbedBy == LocalPlayer.Entity)
			{
				BoltEntity boltEntity = BoltNetwork.Instantiate(BoltPrefabs.MultiSledBuilt, base.transform.position, base.transform.rotation);
				IMultiHolderState state = boltEntity.GetState<IMultiHolderState>();
				MultiHolder[] componentsInChildren = boltEntity.GetComponentsInChildren<MultiHolder>(true);
				if (componentsInChildren.Length > 0)
				{
					componentsInChildren[0]._contentActual = base.state.LogCount;
					componentsInChildren[0]._contentTypeActual = (MultiHolder.ContentTypes)base.state.ContentType;
					componentsInChildren[0].ItemCountChangedMP();
					state.Replaces = this.entity;
					state.LogCount = base.state.LogCount;
					state.Body0 = base.state.Body0;
					state.Body1 = base.state.Body1;
					state.Body2 = base.state.Body2;
				}
			}
		}
		else
		{
			MultiHolder[] componentsInChildren2 = base.GetComponentsInChildren<MultiHolder>(true);
			if (componentsInChildren2.Length > 0)
			{
				componentsInChildren2[0].ItemCountChangedMP();
			}
		}
	}

	public void Hide(bool hide, bool delayPhysics)
	{
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			renderer.enabled = !hide;
		}
		if (hide)
		{
			GameObject[] array2 = this.rendererRoots;
			for (int j = 0; j < array2.Length; j++)
			{
				GameObject gameObject = array2[j];
				gameObject.SetActive(false);
			}
		}
		if (delayPhysics)
		{
			this.HidePhysics(hide);
		}
		else
		{
			base.StartCoroutine(this.HideDelayRoutine(hide));
		}
	}

	[DebuggerHidden]
	private IEnumerator HideDelayRoutine(bool hide)
	{
		CoopMultiHolder.<HideDelayRoutine>c__Iterator1D <HideDelayRoutine>c__Iterator1D = new CoopMultiHolder.<HideDelayRoutine>c__Iterator1D();
		<HideDelayRoutine>c__Iterator1D.hide = hide;
		<HideDelayRoutine>c__Iterator1D.<$>hide = hide;
		<HideDelayRoutine>c__Iterator1D.<>f__this = this;
		return <HideDelayRoutine>c__Iterator1D;
	}

	private void HidePhysics(bool hide)
	{
		if (hide)
		{
			if (this.entity.isOwner)
			{
				this.rb.isKinematic = true;
				this.rb.useGravity = false;
			}
		}
		else if (this.entity.isOwner)
		{
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
		}
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Collider collider = componentsInChildren[i];
			collider.enabled = !hide;
		}
	}
}
