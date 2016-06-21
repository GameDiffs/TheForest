using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InvGameItem
{
	public enum Quality
	{
		Broken,
		Cursed,
		Damaged,
		Worn,
		Sturdy,
		Polished,
		Improved,
		Crafted,
		Superior,
		Enchanted,
		Epic,
		Legendary,
		_LastDoNotUse
	}

	[SerializeField]
	private int mBaseItemID;

	public InvGameItem.Quality quality = InvGameItem.Quality.Sturdy;

	public int itemLevel = 1;

	private InvBaseItem mBaseItem;

	public int baseItemID
	{
		get
		{
			return this.mBaseItemID;
		}
	}

	public InvBaseItem baseItem
	{
		get
		{
			if (this.mBaseItem == null)
			{
				this.mBaseItem = InvDatabase.FindByID(this.baseItemID);
			}
			return this.mBaseItem;
		}
	}

	public string name
	{
		get
		{
			if (this.baseItem == null)
			{
				return null;
			}
			return this.quality.ToString() + " " + this.baseItem.name;
		}
	}

	public float statMultiplier
	{
		get
		{
			float num = 0f;
			switch (this.quality)
			{
			case InvGameItem.Quality.Broken:
				num = 0f;
				break;
			case InvGameItem.Quality.Cursed:
				num = -1f;
				break;
			case InvGameItem.Quality.Damaged:
				num = 0.25f;
				break;
			case InvGameItem.Quality.Worn:
				num = 0.9f;
				break;
			case InvGameItem.Quality.Sturdy:
				num = 1f;
				break;
			case InvGameItem.Quality.Polished:
				num = 1.1f;
				break;
			case InvGameItem.Quality.Improved:
				num = 1.25f;
				break;
			case InvGameItem.Quality.Crafted:
				num = 1.5f;
				break;
			case InvGameItem.Quality.Superior:
				num = 1.75f;
				break;
			case InvGameItem.Quality.Enchanted:
				num = 2f;
				break;
			case InvGameItem.Quality.Epic:
				num = 2.5f;
				break;
			case InvGameItem.Quality.Legendary:
				num = 3f;
				break;
			}
			float num2 = (float)this.itemLevel / 50f;
			return num * Mathf.Lerp(num2, num2 * num2, 0.5f);
		}
	}

	public Color color
	{
		get
		{
			Color result = Color.white;
			switch (this.quality)
			{
			case InvGameItem.Quality.Broken:
				result = new Color(0.4f, 0.2f, 0.2f);
				break;
			case InvGameItem.Quality.Cursed:
				result = Color.red;
				break;
			case InvGameItem.Quality.Damaged:
				result = new Color(0.4f, 0.4f, 0.4f);
				break;
			case InvGameItem.Quality.Worn:
				result = new Color(0.7f, 0.7f, 0.7f);
				break;
			case InvGameItem.Quality.Sturdy:
				result = new Color(1f, 1f, 1f);
				break;
			case InvGameItem.Quality.Polished:
				result = NGUIMath.HexToColor(3774856959u);
				break;
			case InvGameItem.Quality.Improved:
				result = NGUIMath.HexToColor(2480359935u);
				break;
			case InvGameItem.Quality.Crafted:
				result = NGUIMath.HexToColor(1325334783u);
				break;
			case InvGameItem.Quality.Superior:
				result = NGUIMath.HexToColor(12255231u);
				break;
			case InvGameItem.Quality.Enchanted:
				result = NGUIMath.HexToColor(1937178111u);
				break;
			case InvGameItem.Quality.Epic:
				result = NGUIMath.HexToColor(2516647935u);
				break;
			case InvGameItem.Quality.Legendary:
				result = NGUIMath.HexToColor(4287627519u);
				break;
			}
			return result;
		}
	}

	public InvGameItem(int id)
	{
		this.mBaseItemID = id;
	}

	public InvGameItem(int id, InvBaseItem bi)
	{
		this.mBaseItemID = id;
		this.mBaseItem = bi;
	}

	public List<InvStat> CalculateStats()
	{
		List<InvStat> list = new List<InvStat>();
		if (this.baseItem != null)
		{
			float statMultiplier = this.statMultiplier;
			List<InvStat> stats = this.baseItem.stats;
			int i = 0;
			int count = stats.Count;
			while (i < count)
			{
				InvStat invStat = stats[i];
				int num = Mathf.RoundToInt(statMultiplier * (float)invStat.amount);
				if (num != 0)
				{
					bool flag = false;
					int j = 0;
					int count2 = list.Count;
					while (j < count2)
					{
						InvStat invStat2 = list[j];
						if (invStat2.id == invStat.id && invStat2.modifier == invStat.modifier)
						{
							invStat2.amount += num;
							flag = true;
							break;
						}
						j++;
					}
					if (!flag)
					{
						list.Add(new InvStat
						{
							id = invStat.id,
							amount = num,
							modifier = invStat.modifier
						});
					}
				}
				i++;
			}
			list.Sort(new Comparison<InvStat>(InvStat.CompareArmor));
		}
		return list;
	}
}
