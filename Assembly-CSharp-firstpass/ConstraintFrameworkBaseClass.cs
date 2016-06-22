using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu(""), ExecuteInEditMode]
public class ConstraintFrameworkBaseClass : MonoBehaviour
{
	[HideInInspector]
	public Transform xform;

	protected virtual void Awake()
	{
		this.xform = base.transform;
	}

	protected virtual void OnEnable()
	{
		this.InitConstraint();
	}

	protected virtual void OnDisable()
	{
		base.StopCoroutine("Constrain");
	}

	protected virtual void InitConstraint()
	{
		base.StartCoroutine("Constrain");
	}

	[DebuggerHidden]
	protected virtual IEnumerator Constrain()
	{
		ConstraintFrameworkBaseClass.<Constrain>c__Iterator13 <Constrain>c__Iterator = new ConstraintFrameworkBaseClass.<Constrain>c__Iterator13();
		<Constrain>c__Iterator.<>f__this = this;
		return <Constrain>c__Iterator;
	}

	protected virtual void OnConstraintUpdate()
	{
		throw new NotImplementedException();
	}
}
