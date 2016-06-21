using System;
using UnityEngine;

namespace TheForest.Graphics
{
	[AddComponentMenu("The Forest/Graphics/Water")]
	public class Water : CGUtility
	{
		private CGUtility.float4 SystemColor;

		private float SystemValue1;

		private float SystemValue2;

		private float SystemValue3;

		private float SystemValue4;

		private float SystemValue5;

		private float SystemValue6;

		private CGUtility.float4 SystemVector;

		private float SystemTime;

		private float DetailDistance;

		private float DetailMaxMipMap;

		private float Terrain_Mapping;

		private CGUtility.float4 TerrainPosition;

		private float TerrainResolution;

		private Texture2D TerrainFlowHeightmap;

		private float Flow;

		private float TerrainFlowPower;

		private float TerrainHeight;

		private float SystemCalmHeight;

		private Texture2D TerrainFog;

		private float FogDensity;

		private float FogPower;

		private float Displacement;

		private float WavesSlots;

		private float WavesCount;

		private Texture2D WavesHeightmap1;

		private Texture2D WavesHeightmap2;

		private Texture2D WavesHeightmap3;

		private Texture2D WavesNormalmap1;

		private Texture2D WavesNormalmap2;

		private Texture2D WavesNormalmap3;

		private float WavesNormalmapBump1;

		private float WavesNormalmapBump2;

		private float WavesNormalmapBump3;

		private CGUtility.float4 WavesTiling1;

		private CGUtility.float4 WavesTiling2;

		private CGUtility.float4 WavesTiling3;

		private CGUtility.float4 WavesRotation1;

		private CGUtility.float4 WavesRotation2;

		private CGUtility.float4 WavesRotation3;

		private CGUtility.float4 WavesAmplitude1;

		private CGUtility.float4 WavesAmplitude2;

		private CGUtility.float4 WavesAmplitude3;

		private CGUtility.float4 WavesSpeed1;

		private CGUtility.float4 WavesSpeed2;

		private CGUtility.float4 WavesSpeed3;

		private CGUtility.float4 WavesBump1;

		private CGUtility.float4 WavesBump2;

		private CGUtility.float4 WavesBump3;

		private CGUtility.float4 WavesFoam1;

		private CGUtility.float4 WavesFoam2;

		private CGUtility.float4 WavesFoam3;

		private float LOD;

		private CGUtility.float3 WorldPos;

		private CGUtility.float4 terrainData;

		private float CachedDisplacement;

		private float CachedWavesHeight;

		private float UnderWater;

		private CGUtility.float4 TransformPosition
		{
			get
			{
				return base.transform.position;
			}
		}

		private CGUtility.float4 TransformScale
		{
			get
			{
				return new CGUtility.float4(base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z, 1f);
			}
		}

		private CGUtility.float3 TransformUp
		{
			get
			{
				return new Vector3(0f, 0f, 1f);
			}
		}

		private CGUtility.float3 CameraPosition
		{
			get
			{
				if (WaterEngine.Camera)
				{
					return WaterEngine.CameraTransform.position;
				}
				return Vector3.zero;
			}
		}

		private float DETAIL_FADE
		{
			get
			{
				return this.LOD / this.DetailMaxMipMap;
			}
		}

		private bool IS_DETAIL
		{
			get
			{
				return this.DETAIL_FADE < 1f;
			}
		}

		private bool IS_DETAIL_ONE
		{
			get
			{
				return this.DETAIL_FADE < 0.66f;
			}
		}

		private bool IS_DETAIL_TWO
		{
			get
			{
				return this.DETAIL_FADE < 0.33f;
			}
		}

		private CGUtility.float4 TERRAIN_FLOW
		{
			get
			{
				return this.terrainData;
			}
		}

		private CGUtility.float4 TERRAIN_FLOW_XY
		{
			get
			{
				return new CGUtility.float4(this.TERRAIN_FLOW.xy, 0f, 0f);
			}
		}

		private float TERRAIN_BUMP
		{
			get
			{
				return this.terrainData.z;
			}
		}

