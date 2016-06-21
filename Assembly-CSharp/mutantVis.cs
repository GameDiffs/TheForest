using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class mutantVis : MonoBehaviour
{
	private PlayMakerFSM pmControl;

	public Animator animator;

	private SkinnedMeshRenderer thisRenderer;

	private SkinnedMeshRenderer thisLowRenderer;

	private SkinnedMeshRenderer thisSkinnyRenderer;

	private SkinnedMeshRenderer thisLowSkinnyRenderer;

	public GameObject mainRendererGo;

	public GameObject lowRendererGo;

	public GameObject lowSkinnyGo;

	public Camera renderCam;

	private mutantScriptSetup setup;

	public AmplifyMotionObject[] amplifyObj;

	public AmplifyMotionObjectBase[] amplifyBase;

	public amplifyDisableHook[] adh;

	private bool trigger;

	private bool amplifyTrigger;

	private bool doAmplify;

	public bool animEnabled;

	public bool animDisabled;

	public bool animReduced;

	public GameObject[] joints;

	public GameObject[] hideGo;

	public Renderer[] hideRenderers;

	public GameObject displacementGo;

	public MecanimEventEmitter mecanimEmitter;

	private GameObject props;

	private GameObject collisionGo;

	private GameObject rootGo;

	public float playerDist;

	private Transform playerTr;

	private Transform thisTr;

	private Vector3 playerLocalTarget;

	private bool encounterCheck;

	private FsmBool fsmSleepBool;

	private FsmBool fsmDoingEncounter;

	private FsmBool fsmAnimatorActive;

	public FsmFloat fsmDist;

	public float mecanimEventsDisableDistance = 75f;

	public float displacementDisableDistance = 35f;

	public bool isVisible;

	public bool isActive;

	public bool netPrefab;

	private bool initBool;

	public List<GameObject> allPlayers = new List<GameObject>();

	private void Awake()
	{
		Transform[] componentsInChildren = base.transform.root.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			if (transform.name == "propsGo")
			{
				this.props = transform.gameObject;
			}
			if (transform.name == "collisionRig")
			{
				this.collisionGo = transform.gameObject;
			}
		}
		this.adh = base.transform.root.GetComponentsInChildren<amplifyDisableHook>();
	}

	private void Start()
	{
		this.thisTr = base.transform;
		this.animator = base.transform.GetComponent<Animator>();
		if (this.mainRendererGo)
		{
			this.thisRenderer = this.mainRendererGo.GetComponent<SkinnedMeshRenderer>();
		}
		if (this.lowSkinnyGo)
		{
			this.thisSkinnyRenderer = this.lowSkinnyGo.GetComponent<SkinnedMeshRenderer>();
		}
		if (this.lowRendererGo)
		{
			this.thisLowRenderer = this.lowRendererGo.GetComponent<SkinnedMeshRenderer>();
		}
		if (this.lowSkinnyGo)
		{
			this.thisLowSkinnyRenderer = this.lowSkinnyGo.GetComponent<SkinnedMeshRenderer>();
		}
		this.rootGo = base.transform.root.gameObject;
		if (!this.netPrefab)
		{
			this.setup = base.transform.GetComponent<mutantScriptSetup>();
			this.fsmDist = this.setup.pmCombat.FsmVariables.GetFsmFloat("playerDist");
			if (this.setup.pmSleep)
			{
				this.setup.pmSleep.FsmVariables.GetFsmGameObject("meshGo").Value = base.gameObject;
			}
			if (this.setup.pmEncounter)
			{
				this.fsmDoingEncounter = this.setup.pmEncounter.FsmVariables.GetFsmBool("doingEncounter");
			}
			if (this.setup.pmBrain)
			{
				this.fsmSleepBool = this.setup.pmBrain.FsmVariables.GetFsmBool("sleepBool");
			}
			else if (this.setup.pmCombat)
			{
				this.fsmSleepBool = this.setup.pmCombat.FsmVariables.GetFsmBool("sleepBool");
			}
			if (this.setup.pmCombat)
			{
				this.fsmAnimatorActive = this.setup.pmCombat.FsmVariables.GetFsmBool("animatorActive");
			}
		}
		base.Invoke("getAmplifyObj", 0.1f);
		if (this.netPrefab)
		{
			base.InvokeRepeating("updatePlayerTargets", 1f, 1f);
		}
		this.initBool = false;
		base.Invoke("initMe", 4f);
	}

	private void OnEnable()
	{
		this.initBool = false;
		base.Invoke("initMe", 4f);
	}

	private void initMe()
	{
		this.initBool = true;
	}

	private void updatePlayerTargets()
	{
		if (!Scene.SceneTracker)
		{
			return;
		}
		this.allPlayers = new List<GameObject>(Scene.SceneTracker.allPlayers);
		this.allPlayers.RemoveAll((GameObject o) => o == null);
		if (this.allPlayers.Count > 1)
		{
			this.allPlayers.Sort((GameObject c1, GameObject c2) => (this.thisTr.position - c1.transform.position).sqrMagnitude.CompareTo((this.thisTr.position - c2.transform.position).sqrMagnitude));
		}
		if (this.allPlayers.Count == 0)
		{
			return;
		}
		if (this.allPlayers[0] && this.allPlayers[0] != null)
		{
			this.playerTr = this.allPlayers[0].transform;
		}
		this.playerDist = Vector3.Distance(this.thisTr.position, this.playerTr.position);
	}

	private void getAmplifyObj()
	{
		this.amplifyObj = base.transform.parent.GetComponentsInChildren<AmplifyMotionObject>();
		this.amplifyBase = base.transform.root.GetComponentsInChildren<AmplifyMotionObjectBase>();
		this.doAmplify = true;
	}

	private void Update()
	{
		if (!this.initBool)
		{
			return;
		}
		if (this.netPrefab)
		{
			this.encounterCheck = false;
		}
		if (!this.netPrefab)
		{
			this.playerDist = this.setup.ai.mainPlayerDist;
		}
		if (this.rootGo.activeSelf)
		{
			if (!this.netPrefab)
			{
				if (this.setup.ai.creepy || this.setup.ai.creepy_baby || this.setup.ai.creepy_male || this.setup.ai.creepy_fat)
				{
					this.encounterCheck = false;
				}
				else
				{
					this.encounterCheck = this.fsmDoingEncounter.Value;
					if (this.playerDist > 30f)
					{
						if (this.setup.ai.maleSkinny)
						{
							this.lowSkinnyGo.SetActive(true);
							this.lowRendererGo.SetActive(false);
							this.thisRenderer.enabled = false;
						}
						else
						{
							this.lowRendererGo.SetActive(true);
							this.lowSkinnyGo.SetActive(false);
							this.thisRenderer.enabled = false;
						}
					}
					else
					{
						this.lowSkinnyGo.SetActive(false);
						this.lowRendererGo.SetActive(false);
						this.thisRenderer.enabled = true;
					}
				}
			}
			if (this.renderCam == null && Camera.main)
			{
				this.renderCam = Camera.main;
			}
			if (this.renderCam != null)
			{
				if (this.mainRendererGo && this.mainRendererGo.activeSelf)
				{
					if (this.thisRenderer.IsVisibleFrom(this.renderCam))
					{
						this.isVisible = true;
					}
					else
					{
						this.isVisible = false;
					}
				}
				if (this.lowSkinnyGo && this.lowSkinnyGo.activeSelf)
				{
					if (this.thisSkinnyRenderer.IsVisibleFrom(this.renderCam))
					{
						this.isVisible = true;
					}
					else
					{
						this.isVisible = false;
					}
				}
				if (this.lowRendererGo && this.lowRendererGo.activeSelf)
				{
					if (this.thisLowRenderer.IsVisibleFrom(this.renderCam))
					{
						this.isVisible = true;
					}
					else
					{
						this.isVisible = false;
					}
				}
			}
			if (this.encounterCheck)
			{
				if (this.trigger)
				{
					this.enableAnimation();
					this.trigger = false;
				}
			}
			else if (this.playerDist > 220f)
			{
				this.disableAnimation();
				this.trigger = true;
			}
			else if (this.isVisible)
			{
				if (this.trigger)
				{
					this.enableAnimation();
					this.trigger = false;
				}
			}
			else if ((this.isVisible && this.playerDist < 150f) || this.playerDist < 30f)
			{
				if (this.trigger)
				{
					this.enableAnimation();
					this.trigger = false;
				}
			}
			else if (this.playerDist > 150f)
			{
				this.disableAnimation();
				this.trigger = true;
			}
			else
			{
				this.reduceAnimation();
				this.trigger = true;
			}
			if (this.playerDist > 20f && !this.amplifyTrigger && this.doAmplify)
			{
				this.disableAmplifyMotion();
				this.amplifyTrigger = true;
			}
			else if (this.playerDist < 20f && this.amplifyTrigger && this.doAmplify)
			{
				this.enableAmplifyMotion();
				this.amplifyTrigger = false;
			}
		}
		if (this.mecanimEmitter)
		{
			if (this.playerDist > this.mecanimEventsDisableDistance)
			{
				this.mecanimEmitter.enabled = false;
			}
			else
			{
				this.mecanimEmitter.enabled = true;
			}
		}
		if (this.displacementGo)
		{
			if (this.playerDist > this.displacementDisableDistance)
			{
				this.displacementGo.SetActive(false);
			}
			else
			{
				this.displacementGo.SetActive(true);
			}
		}
	}

	private void syncBlood()
	{
	}

	private void disableAmplifyMotion()
	{
		if (this.adh != null)
		{
			for (int i = 0; i < this.adh.Length; i++)
			{
				this.adh[i].skipUpdate = true;
			}
		}
	}

	private void enableAmplifyMotion()
	{
		if (this.adh != null)
		{
			for (int i = 0; i < this.adh.Length; i++)
			{
				this.adh[i].skipUpdate = false;
			}
		}
	}

	private void disableAnimation()
	{
		if (BoltNetwork.isServer)
		{
			return;
		}
		if (!this.animDisabled)
		{
			if (!this.netPrefab)
			{
				if (this.setup.ai.male || this.setup.ai.female)
				{
					this.setup.typeSetup.storeDefaultParams();
				}
				this.fsmAnimatorActive.Value = false;
			}
			this.isActive = false;
			this.animator.enabled = false;
			if (this.props)
			{
				this.props.SetActive(false);
			}
			for (int i = 0; i < this.hideGo.Length; i++)
			{
				this.hideGo[i].SetActive(false);
			}
			for (int j = 0; j < this.hideRenderers.Length; j++)
			{
				this.hideRenderers[j].enabled = false;
			}
			for (int k = 0; k < this.joints.Length; k++)
			{
				this.joints[k].SetActive(false);
			}
			this.thisRenderer.enabled = false;
			if (this.collisionGo)
			{
				this.collisionGo.SetActive(false);
			}
			this.animEnabled = false;
			this.animReduced = false;
			this.animDisabled = true;
		}
	}

	private void reduceAnimation()
	{
		if (BoltNetwork.isServer)
		{
			return;
		}
		if (!this.animReduced)
		{
			if (!this.netPrefab && (this.setup.ai.male || this.setup.ai.female))
			{
				this.setup.typeSetup.storeDefaultParams();
			}
			this.isActive = false;
			if (this.thisRenderer)
			{
				this.thisRenderer.enabled = false;
			}
			if (this.props)
			{
				this.props.SetActive(false);
			}
			for (int i = 0; i < this.hideGo.Length; i++)
			{
				this.hideGo[i].SetActive(false);
			}
			for (int j = 0; j < this.hideRenderers.Length; j++)
			{
				this.hideRenderers[j].enabled = false;
			}
			for (int k = 0; k < this.joints.Length; k++)
			{
				this.joints[k].SetActive(false);
			}
			this.animDisabled = false;
			this.animEnabled = false;
			this.animReduced = true;
		}
	}

	private void enableAnimation()
	{
		if (BoltNetwork.isServer)
		{
			return;
		}
		if (!this.animEnabled)
		{
			this.isActive = true;
			if (this.thisRenderer)
			{
				this.thisRenderer.enabled = true;
			}
			if (this.props)
			{
				this.props.SetActive(true);
			}
			for (int i = 0; i < this.hideGo.Length; i++)
			{
				this.hideGo[i].SetActive(true);
			}
			if (this.hideRenderers.Length > 0)
			{
				Renderer[] array = this.hideRenderers;
				for (int j = 0; j < array.Length; j++)
				{
					Renderer renderer = array[j];
					renderer.enabled = true;
				}
			}
			for (int k = 0; k < this.hideRenderers.Length; k++)
			{
				this.hideRenderers[k].enabled = true;
			}
			for (int l = 0; l < this.joints.Length; l++)
			{
				this.joints[l].SetActive(true);
			}
			this.animator.enabled = true;
			if (!this.netPrefab)
			{
				this.fsmAnimatorActive.Value = true;
				if (this.setup.ai.male || this.setup.ai.female)
				{
					this.setup.typeSetup.setDefaultParams();
				}
			}
			if (this.collisionGo)
			{
				this.collisionGo.SetActive(true);
			}
			this.animReduced = false;
			this.animEnabled = true;
			if (this.animDisabled)
			{
				this.animator.SetTriggerReflected("resetTrigger");
				this.animDisabled = false;
			}
		}
	}
}
