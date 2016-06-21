using System;
using UnityEngine;

public class batController : MonoBehaviour
{
	private Animation anim;

	private void OnEnable()
	{
		base.transform.parent.gameObject.SetActive(true);
		this.anim = base.transform.GetComponent<Animation>();
		this.anim["onCeilingLoop"].layer = 1;
		this.anim["onCeilingLoop"].wrapMode = WrapMode.Loop;
		this.anim["onCeilingLoop"].speed = UnityEngine.Random.Range(0.7f, 1.2f);
		this.anim.Play("onCeilingLoop", PlayMode.StopAll);
		this.anim["onCeilingLoop"].normalizedTime = UnityEngine.Random.Range(0f, 1f);
		base.transform.localPosition = new Vector3(0f, -0.3f, 0f);
		float num = UnityEngine.Random.Range(1f, 1.5f);
		base.transform.localScale = new Vector3(num, num, num);
	}
}
