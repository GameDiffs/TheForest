using System;
using UnityEngine;

public class ActivateTrigger : MonoBehaviour
{
	public enum Mode
	{
		Trigger,
		Replace,
		Activate,
		Enable,
		Animate,
		Deactivate
	}

	public ActivateTrigger.Mode action = ActivateTrigger.Mode.Activate;

	public UnityEngine.Object target;

	public GameObject source;

	public int triggerCount = 1;

	public bool repeatTrigger;

	private void DoActivateTrigger()
	{
		this.triggerCount--;
		if (this.triggerCount == 0 || this.repeatTrigger)
		{
			UnityEngine.Object @object = (!(this.target != null)) ? base.gameObject : this.target;
			Behaviour behaviour = @object as Behaviour;
			GameObject gameObject = @object as GameObject;
			if (behaviour != null)
			{
				gameObject = behaviour.gameObject;
			}
			switch (this.action)
			{
			case ActivateTrigger.Mode.Trigger:
				gameObject.BroadcastMessage("DoActivateTrigger");
				break;
			case ActivateTrigger.Mode.Replace:
				if (this.source != null)
				{
					UnityEngine.Object.Instantiate(this.source, gameObject.transform.position, gameObject.transform.rotation);
					UnityEngine.Object.DestroyObject(gameObject);
				}
				break;
			case ActivateTrigger.Mode.Activate:
				gameObject.SetActive(true);
				break;
			case ActivateTrigger.Mode.Enable:
				if (behaviour != null)
				{
					behaviour.enabled = true;
				}
				break;
			case ActivateTrigger.Mode.Animate:
				gameObject.GetComponent<Animation>().Play();
				break;
			case ActivateTrigger.Mode.Deactivate:
				gameObject.SetActive(false);
				break;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		this.DoActivateTrigger();
	}
}
