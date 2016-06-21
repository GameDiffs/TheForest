using System;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Buoyancy/BuoyantStructure")]
	public class BuoyantStructure : MonoBehaviour
	{
		public float maxAngularVelocity = 0.05f;

		private Buoyancy[] m_buoyancy;

		private void Start()
		{
			this.m_buoyancy = base.GetComponentsInChildren<Buoyancy>();
			int num = this.m_buoyancy.Length;
			for (int i = 0; i < num; i++)
			{
				this.m_buoyancy[i].PartOfStructure = true;
			}
		}

		private void FixedUpdate()
		{
			Rigidbody rigidbody = base.GetComponent<Rigidbody>();
			if (rigidbody == null)
			{
				rigidbody = base.gameObject.AddComponent<Rigidbody>();
			}
			float num = 0f;
			int num2 = this.m_buoyancy.Length;
			for (int i = 0; i < num2; i++)
			{
				if (this.m_buoyancy[i].enabled)
				{
					this.m_buoyancy[i].UpdateProperties();
					num += this.m_buoyancy[i].Mass;
				}
			}
			rigidbody.mass = num;
			Vector3 position = base.transform.position;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			for (int j = 0; j < num2; j++)
			{
				if (this.m_buoyancy[j].enabled)
				{
					this.m_buoyancy[j].UpdateForces(rigidbody);
					Vector3 position2 = this.m_buoyancy[j].transform.position;
					Vector3 totalForces = this.m_buoyancy[j].TotalForces;
					Vector3 lhs = position2 - position;
					vector += totalForces;
					vector2 += Vector3.Cross(lhs, totalForces);
				}
			}
			rigidbody.maxAngularVelocity = this.maxAngularVelocity;
			rigidbody.AddForce(vector);
			rigidbody.AddTorque(vector2);
		}
	}
}