		private float TERRAIN_LEVEL
		{
			get
			{
				return this.terrainData.w;
			}
		}

		private bool IS_UNDER_TERRAIN
		{
			get
			{
				return this.TERRAIN_LEVEL > this.WorldPos.y;
			}
		}

		public virtual Material SharedMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Material InstanceMaterial
		{
			get
			{
				return null;
			}
		}

		private void LoadProperties(Material material)
		{
			if (this.SystemColor.magnitude == 0f && this.SystemValue1 == 0f && this.SystemValue2 == 0f && this.SystemValue3 == 0f && this.SystemValue4 == 0f && this.SystemValue5 == 0f && this.SystemValue6 == 0f && this.SystemVector.magnitude == 0f && this.DetailDistance == 0f && this.DetailMaxMipMap == 0f)
			{
				this.Displacement = 0f;
			}
			if (this.Flow == 0f && this.TerrainFlowPower == 0f && this.FogDensity == 0f && this.FogPower == 0f)
			{
				this.Displacement = 0f;
			}
			if (this.Displacement == 0f && this.WavesFoam1.magnitude == 0f && this.WavesFoam2.magnitude == 0f && this.WavesFoam3.magnitude == 0f)
			{
				this.Displacement = 0f;
			}
			if (this.CachedDisplacement == 0f)
			{
				this.Displacement = 0f;
			}
			this.SystemColor = material.GetColor("SystemColor");
			this.SystemValue1 = material.GetFloat("SystemValue1");
			this.SystemValue2 = material.GetFloat("SystemValue2");
			this.SystemValue3 = material.GetFloat("SystemValue3");
			this.SystemValue4 = material.GetFloat("SystemValue4");
			this.SystemValue5 = material.GetFloat("SystemValue5");
			this.SystemValue6 = material.GetFloat("SystemValue6");
			this.SystemVector = material.GetVector("SystemVector");
			this.SystemTime = material.GetFloat("SystemTime");
			this.DetailDistance = material.GetFloat("DetailDistance");
			this.DetailMaxMipMap = material.GetFloat("DetailMaxMipMap");
			this.Terrain_Mapping = material.GetFloat("Terrain_Mapping");
			this.TerrainPosition = material.GetVector("TerrainPosition");
			this.TerrainResolution = material.GetFloat("TerrainResolution");
			this.TerrainFlowHeightmap = (Texture2D)material.GetTexture("TerrainFlowHeightmap");
			this.Flow = material.GetFloat("Flow");
			this.TerrainFlowPower = material.GetFloat("TerrainFlowPower");
			this.TerrainHeight = material.GetFloat("TerrainHeight");
			this.SystemCalmHeight = material.GetFloat("SystemCalmHeight");
			this.TerrainFog = (Texture2D)material.GetTexture("TerrainFog");
			this.FogDensity = material.GetFloat("FogDensity");
			this.FogPower = material.GetFloat("FogPower");
			this.Displacement = material.GetFloat("Displacement");
			this.WavesSlots = material.GetFloat("WavesSlots");
			this.WavesCount = material.GetFloat("WavesCount");
			this.WavesHeightmap1 = (Texture2D)material.GetTexture("WavesHeightmap1");
			this.WavesHeightmap2 = (Texture2D)material.GetTexture("WavesHeightmap2");
			this.WavesHeightmap3 = (Texture2D)material.GetTexture("WavesHeightmap3");
			this.WavesNormalmap1 = (Texture2D)material.GetTexture("WavesNormalmap1");
			this.WavesNormalmap2 = (Texture2D)material.GetTexture("WavesNormalmap2");
			this.WavesNormalmap3 = (Texture2D)material.GetTexture("WavesNormalmap3");
			this.WavesNormalmapBump1 = material.GetFloat("WavesNormalmapBump1");
			this.WavesNormalmapBump2 = material.GetFloat("WavesNormalmapBump2");
			this.WavesNormalmapBump3 = material.GetFloat("WavesNormalmapBump3");
			this.WavesTiling1 = material.GetVector("WavesTiling1");
			this.WavesTiling2 = material.GetVector("WavesTiling2");
			this.WavesTiling3 = material.GetVector("WavesTiling3");
			this.WavesRotation1 = material.GetVector("WavesRotation1");
			this.WavesRotation2 = material.GetVector("WavesRotation2");
			this.WavesRotation3 = material.GetVector("WavesRotation3");
			this.WavesAmplitude1 = material.GetVector("WavesAmplitude1");
			this.WavesAmplitude2 = material.GetVector("WavesAmplitude2");
			this.WavesAmplitude3 = material.GetVector("WavesAmplitude3");
			this.WavesSpeed1 = material.GetVector("WavesSpeed1");
			this.WavesSpeed2 = material.GetVector("WavesSpeed2");
			this.WavesSpeed3 = material.GetVector("WavesSpeed3");
			this.WavesBump1 = material.GetVector("WavesBump1");
			this.WavesBump2 = material.GetVector("WavesBump2");
			this.WavesBump3 = material.GetVector("WavesBump3");
			this.WavesFoam1 = material.GetVector("WavesFoam1");
			this.WavesFoam2 = material.GetVector("WavesFoam2");
			this.WavesFoam3 = material.GetVector("WavesFoam3");
		}

