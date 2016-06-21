using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite Animation"), ExecuteInEditMode, RequireComponent(typeof(UISprite))]
public class UISpriteAnimation : MonoBehaviour
{
	[HideInInspector, SerializeField]
	protected int mFPS = 30;

	[HideInInspector, SerializeField]
	protected string mPrefix = string.Empty;

	[HideInInspector, SerializeField]
	protected bool mLoop = true;

	[HideInInspector, SerializeField]
	protected bool mSnap = true;

	protected UISprite mSprite;

	protected float mDelta;

	protected int mIndex;

	protected bool mActive = true;

	protected List<string> mSpriteNames = new List<string>();

	public int frames
	{
		get
		{
			return this.mSpriteNames.Count;
		}
	}

	public int framesPerSecond
	{
		get
		{
			return this.mFPS;
		}
		set
		{
			this.mFPS = value;
		}
	}

	public string namePrefix
	{
		get
		{
			return this.mPrefix;
		}
		set
		{
			if (this.mPrefix != value)
			{
				this.mPrefix = value;
				this.RebuildSpriteList();
			}
		}
	}

	public bool loop
	{
		get
		{
			return this.mLoop;
		}
		set
		{
			this.mLoop = value;
		}
	}

	public bool isPlaying
	{
		get
		{
			return this.mActive;
		}
	}

	protected virtual void Start()
	{
		this.RebuildSpriteList();
	}

	protected virtual void Update()
	{
		if (this.mActive && this.mSpriteNames.Count > 1 && Application.isPlaying && this.mFPS > 0)
		{
			this.mDelta += RealTime.deltaTime;
			float num = 1f / (float)this.mFPS;
			if (num < this.mDelta)
			{
				this.mDelta = ((num <= 0f) ? 0f : (this.mDelta - num));
				if (++this.mIndex >= this.mSpriteNames.Count)
				{
					this.mIndex = 0;
					this.mActive = this.mLoop;
				}
				if (this.mActive)
				{
					this.mSprite.spriteName = this.mSpriteNames[this.mIndex];
					if (this.mSnap)
					{
						this.mSprite.MakePixelPerfect();
					}
				}
			}
		}
	}

	public void RebuildSpriteList()
	{
		if (this.mSprite == null)
		{
			this.mSprite = base.GetComponent<UISprite>();
		}
		this.mSpriteNames.Clear();
		if (this.mSprite != null && this.mSprite.atlas != null)
		{
			List<UISpriteData> spriteList = this.mSprite.atlas.spriteList;
			int i = 0;
			int count = spriteList.Count;
			while (i < count)
			{
				UISpriteData uISpriteData = spriteList[i];
				if (string.IsNullOrEmpty(this.mPrefix) || uISpriteData.name.StartsWith(this.mPrefix))
				{
					this.mSpriteNames.Add(uISpriteData.name);
				}
				i++;
			}
			this.mSpriteNames.Sort();
		}
	}

	public void Play()
	{
		this.mActive = true;
	}

	public void Pause()
	{
		this.mActive = false;
	}

	public void ResetToBeginning()
	{
		this.mActive = true;
		this.mIndex = 0;
		if (this.mSprite != null && this.mSpriteNames.Count > 0)
		{
			this.mSprite.spriteName = this.mSpriteNames[this.mIndex];
			if (this.mSnap)
			{
				this.mSprite.MakePixelPerfect();
			}
		}
	}
}
