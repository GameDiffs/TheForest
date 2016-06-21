using HutongGames.PlayMaker;
using System;
using UnityEngine;

public class mutantSoundDetect : MonoBehaviour
{
	private mutantScriptSetup setup;

	private bool soundCoolDown = true;

	private bool distractionCoolDown;

	private Collider currSoundCollider;

	private float terrainPosY;

	private FsmBool fsmInCave;

	private void Start()
	{
		this.setup = base.transform.root.GetComponentInChildren<mutantScriptSetup>();
		base.Invoke("resetCoolDown", 1f);
	}

	private void OnDisable()
	{
		base.CancelInvoke("resetCoolDown");
		this.soundCoolDown = false;
		this.distractionCoolDown = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("soundAlert") && !this.soundCoolDown)
		{
			if (!this.setup)
			{
				this.setup = base.transform.root.GetComponentInChildren<mutantScriptSetup>();
			}
			soundAlertType component = other.GetComponent<soundAlertType>();
			if (component && component.distraction)
			{
				if (this.distractionCoolDown)
				{
					return;
				}
				this.distractionCoolDown = true;
				base.Invoke("resetDistractionCoolDown", 60f);
			}
			if (this.setup && this.setup.lastSighting)
			{
				this.setup.lastSighting.transform.position = other.transform.position;
			}
			if (this.setup.pmSearch && this.setup.pmSearch.enabled)
			{
				this.setup.pmSearch.SendEvent("toPlayerNoise");
			}
			if (this.setup.pmCombat)
			{
				if (this.setup.ai.creepy_fat)
				{
					if (!this.setup.animator.GetBool("charge"))
					{
						this.setup.pmCombat.SendEvent("toPlayerNoise");
					}
				}
				else if (this.setup.pmCombat.enabled)
				{
					this.setup.pmCombat.SendEvent("toPlayerNoise");
				}
			}
			if (this.setup.pmSleep)
			{
				this.setup.pmSleep.SendEvent("toNoise");
			}
			if (this.setup.pmAlert)
			{
				this.setup.pmAlert.SendEvent("toNoise");
			}
			if (this.setup.pmEncounter)
			{
				this.setup.pmEncounter.SendEvent("toNoise");
			}
			if (!this.soundCoolDown)
			{
				base.Invoke("resetCoolDown", 5f);
			}
			this.currSoundCollider = other;
			this.soundCoolDown = true;
		}
	}

	private void resetCoolDown()
	{
		this.soundCoolDown = false;
	}

	private void resetDistractionCoolDown()
	{
		this.distractionCoolDown = false;
	}
}
