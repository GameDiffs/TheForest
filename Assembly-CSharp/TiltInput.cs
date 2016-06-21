using System;
using UnityEngine;

public class TiltInput : MonoBehaviour
{
	public enum AxisOptions
	{
		ForwardAxis,
		SidewaysAxis
	}

	[Serializable]
	public class AxisMapping
	{
		public enum MappingType
		{
			NamedAxis,
			MousePositionX,
			MousePositionY,
			MousePositionZ
		}

		public TiltInput.AxisMapping.MappingType type;

		public string axisName;
	}

	public TiltInput.AxisMapping mapping;

	public TiltInput.AxisOptions tiltAroundAxis;

	public float fullTiltAngle = 25f;

	public float centreAngleOffset;

	private CrossPlatformInput.VirtualAxis steerAxis;

	private void OnEnable()
	{
		if (this.mapping.type == TiltInput.AxisMapping.MappingType.NamedAxis)
		{
			this.steerAxis = new CrossPlatformInput.VirtualAxis(this.mapping.axisName);
		}
	}

	private void Update()
	{
		float value = 0f;
		if (Input.acceleration != Vector3.zero)
		{
			TiltInput.AxisOptions axisOptions = this.tiltAroundAxis;
			if (axisOptions != TiltInput.AxisOptions.ForwardAxis)
			{
				if (axisOptions == TiltInput.AxisOptions.SidewaysAxis)
				{
					value = Mathf.Atan2(Input.acceleration.z, -Input.acceleration.y) * 57.29578f + this.centreAngleOffset;
				}
			}
			else
			{
				value = Mathf.Atan2(Input.acceleration.x, -Input.acceleration.y) * 57.29578f + this.centreAngleOffset;
			}
		}
		float num = Mathf.InverseLerp(-this.fullTiltAngle, this.fullTiltAngle, value) * 2f - 1f;
		switch (this.mapping.type)
		{
		case TiltInput.AxisMapping.MappingType.NamedAxis:
			this.steerAxis.Update(num);
			break;
		case TiltInput.AxisMapping.MappingType.MousePositionX:
			CrossPlatformInput.SetVirtualMousePositionX(num * (float)Screen.width);
			break;
		case TiltInput.AxisMapping.MappingType.MousePositionY:
			CrossPlatformInput.SetVirtualMousePositionY(num * (float)Screen.width);
			break;
		case TiltInput.AxisMapping.MappingType.MousePositionZ:
			CrossPlatformInput.SetVirtualMousePositionZ(num * (float)Screen.width);
			break;
		}
	}

	private void OnDisable()
	{
		this.steerAxis.Remove();
	}
}
