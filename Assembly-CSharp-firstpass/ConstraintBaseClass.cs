using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("")]
public class ConstraintBaseClass : ConstraintFrameworkBaseClass
{
	public Transform _target;

	protected Transform _internalTarget;

	public UnityConstraints.NO_TARGET_OPTIONS _noTargetMode = UnityConstraints.NO_TARGET_OPTIONS.DoNothing;

	public UnityConstraints.MODE_OPTIONS _mode = UnityConstraints.MODE_OPTIONS.Constrain;

	protected Vector3 pos;

	protected Vector3 scl;

	protected Quaternion rot;

	public Transform target
	{
		get
		{
			if (this.noTargetMode == UnityConstraints.NO_TARGET_OPTIONS.SetByScript)
			{
				return this.internalTarget;
			}
			return this._target;
		}
		set
		{
			this._target = value;
		}
	}

	protected virtual Transform internalTarget
	{
		get
		{
			if (this._internalTarget != null)
			{
				return this._internalTarget;
			}
			this._internalTarget = new GameObject(base.name + "_InternalConstraintTarget")
			{
				hideFlags = HideFlags.HideInHierarchy
			}.transform;
			this._internalTarget.position = this.xform.position;
			this._internalTarget.rotation = this.xform.rotation;
			this._internalTarget.localScale = this.xform.localScale;
			return this._internalTarget;
		}
	}

	public Vector3 position
	{
		get
		{
			return this.internalTarget.position;
		}
		set
		{
			this.internalTarget.position = value;
		}
	}

	public Quaternion rotation
	{
		get
		{
			return this.internalTarget.rotation;
		}
		set
		{
			this.internalTarget.rotation = value;
		}
	}

	public Vector3 scale
	{
		get
		{
			return this.internalTarget.localScale;
		}
		set
		{
			this.internalTarget.localScale = value;
		}
	}

	public UnityConstraints.NO_TARGET_OPTIONS noTargetMode
	{
		get
		{
			return this._noTargetMode;
		}
		set
		{
			this._noTargetMode = value;
		}
	}

	public UnityConstraints.MODE_OPTIONS mode
	{
		get
		{
			return this._mode;
		}
		set
		{
			this._mode = value;
			this.InitConstraint();
		}
	}

	private void OnDestroy()
	{
		if (this._internalTarget != null)
		{
			if (!Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(this._internalTarget.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(this._internalTarget.gameObject);
			}
		}
	}

	protected sealed override void InitConstraint()
	{
		UnityConstraints.MODE_OPTIONS mode = this.mode;
		if (mode != UnityConstraints.MODE_OPTIONS.Align)
		{
			if (mode == UnityConstraints.MODE_OPTIONS.Constrain)
			{
				base.StartCoroutine("Constrain");
			}
		}
		else
		{
			this.OnOnce();
		}
	}

	[DebuggerHidden]
	protected sealed override IEnumerator Constrain()
	{
		ConstraintBaseClass.<Constrain>c__Iterator14 <Constrain>c__Iterator = new ConstraintBaseClass.<Constrain>c__Iterator14();
		<Constrain>c__Iterator.<>f__this = this;
		return <Constrain>c__Iterator;
	}

	protected virtual void NoTargetDefault()
	{
	}

	private void OnOnce()
	{
		this.OnConstraintUpdate();
	}
}
