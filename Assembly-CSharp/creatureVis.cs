using System;
using TheForest.Utils;
using UnityEngine;

public class creatureVis : MonoBehaviour
{
	private PlayMakerFSM pmControl;

	private float playerDist;

	public bool disableAnimator;

	public AmplifyMotionObjectBase[] amplifyBase;

	public Animator animator;

	private Renderer _renderer;

	public Camera renderCam;

	private lizardAnimatorControl control;

	private Transform thisTr;

	public GameObject[] joints;

	private bool trigger;

	private bool isVisible;

	private bool doAmplify;

	private bool amplifyTrigger;

	public bool useMotionBlur;

	private void Awake()
	{
		if (this.useMotionBlur)
		{
			base.InvokeRepeating("checkPlayerDist", UnityEngine.Random.Range(0f, 3f), 2f);
		}
		this.animator = base.transform.parent.GetComponent<Animator>();
		this._renderer = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		this.control = base.transform.parent.GetComponent<lizardAnimatorControl>();
	}

	private void Start()
	{
		this.thisTr = base.transform;
		base.Invoke("getAmplifyObj", 0.1f);
	}

	private void OnEnable()
	{
		if (!base.IsInvoking("checkPlayerDist"))
		{
			base.InvokeRepeating("checkPlayerDist", UnityEngine.Random.Range(0f, 3f), 2f);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("checkPlayerDist");
	}

	private void getAmplifyObj()
	{
		if (this.amplifyBase.Length == 0)
		{
			this.amplifyBase = base.transform.root.GetComponentsInChildren<AmplifyMotionObjectBase>();
		}
		this.doAmplify = true;
		if (!this.useMotionBlur)
		{
			this.disableAmplifyMotion();
		}
	}

	private void checkPlayerDist()
	{
		if (LocalPlayer.Transform != null)
		{
			this.playerDist = Vector3.Distance(this.thisTr.position, LocalPlayer.Transform.position);
		}
	}

	private void Update()
	{
		if (this.renderCam == null && Camera.main)
		{
			this.renderCam = Camera.main;
		}
		if (this.useMotionBlur)
		{
			if (this.playerDist > 40f && !this.amplifyTrigger && this.doAmplify)
			{
				this.disableAmplifyMotion();
				this.amplifyTrigger = true;
			}
			else if (this.playerDist < 40f && this.amplifyTrigger && this.doAmplify)
			{
				this.enableAmplifyMotion();
				this.amplifyTrigger = false;
			}
		}
	}

	private void disableAmplifyMotion()
	{
		if (this.amplifyBase != null)
		{
			for (int i = 0; i < this.amplifyBase.Length; i++)
			{
				this.amplifyBase[i].enabled = false;
			}
		}
	}

	private void enableAmplifyMotion()
	{
		if (this.amplifyBase != null)
		{
			for (int i = 0; i < this.amplifyBase.Length; i++)
			{
				this.amplifyBase[i].enabled = true;
			}
		}
	}

	private void disableAnimation()
	{
		for (int i = 0; i < this.joints.Length; i++)
		{
			this.joints[i].SetActive(false);
		}
		this._renderer.enabled = false;
		if (this.disableAnimator)
		{
			this.animator.enabled = false;
		}
	}

	private void enableAnimation()
	{
		for (int i = 0; i < this.joints.Length; i++)
		{
			this.joints[i].SetActive(true);
		}
		if (this.control)
		{
			this.control.enabled = true;
		}
		this._renderer.enabled = true;
		if (this.disableAnimator)
		{
			this.animator.enabled = true;
		}
	}
}
