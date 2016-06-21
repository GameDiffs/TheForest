using System;
using TheForest.Utils;
using UnityEngine;

public class netHideDuringPlaneCrash : MonoBehaviour
{
	public Renderer[] hideRenderers;

	public GameObject[] hideGo;

	private Animator animator;

	private AnimatorStateInfo currState2;

	private int getupHash = Animator.StringToHash("getup");

	private bool getupStarted;

	private bool timeout;

	public bool saveCheck;

	private float destroyTime = 4f;

	private float enableTime;

	private void Start()
	{
		this.enableTime = Time.time + 3f;
		this.destroyTime = Time.time + 4f;
		this.animator = base.transform.GetComponent<Animator>();
		base.enabled = true;
	}

	private void Update()
	{
		if (Scene.PlaneCrash.doneHidePlayer)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (Scene.PlaneCrash.fakePlaneActive)
		{
			this.timeout = true;
		}
		this.currState2 = this.animator.GetCurrentAnimatorStateInfo(2);
		if (this.currState2.tagHash == this.getupHash)
		{
			this.getupStarted = true;
		}
		else
		{
			this.getupStarted = false;
		}
		if (this.timeout)
		{
			for (int i = 0; i < this.hideRenderers.Length; i++)
			{
				this.hideRenderers[i].enabled = true;
			}
			for (int j = 0; j < this.hideGo.Length; j++)
			{
				this.hideGo[j].SetActive(true);
			}
			Scene.PlaneCrash.doneHidePlayer = true;
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (!LocalPlayer.AnimControl.introCutScene && !this.getupStarted && !this.saveCheck && !this.timeout)
		{
			for (int k = 0; k < this.hideRenderers.Length; k++)
			{
				this.hideRenderers[k].enabled = false;
			}
			for (int l = 0; l < this.hideGo.Length; l++)
			{
				this.hideGo[l].SetActive(false);
			}
			this.enableTime = Time.time + 3f;
		}
		else if (Time.time > this.enableTime)
		{
			for (int m = 0; m < this.hideRenderers.Length; m++)
			{
				this.hideRenderers[m].enabled = true;
			}
			for (int n = 0; n < this.hideGo.Length; n++)
			{
				this.hideGo[n].SetActive(true);
			}
			Scene.PlaneCrash.doneHidePlayer = true;
			UnityEngine.Object.Destroy(this);
		}
	}
}
