using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UniLinq;
using UnityEngine;

[AddComponentMenu("Storage/Advanced/Only In Range Manager")]
public class OnlyInRangeManager : MonoBehaviour
{
	public class InRange
	{
		public Transform transform;

		public Vector3 lastPosition;

		private bool _inprogress;

		public string id;

		public int count;

		public bool inprogress
		{
			get
			{
				return this._inprogress;
			}
			set
			{
				this._inprogress = value;
				this.count = 0;
			}
		}

		public void Test(OnlyInRangeManager manager, Vector3 position, float sqrRange)
		{
			if (this.inprogress)
			{
				return;
			}
			if (this.transform != null)
			{
				if ((this.transform.position - position).sqrMagnitude < sqrRange)
				{
					this.count++;
					if (this.count > 3)
					{
						manager.hideList.Remove(this);
					}
				}
				else
				{
					this.count = 0;
					manager.hideList.Add(this);
				}
			}
			else if ((this.lastPosition - position).sqrMagnitude < sqrRange)
			{
				this.count++;
				if (this.count > 3)
				{
					manager.viewList.Add(this);
				}
			}
			else
			{
				this.count = 0;
				manager.viewList.Remove(this);
			}
		}
	}

	public static OnlyInRangeManager Instance;

	[SerializeThis]
	private HashSet<OnlyInRangeManager.InRange> rangedItems = new HashSet<OnlyInRangeManager.InRange>();

	[SerializeThis]
	private HashSet<OnlyInRangeManager.InRange> hideList = new HashSet<OnlyInRangeManager.InRange>();

	[SerializeThis]
	private HashSet<OnlyInRangeManager.InRange> viewList = new HashSet<OnlyInRangeManager.InRange>();

	public float range = 5f;

	public void AddRangedItem(GameObject go)
	{
		PrefabIdentifier ui = go.GetComponent<PrefabIdentifier>();
		if (ui == null)
		{
			return;
		}
		if (!this.rangedItems.Any((OnlyInRangeManager.InRange i) => i.id == ui.Id))
		{
			this.rangedItems.Add(new OnlyInRangeManager.InRange
			{
				id = ui.Id,
				transform = go.transform
			});
		}
	}

	public void DestroyRangedItem(GameObject go)
	{
		PrefabIdentifier ui = go.GetComponent<PrefabIdentifier>();
		if (ui == null)
		{
			return;
		}
		OnlyInRangeManager.InRange inRange = this.rangedItems.FirstOrDefault((OnlyInRangeManager.InRange i) => i.id == ui.Id);
		if (inRange == null || inRange.inprogress)
		{
			return;
		}
		if (File.Exists(Application.persistentDataPath + "/" + inRange.id + ".dat"))
		{
			File.Delete(Application.persistentDataPath + "/" + inRange.id + ".dat");
		}
		this.rangedItems.Remove(inRange);
	}

	private void Awake()
	{
		OnlyInRangeManager.Instance = this;
	}

	private void LateUpdate()
	{
		if (LevelSerializer.IsDeserializing)
		{
			return;
		}
		float sqrRange = this.range * this.range;
		Vector3 position = base.transform.position;
		foreach (OnlyInRangeManager.InRange current in this.rangedItems)
		{
			current.Test(this, position, sqrRange);
		}
		if (this.hideList.Count > 0 && (Time.frameCount & 1) != 0)
		{
			OnlyInRangeManager.InRange inRange = this.hideList.First<OnlyInRangeManager.InRange>();
			this.hideList.Remove(inRange);
			inRange.inprogress = true;
			base.StartCoroutine(this.HideItem(inRange));
		}
		if (this.viewList.Count > 0 && (Time.frameCount & 1) == 0)
		{
			OnlyInRangeManager.InRange inRange2 = this.viewList.First<OnlyInRangeManager.InRange>();
			this.viewList.Remove(inRange2);
			inRange2.inprogress = true;
			base.StartCoroutine(this.ViewItem(inRange2));
		}
	}

	[DebuggerHidden]
	private IEnumerator HideItem(OnlyInRangeManager.InRange item)
	{
		OnlyInRangeManager.<HideItem>c__Iterator1D8 <HideItem>c__Iterator1D = new OnlyInRangeManager.<HideItem>c__Iterator1D8();
		<HideItem>c__Iterator1D.item = item;
		<HideItem>c__Iterator1D.<$>item = item;
		return <HideItem>c__Iterator1D;
	}

	[DebuggerHidden]
	private IEnumerator ViewItem(OnlyInRangeManager.InRange item)
	{
		OnlyInRangeManager.<ViewItem>c__Iterator1D9 <ViewItem>c__Iterator1D = new OnlyInRangeManager.<ViewItem>c__Iterator1D9();
		<ViewItem>c__Iterator1D.item = item;
		<ViewItem>c__Iterator1D.<$>item = item;
		return <ViewItem>c__Iterator1D;
	}
}