		private void WATER_BASE(CGUtility.float3 worldPos, float detail)
		{
			this.WorldPos = worldPos;
			this.LOD = detail * this.DetailMaxMipMap;
		}

		private CGUtility.float4 TerrainCoords()
		{
			CGUtility.float2 @float = this.WorldPos.xz;
			if (this.Terrain_Mapping == 1f)
			{
				CGUtility.float2 float2 = this.TransformScale.xz * this.TerrainResolution;
				@float = (@float - (this.TransformPosition.xz - float2 / 2f)) / float2;
			}
			else
			{
				@float = (@float - this.TerrainPosition.xz) / this.TerrainResolution;
			}
			return new CGUtility.float4(@float, 0f, this.LOD);
		}

		private void TERRAIN_DATA()
		{
			this.terrainData = base.tex2Dlod(this.TerrainFlowHeightmap, this.TerrainCoords());
			this.terrainData.xy = this.terrainData.xy * 2f - 1f;
			this.terrainData.w = this.terrainData.w * this.TerrainHeight;
		}

		private CGUtility.float4 TerrainFogColor()
		{
			return base.tex2Dlod(this.TerrainFog, this.TerrainCoords());
		}

		private float TerrainShore()
		{
			return base.saturate((this.WorldPos.y - this.TERRAIN_LEVEL) / this.SystemCalmHeight);
		}

		private bool UnderTerrain(CGUtility.float3 worldPos)
		{
			this.WATER_BASE(worldPos, base.saturate(base.distance(this.CameraPosition, worldPos) / this.DetailDistance));
			this.TERRAIN_DATA();
			return this.IS_UNDER_TERRAIN;
		}

		private void TerrainClip(CGUtility.float3 worldPos)
		{
			if (this.UnderTerrain(worldPos))
			{
				base.clip(0.95f - base.saturate((this.TERRAIN_LEVEL - this.WorldPos.y) / this.SystemCalmHeight));
			}
		}

		private float WavesMaxHeight()
		{
			float num = 0f;
			if (this.WavesSlots > 0f)
			{
				if (this.WavesCount > 0f)
				{
					num += this.WavesAmplitude1.x;
				}
				if (this.WavesCount > 1f)
				{
					num += this.WavesAmplitude1.y;
				}
				if (this.WavesCount > 2f)
				{
					num += this.WavesAmplitude1.z;
				}
				if (this.WavesCount > 3f)
				{
					num += this.WavesAmplitude1.w;
				}
			}
			if (this.WavesSlots > 1f)
			{
				if (this.WavesCount > 0f)
				{
					num += this.WavesAmplitude2.x;
				}
				if (this.WavesCount > 1f)
				{
					num += this.WavesAmplitude2.y;
				}
				if (this.WavesCount > 2f)
				{
					num += this.WavesAmplitude2.z;
				}
				if (this.WavesCount > 3f)
				{
					num += this.WavesAmplitude2.w;
				}
			}
			if (this.WavesSlots > 2f)
			{
				if (this.WavesCount > 0f)
				{
					num += this.WavesAmplitude3.x;
				}
				if (this.WavesCount > 1f)
				{
					num += this.WavesAmplitude3.y;
				}
				if (this.WavesCount > 2f)
				{
					num += this.WavesAmplitude3.z;
				}
				if (this.WavesCount > 3f)
				{
					num += this.WavesAmplitude3.w;
				}
			}
			return num;
		}

