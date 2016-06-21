using System;
using TheForest.Utils;
using UnityEngine;

[Serializable]
public class LOD_Settings
{
	public enum LowLodModes
	{
		Always,
		HighDrawDistance
	}

	public enum BillboardLowLodModes
	{
		MatchLowMode,
		RangeOnly
	}

	public enum CaveModes
	{
		Always,
		CaveOnly,
		OutsideOnly
	}

	public enum ScaleModes
	{
		ScaledQuality = 1,
		Quality,
		One
	}

	[QualitySettingCurve(6)]
	public AnimationCurve QualityRatio = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(3f, 1f)
	});

	public LOD_Settings.ScaleModes[] ScaleRanges = new LOD_Settings.ScaleModes[3];

	[Range(0f, 300f)]
	public float[] Ranges = new float[0];

	[Range(0f, 100f)]
	public float StippleRange = 25f;

	public int UpscaleToStippleRange = -1;

	public bool Use2dDistance = true;

	public CustomBillboard[] Billboards;

	public Material[] StippledMaterials;

	public LOD_Settings.LowLodModes LowLodMode;

	public LOD_Settings.BillboardLowLodModes BillboardLowLodMode;

	public LOD_Settings.CaveModes CaveMode = LOD_Settings.CaveModes.OutsideOnly;

	public TheForestQualitySettings.DrawDistances newObjectDrawDistanceFrom = TheForestQualitySettings.DrawDistances.UltraLow;

	public TheForestQualitySettings.DrawDistances newObjectDrawDistanceTo = TheForestQualitySettings.DrawDistances.UltraLow;

	public bool DrawDebug;

	private Vector2 lastStippleRange = Vector2.zero;

	private int lastBillboardCount;

	private int lastStippledMaterialCount;

	private float[] currentQuality = new float[3];

	private float[] currentRanges = new float[3];

	private int unConstrainedMidLodCount;

	private TheForestQualitySettings.DrawDistances newObjectMaxDrawDistance = TheForestQualitySettings.DrawDistances.UltraLow;

	public TheForestQualitySettings.DrawDistances GetNewObjectMaxDrawDistance
	{
		get
		{
			if (++this.newObjectMaxDrawDistance > this.newObjectDrawDistanceTo)
			{
				this.newObjectMaxDrawDistance = this.newObjectDrawDistanceFrom;
			}
			return this.newObjectMaxDrawDistance;
		}
	}

	public LOD_Settings(float[] ranges)
	{
		this.Ranges = ranges;
	}

	public void Update(float quality)
	{
		if (this.Ranges.Length <= 0)
		{
			return;
		}
		if (Scene.Atmosphere && this.CaveMode != LOD_Settings.CaveModes.Always && (this.CaveMode == LOD_Settings.CaveModes.CaveOnly != Scene.Atmosphere.InACave || this.CaveMode == LOD_Settings.CaveModes.OutsideOnly == Scene.Atmosphere.InACave))
		{
			for (int i = 0; i < this.Ranges.Length; i++)
			{
				this.currentRanges[i] = 0f;
			}
			return;
		}
		float num = Mathf.Max(quality * this.QualityRatio.Evaluate((float)TheForestQualitySettings.UserSettings.DrawDistance), 0.3f);
		bool flag = (this.LowLodMode == LOD_Settings.LowLodModes.Always || TheForestQualitySettings.UserSettings.DrawDistance <= TheForestQualitySettings.DrawDistances.High) && this.Ranges.Length > 2;
		float num2 = this.StippleRange * num;
		for (int j = 0; j < this.Ranges.Length; j++)
		{
			switch (this.ScaleRanges[j])
			{
			case LOD_Settings.ScaleModes.ScaledQuality:
				this.currentQuality[j] = num;
				break;
			case LOD_Settings.ScaleModes.Quality:
				this.currentQuality[j] = quality;
				break;
			case LOD_Settings.ScaleModes.One:
				this.currentQuality[j] = 1f;
				break;
			}
			this.currentRanges[j] = this.GetRange(j);
			if (j == this.UpscaleToStippleRange && this.currentRanges[j] < this.currentRanges[j - 1] + num2 * 1.3f)
			{
				this.currentRanges[j] = this.currentRanges[j - 1] + num2 * 1.3f;
			}
			if (j == 2 && (this.currentRanges[j] < this.currentRanges[j - 1] + num2 || !flag))
			{
				flag = false;
				this.currentRanges[j] = -1f;
			}
		}
		float num3 = this.currentRanges[0];
		float num4 = this.currentRanges[1];
		float num5 = this.currentRanges[2];
		float num6;
		if (this.BillboardLowLodMode == LOD_Settings.BillboardLowLodModes.MatchLowMode && flag)
		{
			num6 = num5 - num2 * 0.1f;
		}
		else
		{
			num2 = Mathf.Clamp((num4 - num3) / (num2 * 2f), 0.5f, 1f) * num2;
			num6 = num4 - num2 * 0.2f;
		}
		float num7 = num6 - num2;
		float num8 = num7 + num2 * 0.2f;
		float num9 = num8 - num2;
		if (this.DrawDebug && LocalPlayer.Transform)
		{
			Debug.DrawLine(LocalPlayer.Transform.position + LocalPlayer.Transform.forward * num9 - Vector3.up, LocalPlayer.Transform.position + LocalPlayer.Transform.forward * num8 + Vector3.up, Color.blue);
			Debug.DrawLine(LocalPlayer.Transform.position + LocalPlayer.Transform.forward * num6 - Vector3.up, LocalPlayer.Transform.position + LocalPlayer.Transform.forward * num7 + Vector3.up, Color.red);
			Debug.DrawLine(LocalPlayer.Transform.position + LocalPlayer.Transform.forward * this.currentRanges[1], LocalPlayer.Transform.position + LocalPlayer.Transform.forward * this.currentRanges[2], Color.white);
			Debug.DrawLine(LocalPlayer.Transform.position + LocalPlayer.Transform.forward * this.currentRanges[0], LocalPlayer.Transform.position + LocalPlayer.Transform.forward * this.currentRanges[1], Color.cyan);
		}
		Vector2 lhs = new Vector2(num4 - num2 + num9 + num5, num4 + num8);
		bool flag2 = lhs != this.lastStippleRange || (this.Billboards != null && this.lastBillboardCount != this.Billboards.Length) || (this.StippledMaterials != null && this.lastStippledMaterialCount != this.StippledMaterials.Length);
		if (flag2)
		{
			if (this.Billboards != null)
			{
				CustomBillboard[] billboards = this.Billboards;
				for (int k = 0; k < billboards.Length; k++)
				{
					CustomBillboard customBillboard = billboards[k];
					customBillboard.FadeNearDistance = num9;
					customBillboard.FadeFarDistance = num8;
				}
			}
			if (this.StippledMaterials != null)
			{
				Material[] stippledMaterials = this.StippledMaterials;
				for (int l = 0; l < stippledMaterials.Length; l++)
				{
					Material material = stippledMaterials[l];
					material.SetFloat("_StippleNear", num7);
					material.SetFloat("_StippleFar", num6);
				}
			}
			this.lastStippleRange = lhs;
			this.lastBillboardCount = ((this.Billboards == null) ? 0 : this.Billboards.Length);
			this.lastStippledMaterialCount = ((this.StippledMaterials == null) ? 0 : this.StippledMaterials.Length);
		}
	}

	public float GetRange(int index)
	{
		float num;
		if (index >= 0 && index < this.Ranges.Length)
		{
			num = this.Ranges[index];
			num *= this.currentQuality[index];
		}
		else
		{
			num = -1f;
		}
		return num;
	}

	public int GetLOD(Vector3 position, int currentLOD)
	{
		if (this.Use2dDistance)
		{
			position.y = PlayerCamLocation.PlayerLoc.y;
		}
		float num = Vector3.Distance(position, PlayerCamLocation.PlayerLoc);
		int num2 = this.Ranges.Length;
		int result = num2;
		float num3 = LOD_Manager.Instance.Padding / 2f;
		int i = 0;
		while (i <= num2)
		{
			float num4 = (i != 0) ? this.currentRanges[i - 1] : (-num3);
			float num5 = (i != num2) ? this.currentRanges[i] : 3.40282347E+38f;
			float num6 = num - num4;
			float num7 = num5 - num;
			if (num6 >= 0f && num7 >= 0f)
			{
				if (Mathf.Abs(i - currentLOD) != 1)
				{
					result = i;
					break;
				}
				if (num6 >= num3 && num7 >= num3)
				{
					result = i;
					break;
				}
				result = currentLOD;
				break;
			}
			else
			{
				i++;
			}
		}
		return result;
	}
}
