using System;
using UnityEngine;

public class UI2DSpriteAnimation : MonoBehaviour
{
	[SerializeField]
	protected int framerate = 20;

	public bool ignoreTimeScale = true;

	public bool loop = true;

	public Sprite[] frames;

	private SpriteRenderer mUnitySprite;

	private UI2DSprite mNguiSprite;

	private int mIndex;

	private float mUpdate;

	public bool isPlaying
	{
		get
		{
			return base.enabled;
		}
	}

	public int framesPerSecond
	{
		get
		{
			return this.framerate;
		}
		set
		{
			this.framerate = value;
		}
	}

	public void Play()
	{
		if (this.frames != null && this.frames.Length > 0)
		{
			if (!base.enabled && !this.loop)
			{
				int num = (this.framerate <= 0) ? (this.mIndex - 1) : (this.mIndex + 1);
				if (num < 0 || num >= this.frames.Length)
				{
					this.mIndex = ((this.framerate >= 0) ? 0 : (this.frames.Length - 1));
				}
			}
			base.enabled = true;
			this.UpdateSprite();
		}
	}

	public void Pause()
	{
		base.enabled = false;
	}

	public void ResetToBeginning()
	{
		this.mIndex = ((this.framerate >= 0) ? 0 : (this.frames.Length - 1));
		this.UpdateSprite();
	}

	private void Start()
	{
		this.Play();
	}

	private void Update()
	{
		if (this.frames == null || this.frames.Length == 0)
		{
			base.enabled = false;
		}
		else if (this.framerate != 0)
		{
			float num = (!this.ignoreTimeScale) ? Time.time : RealTime.time;
			if (this.mUpdate < num)
			{
				this.mUpdate = num;
				int num2 = (this.framerate <= 0) ? (this.mIndex - 1) : (this.mIndex + 1);
				if (!this.loop && (num2 < 0 || num2 >= this.frames.Length))
				{
					base.enabled = false;
					return;
				}
				this.mIndex = NGUIMath.RepeatIndex(num2, this.frames.Length);
				this.UpdateSprite();
			}
		}
	}

	private void UpdateSprite()
	{
		if (this.mUnitySprite == null && this.mNguiSprite == null)
		{
			this.mUnitySprite = base.GetComponent<SpriteRenderer>();
			this.mNguiSprite = base.GetComponent<UI2DSprite>();
			if (this.mUnitySprite == null && this.mNguiSprite == null)
			{
				base.enabled = false;
				return;
			}
		}
		float num = (!this.ignoreTimeScale) ? Time.time : RealTime.time;
		if (this.framerate != 0)
		{
			this.mUpdate = num + Mathf.Abs(1f / (float)this.framerate);
		}
		if (this.mUnitySprite != null)
		{
			this.mUnitySprite.sprite = this.frames[this.mIndex];
		}
		else if (this.mNguiSprite != null)
		{
			this.mNguiSprite.nextSprite = this.frames[this.mIndex];
		}
	}
}
