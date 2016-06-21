using System;
using UniLinq;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleScale2 : MonoBehaviour
{
	private float _currentScale;

	private float[] _startSizes;

	private float[] _startSpeed;

	private float[] _startGravity;

	private ParticleSystem[] _systems;

	public float ScaleOfParticles = 1f;

	public bool AlsoScaleGameobject;

	private void Start()
	{
		this._currentScale = 1f;
		this._systems = base.GetComponentsInChildren<ParticleSystem>();
		this._startSizes = (from x in this._systems
		select x.startSize).ToArray<float>();
		this._startSpeed = (from x in this._systems
		select x.startSpeed).ToArray<float>();
		this._startGravity = (from x in this._systems
		select x.gravityModifier).ToArray<float>();
		this.Scale(this._currentScale = this.ScaleOfParticles);
		this._startSizes = (from x in this._systems
		select x.startSize).ToArray<float>();
		this._startSpeed = (from x in this._systems
		select x.startSpeed).ToArray<float>();
		this._startGravity = (from x in this._systems
		select x.gravityModifier).ToArray<float>();
		this.Scale(this._currentScale);
	}

	private void Update()
	{
		if (this._currentScale != this.ScaleOfParticles)
		{
			this.Scale(this._currentScale = this.ScaleOfParticles);
			if (this.AlsoScaleGameobject)
			{
				base.transform.localScale = new Vector3(this._currentScale, this._currentScale, this._currentScale);
			}
		}
	}

	private void Scale(float scale)
	{
		for (int i = 0; i < this._systems.Length; i++)
		{
			this._systems[i].startSize = this._startSizes[i] * scale;
			this._systems[i].startSpeed = this._startSpeed[i] * scale;
			this._systems[i].gravityModifier = this._startGravity[i] * scale;
		}
	}
}
