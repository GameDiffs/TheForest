using Pathfinding;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class gridObjectBlocker : MonoBehaviour
{
	public GameObject go;

	private GameObject goCopy;

	public bool direct;

	public bool oneTimeOnly;

	public float delay;

	public bool blockNav;

	public bool ignoreOnDisable;

	private bool coolDown;

	private bool delayedCoolDown;

	private float diff;

	private void Awake()
	{
		this.go = base.gameObject;
	}

	private void OnDeserialized()
	{
		this.go = base.gameObject;
		this.doNavCut();
	}

	private void Start()
	{
		if (!this.blockNav)
		{
			this.doNavCut();
		}
	}

	private void OnEnable()
	{
		if (!this.blockNav && !this.ignoreOnDisable)
		{
			this.doNavCut();
		}
	}

	private void doNavCut()
	{
		if (this.blockNav)
		{
			return;
		}
		if (AstarPath.active && !this.coolDown)
		{
			this.go = base.gameObject;
			if (this.oneTimeOnly)
			{
				this.blockNav = true;
			}
			Collider component = this.go.GetComponent<Collider>();
			if (!component)
			{
				return;
			}
			Bounds bounds = component.bounds;
			this.diff = bounds.center.y - bounds.extents.y;
			float num = Terrain.activeTerrain.SampleHeight(bounds.center) + Terrain.activeTerrain.transform.position.y;
			this.diff -= num;
			if (this.diff > 7f)
			{
				return;
			}
			getStructureStrength component2 = this.go.GetComponent<getStructureStrength>();
			bool flag = false;
			if (this.go.layer == 21 && component2)
			{
				if (component2._type == getStructureStrength.structureType.floor)
				{
					this.go.layer = 26;
					flag = true;
				}
				else if (component2._type == getStructureStrength.structureType.wall)
				{
					this.go.layer = 20;
					flag = true;
				}
			}
			GraphUpdateObject ob = new GraphUpdateObject(bounds);
			AstarPath.active.UpdateGraphs(ob);
			this.coolDown = true;
			base.Invoke("resetCoolDown", 1f);
			if (flag)
			{
				base.Invoke("resetLayer", 0.2f);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator doDelayedNavCut(int delay)
	{
		gridObjectBlocker.<doDelayedNavCut>c__Iterator60 <doDelayedNavCut>c__Iterator = new gridObjectBlocker.<doDelayedNavCut>c__Iterator60();
		<doDelayedNavCut>c__Iterator.delay = delay;
		<doDelayedNavCut>c__Iterator.<$>delay = delay;
		<doDelayedNavCut>c__Iterator.<>f__this = this;
		return <doDelayedNavCut>c__Iterator;
	}

	private void resetLayer()
	{
		if (this.go)
		{
			this.go.layer = 21;
		}
	}

	private void OnDestroy()
	{
		this.doRemove();
	}

	private void OnDisable()
	{
		if (!this.ignoreOnDisable)
		{
			this.doRemove();
		}
	}

	private void doRemove()
	{
		if (this.blockNav)
		{
			return;
		}
		if (AstarPath.active && !this.coolDown)
		{
			Terrain activeTerrain = Terrain.activeTerrain;
			if (activeTerrain)
			{
				if (!this.go)
				{
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(this.go, this.go.transform.position, this.go.transform.rotation) as GameObject;
				Collider component = gameObject.GetComponent<Collider>();
				if (!component)
				{
					UnityEngine.Object.Destroy(gameObject);
					return;
				}
				Bounds bounds = component.bounds;
				UnityEngine.Object.Destroy(gameObject);
				GraphUpdateObject ob = new GraphUpdateObject(bounds);
				AstarPath.active.UpdateGraphs(ob, 0f);
				this.coolDown = true;
				base.Invoke("resetCoolDown", 1f);
			}
		}
	}

	private void startDummyNavRemove()
	{
		if (this.blockNav)
		{
			return;
		}
		if (!AstarPath.active)
		{
			return;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain)
		{
			Collider component = base.GetComponent<Collider>();
			if (!component)
			{
				return;
			}
			Bounds bounds = component.bounds;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load("dummyNavRemove"), base.transform.position, base.transform.rotation);
			gameObject.SendMessage("doDummyNavRemove", bounds);
			this.blockNav = true;
		}
	}

	private void resetCoolDown()
	{
		this.coolDown = false;
	}

	private void resetDelayedCoolDown()
	{
		this.delayedCoolDown = false;
	}
}
