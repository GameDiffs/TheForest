using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BMFont
{
	[HideInInspector, SerializeField]
	private int mSize = 16;

	[HideInInspector, SerializeField]
	private int mBase;

	[HideInInspector, SerializeField]
	private int mWidth;

	[HideInInspector, SerializeField]
	private int mHeight;

	[HideInInspector, SerializeField]
	private string mSpriteName;

	[HideInInspector, SerializeField]
	private List<BMGlyph> mSaved = new List<BMGlyph>();

	private Dictionary<int, BMGlyph> mDict = new Dictionary<int, BMGlyph>();

	public bool isValid
	{
		get
		{
			return this.mSaved.Count > 0;
		}
	}

	public int charSize
	{
		get
		{
			return this.mSize;
		}
		set
		{
			this.mSize = value;
		}
	}

	public int baseOffset
	{
		get
		{
			return this.mBase;
		}
		set
		{
			this.mBase = value;
		}
	}

	public int texWidth
	{
		get
		{
			return this.mWidth;
		}
		set
		{
			this.mWidth = value;
		}
	}

	public int texHeight
	{
		get
		{
			return this.mHeight;
		}
		set
		{
			this.mHeight = value;
		}
	}

	public int glyphCount
	{
		get
		{
			return (!this.isValid) ? 0 : this.mSaved.Count;
		}
	}

	public string spriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			this.mSpriteName = value;
		}
	}

	public List<BMGlyph> glyphs
	{
		get
		{
			return this.mSaved;
		}
	}

	public BMGlyph GetGlyph(int index, bool createIfMissing)
	{
		BMGlyph bMGlyph = null;
		if (this.mDict.Count == 0)
		{
			int i = 0;
			int count = this.mSaved.Count;
			while (i < count)
			{
				BMGlyph bMGlyph2 = this.mSaved[i];
				this.mDict.Add(bMGlyph2.index, bMGlyph2);
				i++;
			}
		}
		if (!this.mDict.TryGetValue(index, out bMGlyph) && createIfMissing)
		{
			bMGlyph = new BMGlyph();
			bMGlyph.index = index;
			this.mSaved.Add(bMGlyph);
			this.mDict.Add(index, bMGlyph);
		}
		return bMGlyph;
	}

	public BMGlyph GetGlyph(int index)
	{
		return this.GetGlyph(index, false);
	}

	public void Clear()
	{
		this.mDict.Clear();
		this.mSaved.Clear();
	}

	public void Trim(int xMin, int yMin, int xMax, int yMax)
	{
		if (this.isValid)
		{
			int i = 0;
			int count = this.mSaved.Count;
			while (i < count)
			{
				BMGlyph bMGlyph = this.mSaved[i];
				if (bMGlyph != null)
				{
					bMGlyph.Trim(xMin, yMin, xMax, yMax);
				}
				i++;
			}
		}
	}
}
