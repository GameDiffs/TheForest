using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class setupFeeding : MonoBehaviour
{
	private mutantController mutantControl;

	public GameObject destroyGo;

	private void Awake()
	{
	}

	private void Start()
	{
		this.mutantControl = Scene.MutantControler;
		if (UnityEngine.Random.value < 0.5f)
		{
			base.InvokeRepeating("startEatMeEvent", UnityEngine.Random.Range(0f, 3f), 15f);
			base.Invoke("stopEatEvent", 60f);
		}
	}

	public void startEatMeEvent()
	{
		base.StartCoroutine("sendEatMeEvent");
	}

	private void stopEatEvent()
	{
		base.CancelInvoke("startEatMeEvent");
		base.StopCoroutine("sendEatMeEvent");
		this.cancelEatMeEvent();
	}

	[DebuggerHidden]
	public IEnumerator sendEatMeEvent()
	{
		setupFeeding.<sendEatMeEvent>c__IteratorF5 <sendEatMeEvent>c__IteratorF = new setupFeeding.<sendEatMeEvent>c__IteratorF5();
		<sendEatMeEvent>c__IteratorF.<>f__this = this;
		return <sendEatMeEvent>c__IteratorF;
	}

	public void cancelEatMeEvent()
	{
		foreach (GameObject current in this.mutantControl.activeSkinnyCannibals)
		{
			if (current)
			{
				current.SendMessage("cancelEatMe", base.gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void OnDestroy()
	{
		this.cancelEatMeEvent();
		base.CancelInvoke("startEatMeEvent");
		base.StopAllCoroutines();
	}

	private void OnDisable()
	{
		this.cancelEatMeEvent();
		base.CancelInvoke("startEatMeEvent");
		base.StopAllCoroutines();
	}

	public void Explosion(float dist)
	{
		if (this.destroyGo)
		{
			UnityEngine.Object.Destroy(this.destroyGo);
		}
	}
}
