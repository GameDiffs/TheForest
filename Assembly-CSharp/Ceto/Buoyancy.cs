using System;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Buoyancy/Buoyancy")]
	public class Buoyancy : MonoBehaviour
	{
		public enum MASS_UNIT
		{
			KILOGRAMS,
			TENS_OF_KILOGRAMS,
			TONNES,
			TENS_OF_TONNES
		}

		private float DENSITY_WATER = 999.97f;

		public float radius = 0.5f;

		[Range(100f, 10000f)]
		public float density = 400f;

		[Range(0f, 100f)]
		public float stickyness = 0.1f;

		public Buoyancy.MASS_UNIT unit = Buoyancy.MASS_UNIT.TENS_OF_TONNES;

		public float dragCoefficient = 0.3f;

		public bool PartOfStructure
		{
			get;
			set;
		}

		public float Volume
		{
			get;
			private set;
		}

		public float SubmergedVolume
		{
			get;
			private set;
		}

		public float PercentageSubmerged
		{
			get
			{
				return this.SubmergedVolume / this.Volume;
			}
		}

		public float SurfaceArea
		{
			get;
			private set;
		}

		public float Mass
		{
			get;
			private set;
		}

		public float WaterHeight
		{
			get;
			private set;
		}

		public Vector3 BuoyantForce
		{
			get;
			private set;
		}

		public Vector3 DragForce
		{
			get;
			private set;
		}

		public Vector3 Stickyness
		{
			get;
			private set;
		}

		public Vector3 TotalForces
		{
			get
			{
				return this.BuoyantForce + this.DragForce + this.Stickyness;
			}
		}

		private void Start()
		{
			this.UpdateProperties();
		}

		private void FixedUpdate()
		{
			if (this.PartOfStructure)
			{
				return;
			}
			Rigidbody rigidbody = base.GetComponent<Rigidbody>();
			if (rigidbody == null)
			{
				rigidbody = base.gameObject.AddComponent<Rigidbody>();
			}
			rigidbody.mass = this.Mass;
			this.UpdateProperties();
			this.UpdateForces(rigidbody);
			rigidbody.AddForce(this.TotalForces);
		}

		public void UpdateProperties()
		{
			this.Volume = 4.18879032f * Mathf.Pow(this.radius, 3f);
			this.Mass = this.Volume * this.density * this.GetUnitScale();
			this.SurfaceArea = 12.566371f * Mathf.Pow(this.radius, 2f);
		}

		public void UpdateForces(Rigidbody body)
		{
			if (Ocean.Instance == null)
			{
				this.BuoyantForce = Vector3.zero;
				this.DragForce = Vector3.zero;
				this.Stickyness = Vector3.zero;
				return;
			}
			Vector3 position = base.transform.position;
			this.WaterHeight = Ocean.Instance.QueryWaves(position.x, position.z);
			this.CalculateSubmersion(this.radius, position.y);
			float unitScale = this.GetUnitScale();
			float num = this.DENSITY_WATER * unitScale * this.SubmergedVolume;
			this.BuoyantForce = Physics.gravity * -num;
			Vector3 a = body.velocity;
			float magnitude = a.magnitude;
			a = a.normalized * magnitude * magnitude * -1f;
			this.DragForce = 0.5f * this.dragCoefficient * this.DENSITY_WATER * unitScale * this.SubmergedVolume * a;
			this.Stickyness = Vector3.up * (this.WaterHeight - position.y) * this.Mass * this.stickyness;
		}

		private void CalculateSubmersion(float r, float y)
		{
			float num = this.WaterHeight - (y - this.radius);
			float num2 = 2f * r - num;
			if (num2 <= 0f)
			{
				this.SubmergedVolume = this.Volume;
				return;
			}
			if (num2 > 2f * r)
			{
				this.SubmergedVolume = 0f;
				return;
			}
			float num3 = Mathf.Sqrt(num * num2);
			this.SubmergedVolume = 0.5235988f * num * (3f * num3 * num3 + num * num);
		}

		private float GetUnitScale()
		{
			switch (this.unit)
			{
			case Buoyancy.MASS_UNIT.KILOGRAMS:
				return 1f;
			case Buoyancy.MASS_UNIT.TENS_OF_KILOGRAMS:
				return 0.1f;
			case Buoyancy.MASS_UNIT.TONNES:
				return 0.001f;
			case Buoyancy.MASS_UNIT.TENS_OF_TONNES:
				return 0.0001f;
			default:
				return 1f;
			}
		}

		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, this.radius);
		}
	}
}
