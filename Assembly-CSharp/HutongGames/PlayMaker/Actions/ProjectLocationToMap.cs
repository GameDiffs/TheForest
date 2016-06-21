using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Projects the location found with Get Location Info to a 2d map using common projections.")]
	public class ProjectLocationToMap : FsmStateAction
	{
		public enum MapProjection
		{
			EquidistantCylindrical,
			Mercator
		}

		[HutongGames.PlayMaker.Tooltip("Location vector in degrees longitude and latitude. Typically returned by the Get Location Info action.")]
		public FsmVector3 GPSLocation;

		[HutongGames.PlayMaker.Tooltip("The projection used by the map.")]
		public ProjectLocationToMap.MapProjection mapProjection;

		[ActionSection("Map Region"), HasFloatSlider(-180f, 180f)]
		public FsmFloat minLongitude;

		[HasFloatSlider(-180f, 180f)]
		public FsmFloat maxLongitude;

		[HasFloatSlider(-90f, 90f)]
		public FsmFloat minLatitude;

		[HasFloatSlider(-90f, 90f)]
		public FsmFloat maxLatitude;

		[ActionSection("Screen Region")]
		public FsmFloat minX;

		public FsmFloat minY;

		public FsmFloat width;

		public FsmFloat height;

		[ActionSection("Projection"), HutongGames.PlayMaker.Tooltip("Store the projected X coordinate in a Float Variable. Use this to display a marker on the map."), UIHint(UIHint.Variable)]
		public FsmFloat projectedX;

		[HutongGames.PlayMaker.Tooltip("Store the projected Y coordinate in a Float Variable. Use this to display a marker on the map."), UIHint(UIHint.Variable)]
		public FsmFloat projectedY;

		[HutongGames.PlayMaker.Tooltip("If true all coordinates in this action are normalized (0-1); otherwise coordinates are in pixels.")]
		public FsmBool normalized;

		public bool everyFrame;

		private float x;

		private float y;

		public override void Reset()
		{
			this.GPSLocation = new FsmVector3
			{
				UseVariable = true
			};
			this.mapProjection = ProjectLocationToMap.MapProjection.EquidistantCylindrical;
			this.minLongitude = -180f;
			this.maxLongitude = 180f;
			this.minLatitude = -90f;
			this.maxLatitude = 90f;
			this.minX = 0f;
			this.minY = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
			this.projectedX = null;
			this.projectedY = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			if (this.GPSLocation.IsNone)
			{
				base.Finish();
				return;
			}
			this.DoProjectGPSLocation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoProjectGPSLocation();
		}

		private void DoProjectGPSLocation()
		{
			this.x = Mathf.Clamp(this.GPSLocation.Value.x, this.minLongitude.Value, this.maxLongitude.Value);
			this.y = Mathf.Clamp(this.GPSLocation.Value.y, this.minLatitude.Value, this.maxLatitude.Value);
			ProjectLocationToMap.MapProjection mapProjection = this.mapProjection;
			if (mapProjection != ProjectLocationToMap.MapProjection.EquidistantCylindrical)
			{
				if (mapProjection == ProjectLocationToMap.MapProjection.Mercator)
				{
					this.DoMercatorProjection();
				}
			}
			else
			{
				this.DoEquidistantCylindrical();
			}
			this.x *= this.width.Value;
			this.y *= this.height.Value;
			this.projectedX.Value = ((!this.normalized.Value) ? (this.minX.Value + this.x * (float)Screen.width) : (this.minX.Value + this.x));
			this.projectedY.Value = ((!this.normalized.Value) ? (this.minY.Value + this.y * (float)Screen.height) : (this.minY.Value + this.y));
		}

		private void DoEquidistantCylindrical()
		{
			this.x = (this.x - this.minLongitude.Value) / (this.maxLongitude.Value - this.minLongitude.Value);
			this.y = (this.y - this.minLatitude.Value) / (this.maxLatitude.Value - this.minLatitude.Value);
		}

		private void DoMercatorProjection()
		{
			this.x = (this.x - this.minLongitude.Value) / (this.maxLongitude.Value - this.minLongitude.Value);
			float num = ProjectLocationToMap.LatitudeToMercator(this.minLatitude.Value);
			float num2 = ProjectLocationToMap.LatitudeToMercator(this.maxLatitude.Value);
			this.y = (ProjectLocationToMap.LatitudeToMercator(this.GPSLocation.Value.y) - num) / (num2 - num);
		}

		private static float LatitudeToMercator(float latitudeInDegrees)
		{
			float num = Mathf.Clamp(latitudeInDegrees, -85f, 85f);
			num = 0.0174532924f * num;
			return Mathf.Log(Mathf.Tan(num / 2f + 0.7853982f));
		}
	}
}
