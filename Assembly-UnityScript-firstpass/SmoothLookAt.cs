using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Look At")]
[Serializable]
public class SmoothLookAt : MonoBehaviour
{
	public Transform target;

	public float damping;

	public bool smooth;

	public SmoothLookAt()
	{
		this.damping = 6f;
		this.smooth = true;
	}

	public override void LateUpdate()
	{
		if (this.target)
		{
			if (this.smooth)
			{
				Quaternion to = Quaternion.LookRotation(this.target.position - this.transform.position);
				this.transform.rotation = Quaternion.Slerp(this.transform.rotation, to, Time.deltaTime * this.damping);
			}
			else
			{
				this.transform.LookAt(this.target);
			}
		}
	}

	public override void Start()
	{
		if (this.GetComponent<Rigidbody>())
		{
			this.GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	public override void Main()
	{
	}
}
