using Ceto.Common.Containers.Interpolation;
using Ceto.Common.Containers.Queues;
using Ceto.Common.Threading.Scheduling;
using Ceto.Common.Threading.Tasks;
using Ceto.Common.Unity.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Components/WaveSpectrum"), DisallowMultipleComponent, RequireComponent(typeof(Ocean))]
	public class WaveSpectrum : WaveSpectrumBase
	{
		private struct BufferSettings
		{
			public bool beenCreated;

			public bool isCpu;

			public int size;
		}

		public const float MAX_CHOPPYNESS = 1.2f;

		public const float MAX_FOAM_AMOUNT = 6f;

		public const float MAX_FOAM_COVERAGE = 0.5f;

		public const float MAX_WIND_SPEED = 30f;

		public const float MIN_WAVE_AGE = 0.5f;

		public const float MAX_WAVE_AGE = 1f;

		public const float MAX_WAVE_SPEED = 10f;

		public const float MIN_GRID_SCALE = 0.1f;

		public const float MAX_GRID_SCALE = 1f;

		public const float MAX_WAVE_SMOOTHING = 6f;

		public const float MIN_WAVE_SMOOTHING = 1f;

		public const float MAX_SLOPE_SMOOTHING = 6f;

		public const float MIN_SLOPE_SMOOTHING = 1f;

		public const float MAX_FOAM_SMOOTHING = 6f;

		public const float MIN_FOAM_SMOOTHING = 1f;

		public FOURIER_SIZE fourierSize = FOURIER_SIZE.MEDIUM_64_CPU;

		public SPECTRUM_TYPE spectrumType;

		[Range(1f, 4f)]
		public int numberOfGrids = 4;

		public bool disableReadBack = true;

		public bool disableDisplacements;

		public bool disableSlopes;

		public bool disableFoam;

		public bool textureFoam = true;

		[Range(0f, 1.2f)]
		public float choppyness = 0.8f;

		[Range(0f, 6f)]
		public float foamAmount = 1f;

		[Range(0f, 0.5f)]
		public float foamCoverage = 0.1f;

		[Range(0f, 30f)]
		public float windSpeed = 8f;

		[Range(0.5f, 1f)]
		public float waveAge = 0.64f;

		[Range(0f, 10f)]
		public float waveSpeed = 1f;

		[Range(0.1f, 1f)]
		public float gridScale = 0.5f;

		[Range(1f, 6f)]
		public float waveSmoothing = 2f;

		[Range(1f, 6f)]
		private float slopeSmoothing = 1f;

		[Range(1f, 6f)]
		private float foamSmoothing = 2f;

		private RenderTexture[] m_displacementMaps;

		private RenderTexture[] m_slopeMaps;

		private RenderTexture[] m_foamMaps;

		private Material m_slopeCopyMat;

		private Material m_displacementCopyMat;

		private Material m_foamCopyMat;

		private Material m_slopeInitMat;

		private Material m_displacementInitMat;

		private Material m_foamInitMat;

		private Vector4 m_gridSizes = Vector4.one;

		private Vector4 m_choppyness = Vector4.one;

		private Scheduler m_scheduler;

		private WaveSpectrumCondition[] m_conditions;

		private WaveSpectrumBuffer m_displacementBuffer;

		private WaveSpectrumBuffer m_slopeBuffer;

		private WaveSpectrumBuffer m_jacobianBuffer;

		private FindRangeTask m_findRangeTask;

		private ComputeBuffer m_readBuffer;

		private QueryGridScaling m_queryScaling = new QueryGridScaling();

		private WaveSpectrum.BufferSettings m_bufferSettings = default(WaveSpectrum.BufferSettings);

		private DictionaryQueue<WaveSpectrumConditionKey, WaveSpectrumCondition> m_conditionCache;

		private int m_maxConditionCacheSize = 10;

		[HideInInspector]
		public Shader initSlopeSdr;

		[HideInInspector]
		public Shader initDisplacementSdr;

		[HideInInspector]
		public Shader initJacobianSdr;

		[HideInInspector]
		public Shader fourierSdr;

		[HideInInspector]
		public Shader slopeCopySdr;

		[HideInInspector]
		public Shader displacementCopySdr;

		[HideInInspector]
		public Shader foamCopySdr;

		[HideInInspector]
		public ComputeShader readSdr;

		public override bool DisableReadBack
		{
			get
			{
				return this.disableReadBack;
			}
		}

		public override float GridScale
		{
			get
			{
				return this.gridScale;
			}
		}

		public IList<RenderTexture> DisplacementMaps
		{
			get
			{
				return this.m_displacementMaps;
			}
		}

		public IList<RenderTexture> SlopeMaps
		{
			get
			{
				return this.m_slopeMaps;
			}
		}

		public IList<RenderTexture> FoamMaps
		{
			get
			{
				return this.m_foamMaps;
			}
		}

		public override Vector2 MaxDisplacement
		{
			get;
			set;
		}

		public bool IsCreatingNewCondition
		{
			get
			{
				return this.m_conditions[1] != null;
			}
		}

		public override Vector4 GridSizes
		{
			get
			{
				return this.m_gridSizes;
			}
		}

		public override Vector4 Choppyness
		{
			get
			{
				return this.m_choppyness;
			}
		}

		public int MaxConditionCacheSize
		{
			set
			{
				this.m_maxConditionCacheSize = value;
			}
		}

		public override IDisplacementBuffer DisplacementBuffer
		{
			get
			{
				if (this.m_displacementBuffer == null)
				{
					return null;
				}
				if (!(this.m_displacementBuffer is IDisplacementBuffer))
				{
					throw new InvalidCastException("Displacement buffer cast exception");
				}
				return this.m_displacementBuffer as IDisplacementBuffer;
			}
		}

		private void Start()
		{
			try
			{
				Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_FoamMap0", Texture2D.blackTexture);
				this.m_slopeCopyMat = new Material(this.slopeCopySdr);
				this.m_displacementCopyMat = new Material(this.displacementCopySdr);
				this.m_foamCopyMat = new Material(this.foamCopySdr);
				this.m_slopeInitMat = new Material(this.initSlopeSdr);
				this.m_displacementInitMat = new Material(this.initDisplacementSdr);
				this.m_foamInitMat = new Material(this.initJacobianSdr);
				this.m_conditionCache = new DictionaryQueue<WaveSpectrumConditionKey, WaveSpectrumCondition>();
				this.m_scheduler = new Scheduler();
				this.CreateBuffers();
				this.CreateRenderTextures();
				this.CreateConditions();
				this.UpdateQueryScaling();
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Shader.DisableKeyword("CETO_USE_4_SPECTRUM_GRIDS");
			Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_FoamMap0", Texture2D.blackTexture);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			try
			{
				if (this.m_scheduler != null)
				{
					this.m_scheduler.ShutingDown = true;
					this.m_scheduler.CancelAllTasks();
				}
				if (this.m_conditionCache != null)
				{
					foreach (WaveSpectrumCondition current in this.m_conditionCache)
					{
						current.Release();
					}
					this.m_conditionCache.Clear();
					this.m_conditionCache = null;
				}
				this.Release();
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		private void Release()
		{
			if (this.m_displacementBuffer != null)
			{
				this.m_displacementBuffer.Release();
				this.m_displacementBuffer = null;
			}
			if (this.m_slopeBuffer != null)
			{
				this.m_slopeBuffer.Release();
				this.m_slopeBuffer = null;
			}
			if (this.m_jacobianBuffer != null)
			{
				this.m_jacobianBuffer.Release();
				this.m_jacobianBuffer = null;
			}
			if (this.m_readBuffer != null)
			{
				this.m_readBuffer.Release();
				this.m_readBuffer = null;
			}
			if (this.m_conditions != null && this.m_conditions[0] != null && this.m_conditions[0].Done)
			{
				this.CacheCondition(this.m_conditions[0]);
				if (this.m_conditionCache == null || !this.m_conditionCache.ContainsKey(this.m_conditions[0].Key))
				{
					this.m_conditions[0].Release();
					this.m_conditions[0] = null;
				}
			}
			if (this.m_conditions != null && this.m_conditions[1] != null && this.m_conditions[1].Done && (this.m_conditionCache == null || !this.m_conditionCache.ContainsKey(this.m_conditions[1].Key)))
			{
				this.m_conditions[1].Release();
				this.m_conditions[1] = null;
			}
			this.m_conditions = null;
			this.m_findRangeTask = null;
			RTUtility.ReleaseAndDestroy(this.m_displacementMaps);
			this.m_displacementMaps = null;
			RTUtility.ReleaseAndDestroy(this.m_slopeMaps);
			this.m_slopeMaps = null;
			RTUtility.ReleaseAndDestroy(this.m_foamMaps);
			this.m_foamMaps = null;
		}

		private void Update()
		{
			try
			{
				this.gridScale = Mathf.Clamp(this.gridScale, 0.1f, 1f);
				this.windSpeed = Mathf.Clamp(this.windSpeed, 0f, 30f);
				this.waveAge = Mathf.Clamp(this.waveAge, 0.5f, 1f);
				this.waveSpeed = Mathf.Clamp(this.waveSpeed, 0f, 10f);
				this.foamAmount = Mathf.Clamp(this.foamAmount, 0f, 6f);
				this.foamCoverage = Mathf.Clamp(this.foamCoverage, 0f, 0.5f);
				this.waveSmoothing = Mathf.Clamp(this.waveSmoothing, 1f, 6f);
				this.slopeSmoothing = Mathf.Clamp(this.slopeSmoothing, 1f, 6f);
				this.foamSmoothing = Mathf.Clamp(this.foamSmoothing, 1f, 6f);
				float time = this.m_ocean.OceanTime.Now * this.waveSpeed;
				this.CreateBuffers();
				this.CreateRenderTextures();
				this.CreateConditions();
				int numGrids = this.m_conditions[0].Key.NumGrids;
				if (numGrids > 2)
				{
					Shader.EnableKeyword("CETO_USE_4_SPECTRUM_GRIDS");
				}
				else
				{
					Shader.DisableKeyword("CETO_USE_4_SPECTRUM_GRIDS");
				}
				this.UpdateQueryScaling();
				Shader.SetGlobalVector("Ceto_GridSizes", this.GridSizes);
				Shader.SetGlobalVector("Ceto_GridScale", new Vector2(this.GridScale, this.GridScale));
				Shader.SetGlobalVector("Ceto_Choppyness", this.Choppyness);
				Shader.SetGlobalFloat("Ceto_MapSize", (float)this.m_bufferSettings.size);
				Shader.SetGlobalFloat("Ceto_WaveSmoothing", this.waveSmoothing);
				Shader.SetGlobalFloat("Ceto_SlopeSmoothing", this.slopeSmoothing);
				Shader.SetGlobalFloat("Ceto_FoamSmoothing", this.foamSmoothing);
				Shader.SetGlobalFloat("Ceto_TextureWaveFoam", (!this.textureFoam) ? 0f : 1f);
				this.UpdateSpectrumScheduler();
				this.GenerateDisplacement(time);
				this.GenerateSlopes(time);
				this.GenerateFoam(time);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		private void UpdateSpectrumScheduler()
		{
			try
			{
				this.m_scheduler.DisableMultithreading = Ocean.DISABLE_ALL_MULTITHREADING;
				this.m_scheduler.CheckForException();
				this.m_scheduler.Update();
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		private void UpdateQueryScaling()
		{
			this.m_choppyness = this.m_conditions[0].Choppyness * Mathf.Clamp(this.choppyness, 0f, 1.2f);
			this.m_gridSizes = this.m_conditions[0].GridSizes;
			Vector4 invGridSizes = default(Vector4);
			invGridSizes.x = 1f / (this.GridSizes.x * this.GridScale);
			invGridSizes.y = 1f / (this.GridSizes.y * this.GridScale);
			invGridSizes.z = 1f / (this.GridSizes.z * this.GridScale);
			invGridSizes.w = 1f / (this.GridSizes.w * this.GridScale);
			this.m_queryScaling.invGridSizes = invGridSizes;
			this.m_queryScaling.scaleY = this.GridScale;
			this.m_queryScaling.choppyness = this.Choppyness * this.GridScale;
			this.m_queryScaling.offset = this.m_ocean.PositionOffset;
			this.m_queryScaling.numGrids = this.m_conditions[0].Key.NumGrids;
			if (this.m_queryScaling.tmp == null)
			{
				this.m_queryScaling.tmp = new float[QueryDisplacements.CHANNELS];
			}
		}

		private void GenerateSlopes(float time)
		{
			if (!this.disableSlopes && SystemInfo.graphicsShaderLevel < 30)
			{
				Ocean.LogWarning("Spectrum slopes needs at least SM3 to run. Disabling slopes.");
				this.disableSlopes = true;
			}
			if (this.disableSlopes)
			{
				this.m_slopeBuffer.DisableBuffer(-1);
			}
			else
			{
				this.m_slopeBuffer.EnableBuffer(-1);
			}
			if (this.m_slopeBuffer.EnabledBuffers() == 0)
			{
				Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
			}
			else
			{
				int numGrids = this.m_conditions[0].Key.NumGrids;
				if (numGrids <= 2)
				{
					this.m_slopeBuffer.DisableBuffer(1);
				}
				if (!this.m_slopeBuffer.HasRun || this.m_slopeBuffer.TimeValue != time)
				{
					this.m_slopeBuffer.InitMaterial = this.m_slopeInitMat;
					this.m_slopeBuffer.InitPass = numGrids - 1;
					this.m_slopeBuffer.Run(this.m_conditions[0], time);
				}
				if (!this.m_slopeBuffer.BeenSampled)
				{
					this.m_slopeBuffer.EnableSampling();
					if (numGrids > 0)
					{
						this.m_slopeCopyMat.SetTexture("Ceto_SlopeBuffer", this.m_slopeBuffer.GetTexture(0));
						Graphics.Blit(null, this.m_slopeMaps[0], this.m_slopeCopyMat, 0);
						Shader.SetGlobalTexture("Ceto_SlopeMap0", this.m_slopeMaps[0]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
					}
					if (numGrids > 2)
					{
						this.m_slopeCopyMat.SetTexture("Ceto_SlopeBuffer", this.m_slopeBuffer.GetTexture(1));
						Graphics.Blit(null, this.m_slopeMaps[1], this.m_slopeCopyMat, 0);
						Shader.SetGlobalTexture("Ceto_SlopeMap1", this.m_slopeMaps[1]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
					}
					this.m_slopeBuffer.DisableSampling();
					this.m_slopeBuffer.BeenSampled = true;
				}
			}
		}

		private void GenerateDisplacement(float time)
		{
			if (!this.disableDisplacements && SystemInfo.graphicsShaderLevel < 30 && this.m_displacementBuffer.IsGPU)
			{
				Ocean.LogWarning("Spectrum displacements needs at least SM3 to run on GPU. Disabling displacement.");
				this.disableDisplacements = true;
			}
			this.m_displacementBuffer.EnableBuffer(-1);
			if (this.disableDisplacements)
			{
				this.m_displacementBuffer.DisableBuffer(-1);
			}
			if (!this.disableDisplacements && this.choppyness == 0f)
			{
				this.m_displacementBuffer.DisableBuffer(1);
				this.m_displacementBuffer.DisableBuffer(2);
			}
			if (!this.disableDisplacements && this.choppyness > 0f)
			{
				this.m_displacementBuffer.EnableBuffer(1);
				this.m_displacementBuffer.EnableBuffer(2);
			}
			if (this.m_displacementBuffer.EnabledBuffers() == 0)
			{
				Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
				return;
			}
			if (this.m_displacementBuffer.Done)
			{
				int numGrids = this.m_conditions[0].Key.NumGrids;
				if (numGrids <= 2)
				{
					this.m_displacementBuffer.DisableBuffer(2);
				}
				if (!this.m_displacementBuffer.HasRun || this.m_displacementBuffer.TimeValue != time)
				{
					this.m_displacementBuffer.InitMaterial = this.m_displacementInitMat;
					this.m_displacementBuffer.InitPass = numGrids - 1;
					this.m_displacementBuffer.Run(this.m_conditions[0], time);
				}
				if (!this.m_displacementBuffer.BeenSampled)
				{
					this.m_displacementBuffer.EnableSampling();
					this.m_displacementCopyMat.SetTexture("Ceto_HeightBuffer", this.m_displacementBuffer.GetTexture(0));
					this.m_displacementCopyMat.SetTexture("Ceto_DisplacementBuffer", this.m_displacementBuffer.GetTexture(1));
					if (numGrids > 0)
					{
						Graphics.Blit(null, this.m_displacementMaps[0], this.m_displacementCopyMat, (numGrids != 1) ? 0 : 4);
						Shader.SetGlobalTexture("Ceto_DisplacementMap0", this.m_displacementMaps[0]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
					}
					if (numGrids > 1)
					{
						Graphics.Blit(null, this.m_displacementMaps[1], this.m_displacementCopyMat, 1);
						Shader.SetGlobalTexture("Ceto_DisplacementMap1", this.m_displacementMaps[1]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
					}
					this.m_displacementCopyMat.SetTexture("Ceto_DisplacementBuffer", this.m_displacementBuffer.GetTexture(2));
					if (numGrids > 2)
					{
						Graphics.Blit(null, this.m_displacementMaps[2], this.m_displacementCopyMat, 2);
						Shader.SetGlobalTexture("Ceto_DisplacementMap2", this.m_displacementMaps[2]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
					}
					if (numGrids > 3)
					{
						Graphics.Blit(null, this.m_displacementMaps[3], this.m_displacementCopyMat, 3);
						Shader.SetGlobalTexture("Ceto_DisplacementMap3", this.m_displacementMaps[3]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
					}
					this.m_displacementBuffer.DisableSampling();
					this.m_displacementBuffer.BeenSampled = true;
					if (this.m_displacementBuffer.IsGPU)
					{
						this.ReadFromGPU(numGrids);
					}
					this.FindRanges();
				}
			}
		}

		private void ReadFromGPU(int numGrids)
		{
			if (!this.disableReadBack && this.readSdr == null)
			{
				Ocean.LogWarning("Trying to read GPU displacement data but the read shader is null");
			}
			bool flag = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			if (!this.disableReadBack && this.readSdr != null && this.m_readBuffer != null && flag)
			{
				InterpolatedArray2f[] readDisplacements = this.DisplacementBuffer.GetReadDisplacements();
				if (numGrids > 0)
				{
					CBUtility.ReadFromRenderTexture(this.m_displacementMaps[0], 3, this.m_readBuffer, this.readSdr);
					this.m_readBuffer.GetData(readDisplacements[0].Data);
				}
				else
				{
					readDisplacements[0].Clear();
				}
				if (numGrids > 1)
				{
					CBUtility.ReadFromRenderTexture(this.m_displacementMaps[1], 3, this.m_readBuffer, this.readSdr);
					this.m_readBuffer.GetData(readDisplacements[1].Data);
				}
				else
				{
					readDisplacements[1].Clear();
				}
				if (numGrids > 2)
				{
					CBUtility.ReadFromRenderTexture(this.m_displacementMaps[2], 3, this.m_readBuffer, this.readSdr);
					this.m_readBuffer.GetData(readDisplacements[2].Data);
				}
				else
				{
					readDisplacements[2].Clear();
				}
				if (numGrids > 3)
				{
				}
			}
		}

		private void FindRanges()
		{
			if (this.disableReadBack && this.DisplacementBuffer.IsGPU)
			{
				this.MaxDisplacement = new Vector2(0f, 40f * this.gridScale);
			}
			else if (this.m_findRangeTask == null || this.m_findRangeTask.Done)
			{
				if (this.m_findRangeTask == null)
				{
					this.m_findRangeTask = new FindRangeTask(this);
				}
				this.m_findRangeTask.Reset();
				this.m_scheduler.Run(this.m_findRangeTask);
			}
		}

		private void GenerateFoam(float time)
		{
			Vector4 vector = this.Choppyness;
			if (!this.disableFoam && SystemInfo.graphicsShaderLevel < 30)
			{
				Ocean.LogWarning("Spectrum foam needs at least SM3 to run. Disabling foam.");
				this.disableFoam = true;
			}
			float sqrMagnitude = vector.sqrMagnitude;
			this.m_jacobianBuffer.EnableBuffer(-1);
			if (this.disableFoam || this.foamAmount == 0f || sqrMagnitude == 0f || !this.m_conditions[0].SupportsJacobians)
			{
				this.m_jacobianBuffer.DisableBuffer(-1);
			}
			if (this.m_jacobianBuffer.EnabledBuffers() == 0)
			{
				Shader.SetGlobalTexture("Ceto_FoamMap0", Texture2D.blackTexture);
			}
			else
			{
				int numGrids = this.m_conditions[0].Key.NumGrids;
				if (numGrids == 1)
				{
					this.m_jacobianBuffer.DisableBuffer(1);
					this.m_jacobianBuffer.DisableBuffer(2);
				}
				else if (numGrids == 2)
				{
					this.m_jacobianBuffer.DisableBuffer(2);
				}
				if (!this.m_jacobianBuffer.HasRun || this.m_jacobianBuffer.TimeValue != time)
				{
					this.m_foamInitMat.SetFloat("Ceto_FoamAmount", this.foamAmount);
					this.m_jacobianBuffer.InitMaterial = this.m_foamInitMat;
					this.m_jacobianBuffer.InitPass = numGrids - 1;
					this.m_jacobianBuffer.Run(this.m_conditions[0], time);
				}
				if (!this.m_jacobianBuffer.BeenSampled)
				{
					this.m_jacobianBuffer.EnableSampling();
					this.m_foamCopyMat.SetTexture("Ceto_JacobianBuffer0", this.m_jacobianBuffer.GetTexture(0));
					this.m_foamCopyMat.SetTexture("Ceto_JacobianBuffer1", this.m_jacobianBuffer.GetTexture(1));
					this.m_foamCopyMat.SetTexture("Ceto_JacobianBuffer2", this.m_jacobianBuffer.GetTexture(2));
					this.m_foamCopyMat.SetTexture("Ceto_HeightBuffer", this.m_displacementBuffer.GetTexture(0));
					this.m_foamCopyMat.SetVector("Ceto_FoamChoppyness", vector);
					this.m_foamCopyMat.SetFloat("Ceto_FoamCoverage", this.foamCoverage);
					Graphics.Blit(null, this.m_foamMaps[0], this.m_foamCopyMat, numGrids - 1);
					Shader.SetGlobalTexture("Ceto_FoamMap0", this.m_foamMaps[0]);
					this.m_jacobianBuffer.DisableSampling();
					this.m_jacobianBuffer.BeenSampled = true;
				}
			}
		}

		public override void QueryWaves(WaveQuery query)
		{
			if (!base.enabled)
			{
				return;
			}
			IDisplacementBuffer displacementBuffer = this.DisplacementBuffer;
			if (displacementBuffer == null)
			{
				return;
			}
			if (this.disableReadBack && displacementBuffer.IsGPU)
			{
				return;
			}
			if (query.mode != QUERY_MODE.DISPLACEMENT && query.mode != QUERY_MODE.POSITION)
			{
				return;
			}
			if (!query.sampleSpectrum[0] && !query.sampleSpectrum[1] && !query.sampleSpectrum[2] && !query.sampleSpectrum[3])
			{
				return;
			}
			displacementBuffer.QueryWaves(query, this.m_queryScaling);
		}

		private void CreateBuffers()
		{
			int num;
			bool flag;
			this.GetFourierSize(out num, out flag);
			if (this.m_bufferSettings.beenCreated)
			{
				if (this.m_bufferSettings.size == num && this.m_bufferSettings.isCpu == flag)
				{
					return;
				}
				while (this.m_scheduler.HasTasks())
				{
					this.UpdateSpectrumScheduler();
				}
				this.Release();
				this.m_bufferSettings.beenCreated = false;
			}
			if (flag)
			{
				this.m_displacementBuffer = new DisplacementBufferCPU(num, this.m_scheduler);
			}
			else
			{
				this.m_displacementBuffer = new DisplacementBufferGPU(num, this.fourierSdr);
			}
			this.m_slopeBuffer = new WaveSpectrumBufferGPU(num, this.fourierSdr, 2);
			this.m_jacobianBuffer = new WaveSpectrumBufferGPU(num, this.fourierSdr, 3);
			this.m_readBuffer = new ComputeBuffer(num * num, 12);
			this.m_conditions = new WaveSpectrumCondition[2];
			this.m_displacementMaps = new RenderTexture[4];
			this.m_slopeMaps = new RenderTexture[2];
			this.m_foamMaps = new RenderTexture[1];
			this.m_bufferSettings.beenCreated = true;
			this.m_bufferSettings.size = num;
			this.m_bufferSettings.isCpu = flag;
		}

		private void CreateRenderTextures()
		{
			int size = this.m_bufferSettings.size;
			int ansio = 9;
			RenderTextureFormat format = RenderTextureFormat.ARGBFloat;
			for (int i = 0; i < this.m_displacementMaps.Length; i++)
			{
				this.CreateMap(ref this.m_displacementMaps[i], "Displacement", format, size, ansio);
			}
			for (int j = 0; j < this.m_slopeMaps.Length; j++)
			{
				this.CreateMap(ref this.m_slopeMaps[j], "Slope", format, size, ansio);
			}
			for (int k = 0; k < this.m_foamMaps.Length; k++)
			{
				this.CreateMap(ref this.m_foamMaps[k], "Foam", format, size, ansio);
			}
		}

		private void CreateMap(ref RenderTexture map, string name, RenderTextureFormat format, int size, int ansio)
		{
			if (map != null)
			{
				if (!map.IsCreated())
				{
					map.Create();
				}
				return;
			}
			map = new RenderTexture(size, size, 0, format, RenderTextureReadWrite.Linear);
			map.filterMode = FilterMode.Trilinear;
			map.wrapMode = TextureWrapMode.Repeat;
			map.anisoLevel = ansio;
			map.useMipMap = true;
			map.hideFlags = HideFlags.HideAndDontSave;
			map.name = "Ceto Wave Spectrum " + name + " Texture";
			map.Create();
		}

		private void CreateConditions()
		{
			int size = this.m_bufferSettings.size;
			WaveSpectrumConditionKey waveSpectrumConditionKey = this.NewSpectrumConditionKey(size, this.windSpeed, this.m_ocean.windDir, this.waveAge);
			if (this.m_conditions[0] == null)
			{
				if (this.m_conditionCache.ContainsKey(waveSpectrumConditionKey))
				{
					this.m_conditions[0] = this.m_conditionCache[waveSpectrumConditionKey];
					this.m_conditionCache.Remove(waveSpectrumConditionKey);
				}
				else
				{
					this.m_conditions[0] = this.NewSpectrumCondition(size, this.windSpeed, this.m_ocean.windDir, this.waveAge);
					IThreadedTask createSpectrumConditionTask = this.m_conditions[0].GetCreateSpectrumConditionTask();
					createSpectrumConditionTask.Start();
					createSpectrumConditionTask.Run();
					createSpectrumConditionTask.End();
				}
			}
			else if (this.m_conditions[1] != null && this.m_conditions[1].Done)
			{
				this.CacheCondition(this.m_conditions[0]);
				this.m_conditions[0] = this.m_conditions[1];
				this.m_conditions[1] = null;
			}
			else if (this.m_conditions[1] == null && this.m_conditions[0].Done && waveSpectrumConditionKey != this.m_conditions[0].Key)
			{
				if (this.m_conditionCache.ContainsKey(waveSpectrumConditionKey))
				{
					this.m_conditions[0] = this.m_conditionCache[waveSpectrumConditionKey];
					this.m_conditionCache.Remove(waveSpectrumConditionKey);
				}
				else
				{
					this.m_conditions[1] = this.NewSpectrumCondition(size, this.windSpeed, this.m_ocean.windDir, this.waveAge);
					IThreadedTask createSpectrumConditionTask2 = this.m_conditions[1].GetCreateSpectrumConditionTask();
					this.m_scheduler.Add(createSpectrumConditionTask2);
				}
			}
		}

		private void CacheCondition(WaveSpectrumCondition condition)
		{
			if (condition == null || this.m_conditionCache == null)
			{
				return;
			}
			if (this.m_conditionCache.ContainsKey(condition.Key))
			{
				return;
			}
			this.m_conditionCache.AddFirst(condition.Key, condition);
			while (this.m_conditionCache.Count != 0 && this.m_conditionCache.Count > this.m_maxConditionCacheSize)
			{
				WaveSpectrumCondition waveSpectrumCondition = this.m_conditionCache.RemoveLast();
				waveSpectrumCondition.Release();
			}
		}

		public void CreateAndCacheCondition(FOURIER_SIZE fourierSize)
		{
			int num;
			bool flag;
			this.GetFourierSize(out num, out flag);
			this.CreateAndCacheCondition(num, this.windSpeed, this.m_ocean.windDir, this.waveAge);
		}

		public void CreateAndCacheCondition(int fourierSize, float windSpeed, float windDir, float waveAge)
		{
			if (this.m_conditionCache == null)
			{
				return;
			}
			if (this.m_conditionCache.Count >= this.m_maxConditionCacheSize)
			{
				Ocean.LogWarning("Condition cache full. Condition not cached.");
				return;
			}
			if (!Mathf.IsPowerOfTwo(fourierSize) || fourierSize < 32 || fourierSize > 512)
			{
				Ocean.LogWarning("Fourier size must be a pow2 number from 32 to 512. Condition not cached.");
				return;
			}
			WaveSpectrumCondition waveSpectrumCondition = this.NewSpectrumCondition(fourierSize, windSpeed, windDir, waveAge);
			if (this.m_conditionCache.ContainsKey(waveSpectrumCondition.Key))
			{
				return;
			}
			IThreadedTask createSpectrumConditionTask = waveSpectrumCondition.GetCreateSpectrumConditionTask();
			createSpectrumConditionTask.Start();
			createSpectrumConditionTask.Run();
			createSpectrumConditionTask.End();
			this.m_conditionCache.AddFirst(waveSpectrumCondition.Key, waveSpectrumCondition);
		}

		private WaveSpectrumCondition NewSpectrumCondition(int fourierSize, float windSpeed, float windDir, float waveAge)
		{
			WaveSpectrumCondition result;
			switch (this.spectrumType)
			{
			case SPECTRUM_TYPE.UNIFIED:
				result = new UnifiedSpectrumCondition(fourierSize, windSpeed, windDir, waveAge, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.PHILLIPS:
				result = new PhillipsSpectrumCondition(fourierSize, windSpeed, windDir, waveAge, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.UNIFIED_PHILLIPS:
				result = new UnifiedPhillipsSpectrumCondition(fourierSize, windSpeed, windDir, waveAge, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.CUSTOM:
				if (base.CustomWaveSpectrum == null)
				{
					Ocean.LogWarning("Custom spectrum type selected but no custom spectrum interface has been added to the wave spectrum. Defaulting to Unified Spectrum");
					this.spectrumType = SPECTRUM_TYPE.UNIFIED;
					result = new UnifiedSpectrumCondition(fourierSize, windSpeed, windDir, waveAge, this.numberOfGrids);
				}
				else
				{
					result = new CustomWaveSpectrumCondition(base.CustomWaveSpectrum, fourierSize, windDir, this.numberOfGrids);
				}
				break;
			default:
				throw new InvalidOperationException("Invalid spectrum type = " + this.spectrumType);
			}
			return result;
		}

		private WaveSpectrumConditionKey NewSpectrumConditionKey(int fourierSize, float windSpeed, float windDir, float waveAge)
		{
			WaveSpectrumConditionKey result;
			switch (this.spectrumType)
			{
			case SPECTRUM_TYPE.UNIFIED:
				result = new UnifiedSpectrumConditionKey(windSpeed, waveAge, fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.PHILLIPS:
				result = new PhillipsSpectrumConditionKey(windSpeed, fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.UNIFIED_PHILLIPS:
				result = new UnifiedSpectrumConditionKey(windSpeed, waveAge, fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.CUSTOM:
				if (base.CustomWaveSpectrum == null)
				{
					Ocean.LogWarning("Custom spectrum type selected but no custom spectrum interface has been added to the wave spectrum. Defaulting to Unified Spectrum");
					this.spectrumType = SPECTRUM_TYPE.UNIFIED;
					result = new UnifiedSpectrumConditionKey(windSpeed, waveAge, fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				}
				else
				{
					result = base.CustomWaveSpectrum.CreateKey(fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				}
				break;
			default:
				throw new InvalidOperationException("Invalid spectrum type = " + this.spectrumType);
			}
			return result;
		}

		private void GetFourierSize(out int size, out bool isCpu)
		{
			switch (this.fourierSize)
			{
			case FOURIER_SIZE.LOW_32_CPU:
				size = 32;
				isCpu = true;
				break;
			case FOURIER_SIZE.LOW_32_GPU:
				size = 32;
				isCpu = false;
				break;
			case FOURIER_SIZE.MEDIUM_64_CPU:
				size = 64;
				isCpu = true;
				break;
			case FOURIER_SIZE.MEDIUM_64_GPU:
				size = 64;
				isCpu = false;
				break;
			case FOURIER_SIZE.HIGH_128_CPU:
				size = 128;
				isCpu = true;
				break;
			case FOURIER_SIZE.HIGH_128_GPU:
				size = 128;
				isCpu = false;
				break;
			case FOURIER_SIZE.ULTRA_256_GPU:
				size = 256;
				isCpu = false;
				break;
			case FOURIER_SIZE.EXTREME_512_GPU:
				size = 512;
				isCpu = false;
				break;
			default:
				size = 64;
				isCpu = true;
				break;
			}
			bool flag = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			if (!isCpu && !this.disableReadBack && !flag)
			{
				Ocean.LogWarning("You card does not support dx11. Fourier can not be GPU. Changing to CPU. Disable read backs to use GPU but with no height querys.");
				this.fourierSize = FOURIER_SIZE.MEDIUM_64_CPU;
				size = 64;
				isCpu = true;
			}
		}
	}
}
