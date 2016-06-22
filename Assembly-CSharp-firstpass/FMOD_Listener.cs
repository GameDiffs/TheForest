using FMOD;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FMOD_Listener : MonoBehaviour
{
	public interface ILinearEmitter
	{
		float GetMaximumDistance();

		void DrawDebug(Vector2 centre, float radius, float maximumDistance);
	}

	public interface IAreaEmitter
	{
		float GetMaximumDistance();

		void DrawDebug(Vector2 centre, float radius, float maximumDistance);
	}

	private struct ParameterInfo : IComparable<FMOD_Listener.ParameterInfo>
	{
		public string name;

		public float value;

		public float normalizedValue;

		public int CompareTo(FMOD_Listener.ParameterInfo other)
		{
			return string.Compare(this.name, other.name, StringComparison.InvariantCultureIgnoreCase);
		}
	}

	private struct EventInstanceInfo
	{
		public int streamCount;

		public FMOD.Studio.ATTRIBUTES_3D attributes3D;

		public List<FMOD_Listener.ParameterInfo> parameters;

		public float distance;

		public float normalizedDistance;

		public float normalizedPosition;

		public int ParameterCount
		{
			get
			{
				return (this.parameters == null) ? 0 : this.parameters.Count;
			}
		}
	}

	private struct EventInfo
	{
		public string path;

		public int streamCount;

		public int timelineLength;

		public bool is3D;

		public List<FMOD_Listener.EventInstanceInfo> instances;

		public List<float> normalizedPositions;
	}

	private class FMODErrorException : Exception
	{
		public RESULT Error
		{
			get;
			private set;
		}

		public FMODErrorException(RESULT error) : base(error.ToString() + " (" + FMOD.Error.String(error) + ")")
		{
			this.Error = error;
		}
	}

	public string[] pluginPaths;

	[Tooltip("Paths of events whose sample data should be preloaded")]
	public string[] preloadEvents;

	private static FMOD_Listener sListener = null;

	private static Dictionary<string, Bank> sLoadedBanks = new Dictionary<string, Bank>();

	private bool hasLoadedBanks;

	private Rigidbody cachedRigidBody;

	public static string[] PathFilter = new string[0];

	public static Action FMODCommonUpdate = null;

	public static Action DrawTreeDebug = null;

	public static HashSet<FMOD_Listener.ILinearEmitter> LinearEmitters = new HashSet<FMOD_Listener.ILinearEmitter>();

	public static HashSet<FMOD_Listener.IAreaEmitter> AreaEmitters = new HashSet<FMOD_Listener.IAreaEmitter>();

	private Action DrawExtraDebug;

	private bool showEventList;

	private bool showInstanceLabelsInWorld;

	private List<FMOD_Listener.EventInfo> eventInfoList = new List<FMOD_Listener.EventInfo>();

	private static Dictionary<string, int> sPreloadRequests = new Dictionary<string, int>();

	private static bool sPreloadComplete = false;

	private static Texture2D progressValueTexture;

	private static Texture2D instanceLabelTexture;

	private CPU_USAGE peakCPUUsage;

	private static GUIStyle sInstanceLabelStyle = null;

	private static GUIStyle sProgressValueStyle = null;

	public static bool HasLoadedBanks
	{
		get
		{
			return FMOD_Listener.sListener != null && FMOD_Listener.sListener.hasLoadedBanks;
		}
	}

	public static bool IsDebugEnabled
	{
		get
		{
			return FMOD_Listener.sListener != null && FMOD_Listener.sListener.EnableDebug;
		}
	}

	public bool EnableDebug
	{
		get;
		private set;
	}

	private string pluginPath
	{
		get
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return Application.dataPath + "/Plugins/x86";
			}
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
			{
				return Application.dataPath + "/Plugins";
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				UnityUtil.LogError("DSP Plugins not currently supported on iOS, contact support@fmod.org for more information");
				return string.Empty;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
				string name = directoryInfo.Parent.Name;
				return "/data/data/" + name + "/lib";
			}
			UnityUtil.LogError("Unknown platform!");
			return string.Empty;
		}
	}

	private static GUIStyle InstanceLabelStyle
	{
		get
		{
			if (FMOD_Listener.sInstanceLabelStyle == null)
			{
				FMOD_Listener.sInstanceLabelStyle = new GUIStyle(GUI.skin.box);
				FMOD_Listener.sInstanceLabelStyle.padding = new RectOffset(1, 1, 1, 1);
				FMOD_Listener.sInstanceLabelStyle.margin = new RectOffset(0, 0, 0, 0);
				FMOD_Listener.sInstanceLabelStyle.alignment = TextAnchor.MiddleCenter;
				FMOD_Listener.sInstanceLabelStyle.normal.background = FMOD_Listener.instanceLabelTexture;
				FMOD_Listener.sInstanceLabelStyle.border = new RectOffset(1, 1, 1, 1);
			}
			return FMOD_Listener.sInstanceLabelStyle;
		}
	}

	private static GUIStyle ProgressValueStyle
	{
		get
		{
			if (FMOD_Listener.sProgressValueStyle == null)
			{
				FMOD_Listener.sProgressValueStyle = new GUIStyle();
				FMOD_Listener.sProgressValueStyle.normal.background = FMOD_Listener.progressValueTexture;
				FMOD_Listener.sProgressValueStyle.border = new RectOffset(1, 1, 1, 1);
			}
			return FMOD_Listener.sProgressValueStyle;
		}
	}

	private void Awake()
	{
		this.Initialize();
	}

	private void loadBank(string path)
	{
		if (FMOD_Listener.sLoadedBanks.ContainsKey(path))
		{
			return;
		}
		UnityUtil.Log("Loading " + path + "...");
		Bank value = null;
		RESULT rESULT = FMOD_StudioSystem.instance.System.loadBankFile(path, LOAD_BANK_FLAGS.NORMAL, out value);
		if (rESULT == RESULT.ERR_VERSION)
		{
			UnityUtil.LogError(path + " was built with an incompatible version of FMOD Studio.");
		}
		if (rESULT == RESULT.OK)
		{
			UnityUtil.Log("Loading " + path + " succeeded.");
			FMOD_Listener.sLoadedBanks.Add(path, value);
		}
		else if (rESULT == RESULT.ERR_EVENT_ALREADY_LOADED)
		{
			UnityUtil.LogError(string.Concat(new string[]
			{
				"There may be an old FMOD bank left in the StreamingAssets directory. Loading ",
				path,
				" failed with ",
				rESULT.ToString(),
				"."
			}));
		}
		else
		{
			UnityUtil.LogError(string.Concat(new string[]
			{
				"Loading ",
				path,
				" failed: ",
				Error.String(rESULT),
				" (",
				rESULT.ToString(),
				")"
			}));
		}
	}

	private void LoadBanks()
	{
		string[] files = Directory.GetFiles(Application.streamingAssetsPath, "*.bank");
		for (int i = 0; i < files.Length; i++)
		{
			string path = files[i];
			this.loadBank(path);
		}
		this.hasLoadedBanks = true;
	}

	public static void UnloadBank(string path)
	{
		foreach (KeyValuePair<string, Bank> current in FMOD_Listener.sLoadedBanks)
		{
			string a;
			FMOD_Listener.ERRCHECK(current.Value.getPath(out a));
			if (a == path)
			{
				UnityUtil.Log("Unloading " + current.Key);
				FMOD_Listener.ERRCHECK(current.Value.unload());
				FMOD_Listener.sLoadedBanks.Remove(current.Key);
				break;
			}
		}
	}

	private void Initialize()
	{
		if (FMOD_StudioSystem.instance)
		{
			UnityUtil.Log("Initialize Listener");
			if (FMOD_Listener.sListener != null)
			{
				UnityUtil.Log("Too many listeners; destroying " + FMOD_Listener.sListener.ToString());
				UnityEngine.Object.Destroy(FMOD_Listener.sListener);
			}
			FMOD_Listener.sListener = this;
			this.LoadPlugins();
			this.LoadBanks();
			this.cachedRigidBody = base.GetComponent<Rigidbody>();
			this.Update3DAttributes();
		}
		else
		{
			UnityEngine.Debug.LogError("FMOD_StudioSystem.instance is null, failed to initialize listener");
			base.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (FMOD_Listener.sListener == this)
		{
			FMOD_Listener.sListener = null;
			FMOD_Listener.sPreloadComplete = false;
		}
	}

	public static void Preload(string path)
	{
		if (FMOD_Listener.sPreloadComplete)
		{
			FMOD_StudioSystem.PreloadEvent(path);
		}
		else if (FMOD_Listener.sPreloadRequests.ContainsKey(path))
		{
			Dictionary<string, int> dictionary;
			Dictionary<string, int> expr_2B = dictionary = FMOD_Listener.sPreloadRequests;
			int num = dictionary[path];
			expr_2B[path] = num + 1;
		}
		else
		{
			FMOD_Listener.sPreloadRequests[path] = 1;
		}
	}

	public static void UnPreload(string path)
	{
		if (FMOD_Listener.sPreloadComplete)
		{
			FMOD_StudioSystem.UnPreloadEvent(path);
		}
		else if (FMOD_Listener.sPreloadRequests.ContainsKey(path))
		{
			Dictionary<string, int> dictionary;
			Dictionary<string, int> expr_2A = dictionary = FMOD_Listener.sPreloadRequests;
			int num = dictionary[path];
			expr_2A[path] = num - 1;
		}
		else
		{
			FMOD_Listener.sPreloadRequests[path] = -1;
		}
	}

	private void Preload()
	{
		string[] array = this.preloadEvents;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			UnityUtil.Log(string.Format("Preloading {0}...", text));
			bool flag = false;
			EventDescription eventDescription;
			if (UnityUtil.ERRCHECK(FMOD_StudioSystem.instance.System.getEvent(text, out eventDescription)))
			{
				flag = UnityUtil.ERRCHECK(eventDescription.loadSampleData());
			}
			if (flag)
			{
				UnityUtil.Log(string.Format("Preloading {0} succeeded.", text));
			}
			else
			{
				UnityUtil.LogWarning(string.Format("Preloading {0} failed!", text));
			}
		}
		foreach (KeyValuePair<string, int> current in FMOD_Listener.sPreloadRequests)
		{
			if (current.Value > 0)
			{
				FMOD_StudioSystem.PreloadEvent(current.Key, current.Value);
			}
		}
		FMOD_Listener.sPreloadRequests.Clear();
		FMOD_Listener.sPreloadComplete = true;
	}

	private static void DrawBorder(Texture2D texture, Color color)
	{
		for (int i = 0; i < texture.width; i++)
		{
			texture.SetPixel(i, 0, color);
			texture.SetPixel(i, texture.height - 1, color);
		}
		for (int j = 1; j < texture.height - 1; j++)
		{
			texture.SetPixel(0, j, color);
			texture.SetPixel(texture.width - 1, j, color);
		}
	}

	public static void Fill(Texture2D texture, Color color)
	{
		for (int i = 0; i < texture.width; i++)
		{
			for (int j = 0; j < texture.height; j++)
			{
				texture.SetPixel(i, j, color);
			}
		}
	}

	private void Start()
	{
		this.EnableDebug = false;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		string[] array = commandLineArgs;
		for (int i = 0; i < array.Length; i++)
		{
			string a = array[i];
			if (a == "-FMODDebug")
			{
				this.EnableDebug = true;
			}
		}
		if (this.EnableDebug)
		{
			FMOD_Listener.progressValueTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			FMOD_Listener.Fill(FMOD_Listener.progressValueTexture, new Color(0f, 1f, 0f, 0.5f));
			FMOD_Listener.progressValueTexture.Apply();
			FMOD_Listener.instanceLabelTexture = new Texture2D(64, 64, TextureFormat.ARGB32, false);
			FMOD_Listener.Fill(FMOD_Listener.instanceLabelTexture, new Color(0f, 0f, 0f, 0.5f));
			FMOD_Listener.DrawBorder(FMOD_Listener.instanceLabelTexture, new Color(0f, 0f, 0f, 0.75f));
			FMOD_Listener.instanceLabelTexture.Apply();
		}
		this.Preload();
	}

	private void Update()
	{
		this.Update3DAttributes();
		if (FMOD_Listener.FMODCommonUpdate != null)
		{
			FMOD_Listener.FMODCommonUpdate();
		}
		if (this.EnableDebug)
		{
			if (Input.GetKeyDown(KeyCode.RightControl))
			{
				if (!this.showEventList)
				{
					this.showEventList = true;
					this.DrawExtraDebug = FMOD_Listener.DrawTreeDebug;
				}
				else if (this.DrawExtraDebug == FMOD_Listener.DrawTreeDebug)
				{
					this.DrawExtraDebug = new Action(this.DrawLinearEmitters);
				}
				else if (this.DrawExtraDebug == new Action(this.DrawLinearEmitters))
				{
					this.DrawExtraDebug = new Action(this.DrawAreaEmitters);
				}
				else
				{
					this.showEventList = false;
				}
			}
			if (Input.GetKeyDown(KeyCode.RightShift))
			{
				this.showInstanceLabelsInWorld = !this.showInstanceLabelsInWorld;
			}
			if (this.showEventList || this.showInstanceLabelsInWorld)
			{
				this.UpdateEventInfoList();
			}
		}
	}

	private void Update3DAttributes()
	{
		FMOD.Studio.System system = FMOD_StudioSystem.instance.System;
		if (system != null && system.isValid())
		{
			FMOD.Studio.ATTRIBUTES_3D attributes = UnityUtil.to3DAttributes(base.gameObject, this.cachedRigidBody);
			FMOD_Listener.ERRCHECK(system.setListenerAttributes(0, attributes));
		}
	}

	private static void THROW_ERRORS(RESULT result)
	{
		if (result != RESULT.OK)
		{
			throw new FMOD_Listener.FMODErrorException(result);
		}
	}

	private void UpdateEventInfoList()
	{
		this.eventInfoList.Clear();
		try
		{
			FMOD.Studio.ATTRIBUTES_3D aTTRIBUTES_3D;
			FMOD_Listener.THROW_ERRORS(FMOD_StudioSystem.instance.System.getListenerAttributes(0, out aTTRIBUTES_3D));
			Vector3 b2 = aTTRIBUTES_3D.position.toUnityVector();
			Bank[] array = null;
			FMOD_Listener.THROW_ERRORS(FMOD_StudioSystem.instance.System.getBankList(out array));
			Bank[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Bank bank = array2[i];
				try
				{
					EventDescription[] array3 = null;
					FMOD_Listener.THROW_ERRORS(bank.getEventList(out array3));
					if (array3 != null)
					{
						EventDescription[] array4 = array3;
						for (int j = 0; j < array4.Length; j++)
						{
							EventDescription eventDescription = array4[j];
							int num = 0;
							FMOD_Listener.THROW_ERRORS(eventDescription.getInstanceCount(out num));
							if (num > 0)
							{
								FMOD_Listener.EventInfo item = default(FMOD_Listener.EventInfo);
								item.streamCount = 0;
								FMOD_Listener.THROW_ERRORS(eventDescription.getPath(out item.path));
								if (FMOD_Listener.PathFilter.Length > 0)
								{
									bool flag = false;
									string[] pathFilter = FMOD_Listener.PathFilter;
									for (int k = 0; k < pathFilter.Length; k++)
									{
										string value = pathFilter[k];
										if (!item.path.ToLowerInvariant().Contains(value))
										{
											flag = true;
											break;
										}
									}
									if (flag)
									{
										goto IL_3F4;
									}
								}
								FMOD_Listener.THROW_ERRORS(eventDescription.getLength(out item.timelineLength));
								FMOD_Listener.THROW_ERRORS(eventDescription.is3D(out item.is3D));
								float num2 = 0f;
								FMOD_Listener.THROW_ERRORS(eventDescription.getMinimumDistance(out num2));
								float num3 = 0f;
								FMOD_Listener.THROW_ERRORS(eventDescription.getMaximumDistance(out num3));
								float num4 = num3 - num2;
								if (item.timelineLength > 0)
								{
									item.normalizedPositions = new List<float>(num);
								}
								else
								{
									item.normalizedPositions = null;
								}
								EventInstance[] array5 = null;
								FMOD_Listener.THROW_ERRORS(eventDescription.getInstanceList(out array5));
								item.instances = new List<FMOD_Listener.EventInstanceInfo>(num);
								EventInstance[] array6 = array5;
								for (int l = 0; l < array6.Length; l++)
								{
									EventInstance eventInstance = array6[l];
									try
									{
										if (FMOD_Listener.IsEventInstancePlaying(eventInstance))
										{
											FMOD_Listener.EventInstanceInfo item2 = default(FMOD_Listener.EventInstanceInfo);
											float num5 = 0f;
											FMOD_Listener.THROW_ERRORS(eventInstance.getProperty(EVENT_PROPERTY.STREAM_COUNT, out num5));
											item2.streamCount = (int)num5;
											item.streamCount += item2.streamCount;
											FMOD_Listener.THROW_ERRORS(eventInstance.get3DAttributes(out item2.attributes3D));
											Vector3 a2 = item2.attributes3D.position.toUnityVector();
											float num6 = Vector3.Distance(a2, b2);
											item2.distance = num6;
											item2.normalizedDistance = Mathf.Clamp01((num6 - num2) / num4);
											if (item.timelineLength > 0)
											{
												int num7 = 0;
												FMOD_Listener.THROW_ERRORS(eventInstance.getTimelinePosition(out num7));
												item2.normalizedPosition = Mathf.Clamp01((float)num7 / (float)item.timelineLength);
											}
											else
											{
												item2.normalizedPosition = 0f;
											}
											int num8 = 0;
											FMOD_Listener.THROW_ERRORS(eventInstance.getParameterCount(out num8));
											if (num8 > 0)
											{
												item2.parameters = new List<FMOD_Listener.ParameterInfo>(num8);
												for (int m = 0; m < num8; m++)
												{
													ParameterInstance parameterInstance;
													FMOD_Listener.THROW_ERRORS(eventInstance.getParameterByIndex(m, out parameterInstance));
													PARAMETER_DESCRIPTION pARAMETER_DESCRIPTION;
													FMOD_Listener.THROW_ERRORS(parameterInstance.getDescription(out pARAMETER_DESCRIPTION));
													float num9 = 0f;
													FMOD_Listener.THROW_ERRORS(parameterInstance.getValue(out num9));
													float num10 = pARAMETER_DESCRIPTION.maximum - pARAMETER_DESCRIPTION.minimum;
													FMOD_Listener.ParameterInfo item3;
													item3.name = pARAMETER_DESCRIPTION.name;
													item3.value = num9;
													item3.normalizedValue = (num9 - pARAMETER_DESCRIPTION.minimum) / num10;
													item2.parameters.Add(item3);
												}
												item2.parameters.Sort();
											}
											else
											{
												item2.parameters = null;
											}
											item.instances.Add(item2);
											if (item.normalizedPositions != null)
											{
												item.normalizedPositions.Add(item2.normalizedPosition);
											}
										}
									}
									catch (FMOD_Listener.FMODErrorException ex)
									{
										if (ex.Error != RESULT.ERR_INVALID_HANDLE)
										{
											UnityEngine.Debug.LogWarning(ex);
										}
									}
								}
								this.eventInfoList.Add(item);
							}
							IL_3F4:;
						}
					}
				}
				catch (FMOD_Listener.FMODErrorException ex2)
				{
					if (ex2.Error != RESULT.ERR_INVALID_HANDLE)
					{
						UnityEngine.Debug.LogWarning(ex2);
					}
				}
			}
			this.eventInfoList.Sort((FMOD_Listener.EventInfo a, FMOD_Listener.EventInfo b) => string.Compare(a.path, b.path, StringComparison.InvariantCultureIgnoreCase));
		}
		catch (FMOD_Listener.FMODErrorException message)
		{
			UnityEngine.Debug.LogWarning(message);
		}
	}

	private void LoadPlugins()
	{
		FMOD.System system = null;
		FMOD_Listener.ERRCHECK(FMOD_StudioSystem.instance.System.getLowLevelSystem(out system));
		if (Application.platform == RuntimePlatform.IPhonePlayer && this.pluginPaths.Length != 0)
		{
			UnityUtil.LogError("DSP Plugins not currently supported on iOS, contact support@fmod.org for more information");
			return;
		}
		string[] array = this.pluginPaths;
		for (int i = 0; i < array.Length; i++)
		{
			string rawName = array[i];
			string text = this.pluginPath + "/" + this.GetPluginFileName(rawName);
			UnityUtil.Log("Loading plugin: " + text);
			if (!File.Exists(text))
			{
				UnityUtil.LogWarning("plugin not found: " + text);
			}
			uint num;
			FMOD_Listener.ERRCHECK(system.loadPlugin(text, out num));
		}
	}

	private string GetPluginFileName(string rawName)
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
		{
			return rawName + ".dll";
		}
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer)
		{
			return rawName + ".dylib";
		}
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.LinuxPlayer)
		{
			return "lib" + rawName + ".so";
		}
		UnityUtil.LogError("Unknown platform!");
		return string.Empty;
	}

	private static bool IsEventInstancePlaying(EventInstance instance)
	{
		PLAYBACK_STATE pLAYBACK_STATE;
		FMOD_Listener.THROW_ERRORS(instance.getPlaybackState(out pLAYBACK_STATE));
		return pLAYBACK_STATE != PLAYBACK_STATE.STOPPED;
	}

	private static void NumberField(string label, int value, GUIStyle labelStyle, float labelWidth, GUIStyle valueStyle, float valueWidth)
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label(label, labelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(labelWidth)
		});
		GUILayout.Label(value.ToString(), valueStyle, new GUILayoutOption[]
		{
			GUILayout.Width(valueWidth)
		});
		GUILayout.EndHorizontal();
	}

	private static void NumberField(string label, float value, GUIStyle labelStyle, float labelWidth, GUIStyle valueStyle, float valueWidth)
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label(label, labelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(labelWidth)
		});
		GUILayout.Label(value.ToString("f2"), valueStyle, new GUILayoutOption[]
		{
			GUILayout.Width(valueWidth)
		});
		GUILayout.EndHorizontal();
	}

	private void DrawEventList()
	{
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
		gUIStyle.alignment = TextAnchor.MiddleLeft;
		gUIStyle.fontSize = 10;
		GUIStyle gUIStyle2 = new GUIStyle(gUIStyle);
		gUIStyle2.alignment = TextAnchor.MiddleRight;
		GUIStyle gUIStyle3 = new GUIStyle(gUIStyle);
		gUIStyle3.alignment = TextAnchor.MiddleRight;
		GUILayout.Space(25f);
		GUILayout.BeginVertical(GUI.skin.button, new GUILayoutOption[0]);
		if (FMOD_Listener.PathFilter.Length > 0)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Path Filter:", gUIStyle, new GUILayoutOption[0]);
			GUILayout.Label(string.Join(" AND ", FMOD_Listener.PathFilter), gUIStyle3, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
		}
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Instances", gUIStyle, new GUILayoutOption[]
		{
			GUILayout.Width(50f)
		});
		GUILayout.Label("Streams", gUIStyle, new GUILayoutOption[]
		{
			GUILayout.Width(40f)
		});
		GUILayout.Label("Event Path", gUIStyle, new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		int num = 0;
		int num2 = 0;
		foreach (FMOD_Listener.EventInfo current in this.eventInfoList)
		{
			if (current.instances.Count > 0)
			{
				List<FMOD_Listener.EventInstanceInfo> instances = current.instances;
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(instances.Count.ToString(), gUIStyle2, new GUILayoutOption[]
				{
					GUILayout.Width(50f)
				});
				GUILayout.Label(current.streamCount.ToString(), gUIStyle2, new GUILayoutOption[]
				{
					GUILayout.Width(40f)
				});
				num += instances.Count;
				num2 += current.streamCount;
				if (current.normalizedPositions != null)
				{
					FMOD_Listener.MultiValueBox(current.path, current.normalizedPositions, gUIStyle, FMOD_Listener.ProgressValueStyle);
				}
				else
				{
					GUILayout.Label(current.path, gUIStyle, new GUILayoutOption[0]);
				}
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label(num.ToString(), gUIStyle2, new GUILayoutOption[]
		{
			GUILayout.Width(50f)
		});
		GUILayout.Label(num2.ToString(), gUIStyle2, new GUILayoutOption[]
		{
			GUILayout.Width(40f)
		});
		GUILayout.Label("Totals", gUIStyle, new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.BeginArea(new Rect((float)(base.GetComponent<Camera>().pixelWidth - 155), 0f, 150f, 550f));
		BUFFER_USAGE bUFFER_USAGE;
		if (UnityUtil.ERRCHECK(FMOD_StudioSystem.instance.System.getBufferUsage(out bUFFER_USAGE)))
		{
			GUILayout.BeginVertical(GUI.skin.button, new GUILayoutOption[0]);
			GUILayout.Label("Command Buffer Usage", gUIStyle, new GUILayoutOption[0]);
			FMOD_Listener.NumberField("Current:", bUFFER_USAGE.studioCommandQueue.currentUsage, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Peak:", bUFFER_USAGE.studioCommandQueue.peakUsage, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Capacity:", bUFFER_USAGE.studioCommandQueue.capacity, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Stalls:", bUFFER_USAGE.studioCommandQueue.stallCount, gUIStyle, 50f, gUIStyle3, 50f);
			GUILayout.EndVertical();
		}
		int value = 0;
		int value2 = 0;
		if (UnityUtil.ERRCHECK(Memory.GetStats(out value, out value2)))
		{
			GUILayout.BeginVertical(GUI.skin.button, new GUILayoutOption[0]);
			GUILayout.Label("Memory Usage", gUIStyle, new GUILayoutOption[0]);
			FMOD_Listener.NumberField("Current:", value, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Peak:", value2, gUIStyle, 50f, gUIStyle3, 50f);
			GUILayout.EndVertical();
		}
		CPU_USAGE cPU_USAGE;
		if (UnityUtil.ERRCHECK(FMOD_StudioSystem.instance.System.getCPUUsage(out cPU_USAGE)))
		{
			GUILayout.BeginVertical(GUI.skin.button, new GUILayoutOption[0]);
			GUILayout.Label("CPU Usage", gUIStyle, new GUILayoutOption[0]);
			GUILayout.Label("Current", gUIStyle, new GUILayoutOption[0]);
			FMOD_Listener.NumberField("DSP:", cPU_USAGE.dspUsage, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Stream:", cPU_USAGE.streamUsage, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Update:", cPU_USAGE.updateUsage, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Studio:", cPU_USAGE.studioUsage, gUIStyle, 50f, gUIStyle3, 50f);
			this.peakCPUUsage.dspUsage = Math.Max(this.peakCPUUsage.dspUsage, cPU_USAGE.dspUsage);
			this.peakCPUUsage.streamUsage = Math.Max(this.peakCPUUsage.streamUsage, cPU_USAGE.streamUsage);
			this.peakCPUUsage.updateUsage = Math.Max(this.peakCPUUsage.updateUsage, cPU_USAGE.updateUsage);
			this.peakCPUUsage.studioUsage = Math.Max(this.peakCPUUsage.studioUsage, cPU_USAGE.studioUsage);
			GUILayout.Space(10f);
			GUILayout.Label("Peak", gUIStyle, new GUILayoutOption[0]);
			FMOD_Listener.NumberField("DSP:", this.peakCPUUsage.dspUsage, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Stream:", this.peakCPUUsage.streamUsage, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Update:", this.peakCPUUsage.updateUsage, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Studio:", this.peakCPUUsage.studioUsage, gUIStyle, 50f, gUIStyle3, 50f);
			GUILayout.EndVertical();
		}
		FMOD.System system = null;
		if (UnityUtil.ERRCHECK(FMOD_StudioSystem.instance.System.getLowLevelSystem(out system)))
		{
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < 1024; i++)
			{
				Channel channel;
				if (UnityUtil.ERRCHECK(system.getChannel(i, out channel)))
				{
					bool flag = false;
					RESULT rESULT = channel.isPlaying(out flag);
					if (rESULT != RESULT.ERR_INVALID_HANDLE)
					{
						FMOD_Listener.ERRCHECK(rESULT);
					}
					if (flag)
					{
						num3++;
						bool flag2 = false;
						UnityUtil.ERRCHECK(channel.isVirtual(out flag2));
						if (!flag2)
						{
							num4++;
						}
					}
				}
			}
			GUILayout.BeginVertical(GUI.skin.button, new GUILayoutOption[0]);
			GUILayout.Label("Channels", gUIStyle, new GUILayoutOption[0]);
			FMOD_Listener.NumberField("Playing:", num3, gUIStyle, 50f, gUIStyle3, 50f);
			FMOD_Listener.NumberField("Real:", num4, gUIStyle, 50f, gUIStyle3, 50f);
			GUILayout.EndVertical();
		}
		GUILayout.BeginVertical(GUI.skin.button, new GUILayoutOption[0]);
		FMOD_Listener.NumberField("Streams:", num2, gUIStyle, 50f, gUIStyle3, 50f);
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void DrawLinearEmitters()
	{
		if (Camera.main != null)
		{
			float num = 0f;
			foreach (FMOD_Listener.ILinearEmitter current in FMOD_Listener.LinearEmitters)
			{
				num = Mathf.Max(num, current.GetMaximumDistance());
			}
			Vector2 centre = new Vector2(110f, (float)(Camera.main.pixelHeight - 110));
			GUI.Box(new Rect(centre.x - 105f, centre.y - 105f - 35f, 210f, 245f), "Linear Emitters");
			foreach (FMOD_Listener.ILinearEmitter current2 in FMOD_Listener.LinearEmitters)
			{
				current2.DrawDebug(centre, 100f, num);
			}
		}
	}

	private void DrawAreaEmitters()
	{
		if (Camera.main != null)
		{
			float num = 0f;
			foreach (FMOD_Listener.IAreaEmitter current in FMOD_Listener.AreaEmitters)
			{
				num = Mathf.Max(num, current.GetMaximumDistance());
			}
			Vector2 centre = new Vector2(110f, (float)(Camera.main.pixelHeight - 110));
			GUI.Box(new Rect(centre.x - 105f, centre.y - 105f - 35f, 210f, 245f), "Area Emitters");
			foreach (FMOD_Listener.IAreaEmitter current2 in FMOD_Listener.AreaEmitters)
			{
				current2.DrawDebug(centre, 100f, num);
			}
		}
	}

	private static void ValueBox(string label, float progress)
	{
		FMOD_Listener.ValueBox(label, progress, FMOD_Listener.InstanceLabelStyle, FMOD_Listener.ProgressValueStyle);
	}

	private static void ValueBox(string label, float value, GUIStyle labelStyle, GUIStyle progressStyle)
	{
		GUILayout.Label(label, labelStyle, new GUILayoutOption[0]);
		Rect lastRect = GUILayoutUtility.GetLastRect();
		lastRect.x += lastRect.width * value;
		lastRect.width = 1f;
		GUI.Box(lastRect, string.Empty, progressStyle);
	}

	private static void MultiValueBox(string label, List<float> values, GUIStyle labelStyle, GUIStyle progressStyle)
	{
		GUILayout.Label(label, labelStyle, new GUILayoutOption[0]);
		foreach (float num in values)
		{
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRect.x += lastRect.width * num;
			lastRect.width = 1f;
			GUI.Box(lastRect, string.Empty, progressStyle);
		}
	}

	private void DrawInstanceLabels()
	{
		foreach (FMOD_Listener.EventInfo current in this.eventInfoList)
		{
			if (current.is3D)
			{
				foreach (FMOD_Listener.EventInstanceInfo current2 in current.instances)
				{
					Vector3 position = current2.attributes3D.position.toUnityVector();
					Vector3 vector = base.GetComponent<Camera>().WorldToScreenPoint(position);
					if (vector.z > 0f)
					{
						Vector2 vector2 = FMOD_Listener.InstanceLabelStyle.CalcSize(new GUIContent(current.path));
						Rect screenRect = new Rect(vector.x, (float)base.GetComponent<Camera>().pixelHeight - vector.y - vector2.y, vector2.x, vector2.y * (float)(current2.ParameterCount + 2));
						GUILayout.BeginArea(screenRect);
						FMOD_Listener.ValueBox(current.path, current2.normalizedPosition);
						FMOD_Listener.ValueBox(string.Format("distance {0:G3}", current2.distance), current2.normalizedDistance);
						if (current2.parameters != null)
						{
							foreach (FMOD_Listener.ParameterInfo current3 in current2.parameters)
							{
								GUILayout.BeginHorizontal(new GUILayoutOption[0]);
								GUILayout.Label(current3.name, FMOD_Listener.InstanceLabelStyle, new GUILayoutOption[]
								{
									GUILayout.ExpandWidth(false)
								});
								FMOD_Listener.ValueBox(current3.value.ToString("G3"), current3.normalizedValue);
								GUILayout.EndHorizontal();
							}
						}
						GUILayout.EndArea();
					}
				}
			}
		}
	}

	private static void ERRCHECK(RESULT result)
	{
		UnityUtil.ERRCHECK(result);
	}
}
