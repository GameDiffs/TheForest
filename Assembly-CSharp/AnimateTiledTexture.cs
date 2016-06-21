using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimateTiledTexture : MonoBehaviour
{
	public delegate void VoidEvent();

	public int _columns = 2;

	public int _rows = 2;

	public Vector2 _scale = new Vector3(1f, 1f);

	public Vector2 _offset = Vector2.zero;

	public Vector2 _buffer = Vector2.zero;

	public float _framesPerSecond = 10f;

	public bool _playOnce;

	public bool _disableUponCompletion;

	public bool _enableEvents;

	public bool _playOnEnable = true;

	public bool _newMaterialInstance;

	private int _index;

	private Vector2 _textureSize = Vector2.zero;

	private Material _materialInstance;

	private bool _hasMaterialInstance;

	private bool _isPlaying;

	private List<AnimateTiledTexture.VoidEvent> _voidEventCallbackList;

	public void RegisterCallback(AnimateTiledTexture.VoidEvent cbFunction)
	{
		if (this._enableEvents)
		{
			this._voidEventCallbackList.Add(cbFunction);
		}
		else
		{
			UnityEngine.Debug.LogWarning("AnimateTiledTexture: You are attempting to register a callback but the events of this object are not enabled!");
		}
	}

	public void UnRegisterCallback(AnimateTiledTexture.VoidEvent cbFunction)
	{
		if (this._enableEvents)
		{
			this._voidEventCallbackList.Remove(cbFunction);
		}
		else
		{
			UnityEngine.Debug.LogWarning("AnimateTiledTexture: You are attempting to un-register a callback but the events of this object are not enabled!");
		}
	}

	public void Play()
	{
		if (this._isPlaying)
		{
			base.StopCoroutine("updateTiling");
			this._isPlaying = false;
		}
		base.GetComponent<Renderer>().enabled = true;
		this._index = this._columns;
		base.StartCoroutine(this.updateTiling());
	}

	public void ChangeMaterial(Material newMaterial, bool newInstance = false)
	{
		if (newInstance)
		{
			if (this._hasMaterialInstance)
			{
				UnityEngine.Object.Destroy(base.GetComponent<Renderer>().sharedMaterial);
			}
			this._materialInstance = new Material(newMaterial);
			base.GetComponent<Renderer>().sharedMaterial = this._materialInstance;
			this._hasMaterialInstance = true;
		}
		else
		{
			base.GetComponent<Renderer>().sharedMaterial = newMaterial;
		}
		this.CalcTextureSize();
		base.GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", this._textureSize);
	}

	private void Awake()
	{
		if (this._enableEvents)
		{
			this._voidEventCallbackList = new List<AnimateTiledTexture.VoidEvent>();
		}
		this.ChangeMaterial(base.GetComponent<Renderer>().sharedMaterial, this._newMaterialInstance);
	}

	private void OnDestroy()
	{
		if (this._hasMaterialInstance)
		{
			UnityEngine.Object.Destroy(base.GetComponent<Renderer>().sharedMaterial);
			this._hasMaterialInstance = false;
		}
	}

	private void HandleCallbacks(List<AnimateTiledTexture.VoidEvent> cbList)
	{
		for (int i = 0; i < cbList.Count; i++)
		{
			cbList[i]();
		}
	}

	private void OnEnable()
	{
		this.CalcTextureSize();
		if (this._playOnEnable)
		{
			this.Play();
		}
	}

	private void CalcTextureSize()
	{
		this._textureSize = new Vector2(1f / (float)this._columns, 1f / (float)this._rows);
		this._textureSize.x = this._textureSize.x / this._scale.x;
		this._textureSize.y = this._textureSize.y / this._scale.y;
		this._textureSize -= this._buffer;
	}

	[DebuggerHidden]
	private IEnumerator updateTiling()
	{
		AnimateTiledTexture.<updateTiling>c__Iterator125 <updateTiling>c__Iterator = new AnimateTiledTexture.<updateTiling>c__Iterator125();
		<updateTiling>c__Iterator.<>f__this = this;
		return <updateTiling>c__Iterator;
	}

	private void ApplyOffset()
	{
		Vector2 offset = new Vector2((float)this._index / (float)this._columns - (float)(this._index / this._columns), 1f - (float)(this._index / this._columns) / (float)this._rows);
		if (offset.y == 1f)
		{
			offset.y = 0f;
		}
		offset.x += (1f / (float)this._columns - this._textureSize.x) / 2f;
		offset.y += (1f / (float)this._rows - this._textureSize.y) / 2f;
		offset.x += this._offset.x;
		offset.y += this._offset.y;
		base.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
	}
}
