using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SteamVR_LaserPointer : MonoBehaviour
{
	public bool active = true;

	public Color color;

	public float thickness = 0.002f;

	public GameObject holder;

	public GameObject pointer;

	private bool isActive;

	public bool addRigidBody;

	public Transform reference;

	private Transform previousContact;

	public event PointerEventHandler PointerIn
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.PointerIn = (PointerEventHandler)Delegate.Combine(this.PointerIn, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.PointerIn = (PointerEventHandler)Delegate.Remove(this.PointerIn, value);
		}
	}

	public event PointerEventHandler PointerOut
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.PointerOut = (PointerEventHandler)Delegate.Combine(this.PointerOut, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.PointerOut = (PointerEventHandler)Delegate.Remove(this.PointerOut, value);
		}
	}

	private void Start()
	{
		this.holder = new GameObject();
		this.holder.transform.parent = base.transform;
		this.holder.transform.localPosition = Vector3.zero;
		this.pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
		this.pointer.transform.parent = this.holder.transform;
		this.pointer.transform.localScale = new Vector3(this.thickness, this.thickness, 100f);
		this.pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
		BoxCollider component = this.pointer.GetComponent<BoxCollider>();
		if (this.addRigidBody)
		{
			if (component)
			{
				component.isTrigger = true;
			}
			Rigidbody rigidbody = this.pointer.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
		}
		else if (component)
		{
			UnityEngine.Object.Destroy(component);
		}
		Material material = new Material(Shader.Find("Unlit/Color"));
		material.SetColor("_Color", this.color);
		this.pointer.GetComponent<MeshRenderer>().material = material;
	}

	public virtual void OnPointerIn(PointerEventArgs e)
	{
		if (this.PointerIn != null)
		{
			this.PointerIn(this, e);
		}
	}

	public virtual void OnPointerOut(PointerEventArgs e)
	{
		if (this.PointerOut != null)
		{
			this.PointerOut(this, e);
		}
	}

	private void Update()
	{
		if (!this.isActive)
		{
			this.isActive = true;
			base.transform.GetChild(0).gameObject.SetActive(true);
		}
		float num = 100f;
		SteamVR_TrackedController component = base.GetComponent<SteamVR_TrackedController>();
		Ray ray = new Ray(base.transform.position, base.transform.forward);
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(ray, out raycastHit);
		if (this.previousContact && this.previousContact != raycastHit.transform)
		{
			PointerEventArgs e = default(PointerEventArgs);
			if (component != null)
			{
				e.controllerIndex = component.controllerIndex;
			}
			e.distance = 0f;
			e.flags = 0u;
			e.target = this.previousContact;
			this.OnPointerOut(e);
			this.previousContact = null;
		}
		if (flag && this.previousContact != raycastHit.transform)
		{
			PointerEventArgs e2 = default(PointerEventArgs);
			if (component != null)
			{
				e2.controllerIndex = component.controllerIndex;
			}
			e2.distance = raycastHit.distance;
			e2.flags = 0u;
			e2.target = raycastHit.transform;
			this.OnPointerIn(e2);
			this.previousContact = raycastHit.transform;
		}
		if (!flag)
		{
			this.previousContact = null;
		}
		if (flag && raycastHit.distance < 100f)
		{
			num = raycastHit.distance;
		}
		if (component != null && component.triggerPressed)
		{
			this.pointer.transform.localScale = new Vector3(this.thickness * 5f, this.thickness * 5f, num);
		}
		else
		{
			this.pointer.transform.localScale = new Vector3(this.thickness, this.thickness, num);
		}
		this.pointer.transform.localPosition = new Vector3(0f, 0f, num / 2f);
	}
}