		private CGUtility.float4 WavesSample(Texture2D tex, float tiling, float rotation, float speed)
		{
			if (tiling == 0f)
			{
				return 0f;
			}
			CGUtility.float4 uv = new CGUtility.float4(this.WorldPos.xz, 0f, this.LOD);
			float num;
			float num2;
			base.sincos(base.radians(rotation), out num, out num2);
			uv.xy = new CGUtility.float2(uv.x * num2 - uv.y * num, uv.x * num + uv.y * num2);
			uv.x += speed * base._Time.y * this.SystemTime;
			uv.xy /= tiling;
			return base.tex2Dlod(tex, uv);
		}

		private float WavesHeight()
		{
			float num = 0f;
			if (this.WavesSlots > 0f)
			{
				if (this.WavesCount > 0f)
				{
					num += this.WavesSample(this.WavesHeightmap1, this.WavesTiling1.x, this.WavesRotation1.x, this.WavesSpeed1.x).r * this.WavesAmplitude1.x;
				}
				if (this.WavesCount > 1f)
				{
					num += this.WavesSample(this.WavesHeightmap1, this.WavesTiling1.y, this.WavesRotation1.y, this.WavesSpeed1.y).r * this.WavesAmplitude1.y;
				}
				if (this.WavesCount > 2f)
				{
					num += this.WavesSample(this.WavesHeightmap1, this.WavesTiling1.z, this.WavesRotation1.z, this.WavesSpeed1.z).r * this.WavesAmplitude1.z;
				}
				if (this.WavesCount > 3f)
				{
					num += this.WavesSample(this.WavesHeightmap1, this.WavesTiling1.w, this.WavesRotation1.w, this.WavesSpeed1.w).r * this.WavesAmplitude1.w;
				}
			}
			if (this.WavesSlots > 1f && this.IS_DETAIL_ONE)
			{
				if (this.WavesCount > 0f)
				{
					num += this.WavesSample(this.WavesHeightmap2, this.WavesTiling2.x, this.WavesRotation2.x, this.WavesSpeed2.x).r * this.WavesAmplitude2.x;
				}
				if (this.WavesCount > 1f)
				{
					num += this.WavesSample(this.WavesHeightmap2, this.WavesTiling2.y, this.WavesRotation2.y, this.WavesSpeed2.y).r * this.WavesAmplitude2.y;
				}
				if (this.WavesCount > 2f)
				{
					num += this.WavesSample(this.WavesHeightmap2, this.WavesTiling2.z, this.WavesRotation2.z, this.WavesSpeed2.z).r * this.WavesAmplitude2.z;
				}
				if (this.WavesCount > 3f)
				{
					num += this.WavesSample(this.WavesHeightmap2, this.WavesTiling2.w, this.WavesRotation2.w, this.WavesSpeed2.w).r * this.WavesAmplitude2.w;
				}
			}
			if (this.WavesSlots > 2f && this.IS_DETAIL_TWO)
			{
				if (this.WavesCount > 0f)
				{
					num += this.WavesSample(this.WavesHeightmap3, this.WavesTiling3.x, this.WavesRotation3.x, this.WavesSpeed3.x).r * this.WavesAmplitude3.x;
				}
				if (this.WavesCount > 1f)
				{
					num += this.WavesSample(this.WavesHeightmap3, this.WavesTiling3.y, this.WavesRotation3.y, this.WavesSpeed3.y).r * this.WavesAmplitude3.y;
				}
				if (this.WavesCount > 2f)
				{
					num += this.WavesSample(this.WavesHeightmap3, this.WavesTiling3.z, this.WavesRotation3.z, this.WavesSpeed3.z).r * this.WavesAmplitude3.z;
				}
				if (this.WavesCount > 3f)
				{
					num += this.WavesSample(this.WavesHeightmap3, this.WavesTiling3.w, this.WavesRotation3.w, this.WavesSpeed3.w).r * this.WavesAmplitude3.w;
				}
			}
			if (this.TERRAIN_LEVEL != 0f)
			{
				num *= this.TerrainShore();
			}
			return num;
		}

		private CGUtility.float3 UnpackBump(CGUtility.float4 normal, float bump)
		{
			if (base.any(normal.xyz) && bump != 0f)
			{
				normal.xyz = base.UnpackNormal(normal).xyz;
				normal.xy *= bump;
				return base.normalize(normal.xyz);
			}
			return this.TransformUp;
		}

		private CGUtility.float3 WavesNormal()
		{
			CGUtility.float3 @float = this.TransformUp;
			if (this.WavesSlots > 0f)
			{
				if (this.WavesCount > 0f)
				{
					@float = this.UnpackBump(this.WavesSample(this.WavesNormalmap1, this.WavesTiling1.x, this.WavesRotation1.x, this.WavesSpeed1.x), this.WavesBump1.x * this.WavesNormalmapBump1);
				}
				if (this.WavesCount > 1f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap1, this.WavesTiling1.y, this.WavesRotation1.y, this.WavesSpeed1.y), this.WavesBump1.y * this.WavesNormalmapBump1));
				}
				if (this.WavesCount > 2f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap1, this.WavesTiling1.z, this.WavesRotation1.z, this.WavesSpeed1.z), this.WavesBump1.z * this.WavesNormalmapBump1));
				}
				if (this.WavesCount > 3f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap1, this.WavesTiling1.w, this.WavesRotation1.w, this.WavesSpeed1.w), this.WavesBump1.w * this.WavesNormalmapBump1));
				}
			}
			if (this.WavesSlots > 1f && this.IS_DETAIL_ONE)
			{
				if (this.WavesCount > 0f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap2, this.WavesTiling2.x, this.WavesRotation2.x, this.WavesSpeed2.x), this.WavesBump2.x * this.WavesNormalmapBump2));
				}
				if (this.WavesCount > 1f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap2, this.WavesTiling2.y, this.WavesRotation2.y, this.WavesSpeed2.y), this.WavesBump2.y * this.WavesNormalmapBump2));
				}
				if (this.WavesCount > 2f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap2, this.WavesTiling2.z, this.WavesRotation2.z, this.WavesSpeed2.z), this.WavesBump2.z * this.WavesNormalmapBump2));
				}
				if (this.WavesCount > 3f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap2, this.WavesTiling2.w, this.WavesRotation2.w, this.WavesSpeed2.w), this.WavesBump2.w * this.WavesNormalmapBump2));
				}
			}
			if (this.WavesSlots > 2f && this.IS_DETAIL_TWO)
			{
				if (this.WavesCount > 0f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap3, this.WavesTiling3.x, this.WavesRotation3.x, this.WavesSpeed3.x), this.WavesBump3.x * this.WavesNormalmapBump3));
				}
				if (this.WavesCount > 1f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap3, this.WavesTiling3.y, this.WavesRotation3.y, this.WavesSpeed3.y), this.WavesBump3.y * this.WavesNormalmapBump3));
				}
				if (this.WavesCount > 2f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap3, this.WavesTiling3.z, this.WavesRotation3.z, this.WavesSpeed3.z), this.WavesBump3.z * this.WavesNormalmapBump3));
				}
				if (this.WavesCount > 3f)
				{
					@float = base.BlendNormals(@float, this.UnpackBump(this.WavesSample(this.WavesNormalmap3, this.WavesTiling3.w, this.WavesRotation3.w, this.WavesSpeed3.w), this.WavesBump3.w * this.WavesNormalmapBump3));
				}
			}
			if (this.TERRAIN_LEVEL != 0f && this.TERRAIN_LEVEL > this.WorldPos.y - this.SystemCalmHeight)
			{
				@float = base.lerp(this.TransformUp, @float, this.TerrainShore());
			}
			@float = base.lerp(@float, this.TransformUp, this.TERRAIN_BUMP);
			return base.normalize(@float);
		}

		private void CameraDisplacement(float height)
		{
			this.CachedWavesHeight = height;
			this.CachedDisplacement = height / base.max(0.01f, this.WavesMaxHeight());
		}

		private void CameraUnderWater(CGUtility.float3 CameraPos)
		{
			this.UnderWater = base.saturate(base.sign(CameraPos.y - this.TransformPosition.y - this.CachedWavesHeight)) * 2f - 1f;
		}

		public float HeightAt(Vector3 position)
		{
			if (this.SharedMaterial)
			{
				return this.HeightAt(position, this.SharedMaterial);
			}
			return position.y;
		}

		public float HeightAt(Vector3 position, Material material)
		{
			position.y = base.transform.position.y;
			if (material && material.HasProperty("Displacement") && material.GetFloat("Displacement") == 0f)
			{
				this.LoadProperties(material);
				this.WATER_BASE(position, 0f);
				this.TERRAIN_DATA();
				this.CameraDisplacement(this.WavesHeight());
				float cachedWavesHeight = this.CachedWavesHeight;
				if (!float.IsNaN(cachedWavesHeight))
				{
					return base.transform.position.y + cachedWavesHeight;
				}
			}
			return position.y;
		}

		public Vector3 AdjustHeightAt(Vector3 position)
		{
			if (this.SharedMaterial)
			{
				return this.AdjustHeightAt(position, this.SharedMaterial);
			}
			return position;
		}

		public Vector3 AdjustHeightAt(Vector3 position, Material material)
		{
			position.y = this.HeightAt(position, material);
			return position;
		}

		public float MaxHeight()
		{
			if (this.SharedMaterial)
			{
				return this.MaxHeight(this.SharedMaterial);
			}
			return base.transform.position.y;
		}

		public float MaxHeight(Material material)
		{
			if (material && material.HasProperty("Displacement") && material.GetFloat("Displacement") == 0f)
			{
				this.LoadProperties(material);
				return base.transform.position.y + this.WavesMaxHeight();
			}
			return base.transform.position.y;
		}

		public Vector3 NormalAt(Vector3 position)
		{
			if (this.SharedMaterial)
			{
				return this.NormalAt(position, this.SharedMaterial);
			}
			return Vector3.zero;
		}

		public Vector3 NormalAt(Vector3 position, Material material)
		{
			position.y = base.transform.position.y;
			if (material && material.HasProperty("Displacement"))
			{
				this.WATER_BASE(position, 0f);
				this.TERRAIN_DATA();
				this.CameraDisplacement(this.WavesHeight());
				CGUtility.float3 @float = this.WavesNormal();
				if (!float.IsNaN(@float.x) && !float.IsNaN(@float.y) && !float.IsNaN(@float.z))
				{
					return base.transform.rotation * new Vector3(@float.x, @float.z, @float.y) * this.UnderWater;
				}
			}
			return Vector3.zero;
		}

		public bool IsUnderWater(Vector3 position)
		{
			if (this.SharedMaterial)
			{
				return this.IsUnderWater(position, this.SharedMaterial);
			}
			return position.y <= base.transform.position.y;
		}

		public bool IsUnderWater(Vector3 position, Material material)
		{
			if (material && material.HasProperty("Displacement") && material.GetFloat("Displacement") == 0f)
			{
				this.LoadProperties(material);
				this.WATER_BASE(position, 0f);
				this.TERRAIN_DATA();
				this.CameraDisplacement(this.WavesHeight());
				this.CameraUnderWater(position);
				return this.UnderWater == -1f;
			}
			return position.y <= base.transform.position.y;
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void Update()
		{
		}

		protected virtual void OnWillRenderObject()
		{
		}

		protected virtual void OnDrawGizmos()
		{
		}
	}
}
