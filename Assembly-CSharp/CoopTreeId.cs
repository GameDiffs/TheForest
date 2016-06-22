using Bolt;
using FMOD.Studio;
using System;
using System.Collections;
using System.Diagnostics;
using UniLinq;
using UnityEngine;

[ExecuteInEditMode]
public class CoopTreeId : EntityBehaviour<ITreeCutState>, IPriorityCalculator
{
	public const int STATE_PRISTINE = 0;

	public const int STATE_DAMAGED = 1;

	public const int STATE_FALLING = 2;

	public const int STATE_DESTROYED = 3;

	public const int STATE_REMOVED = 4;

	[HideInInspector]
	public LOD_Trees lod;

	private GameObject cut;

	private GameObject fall;

	private TreeCutChunk[] cut_chunks;

	[SerializeField]
	public int Id;

	[SerializeField]
	public GameObject NetworkPrefab;

	[SerializeField]
	public GameObject NetworkFallPrefab;

	private PropertyCallbackSimple c1;

	private PropertyCallbackSimple c2;

	private PropertyCallbackSimple c3;

	private PropertyCallbackSimple c4;

	private PropertyCallbackSimple dmg;

	bool IPriorityCalculator.Always
	{
		get
		{
			return false;
		}
	}

	float IPriorityCalculator.CalculateEventPriority(BoltConnection connection, Bolt.Event evnt)
	{
		return (float)((base.state.State != 2) ? 256 : 8192);
	}

	float IPriorityCalculator.CalculateStatePriority(BoltConnection connection, int skipped)
	{
		return (float)((base.state.State != 2) ? 256 : 8192);
	}

	private void Awake()
	{
		this.lod = base.GetComponent<LOD_Trees>();
		base.enabled = false;
	}

	private void Update()
	{
		base.state.Fuel += Time.deltaTime;
		if (this.lod)
		{
			if (base.state.Fuel >= this.lod.High.GetComponent<FireDamage>().FuelSeconds)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"Update (fuel done) ",
					base.name,
					" (lodview=",
					this.lod.CurrentView,
					")"
				}));
				if (this.lod.CurrentLOD != 0)
				{
					GameObject trunk = (GameObject)UnityEngine.Object.Instantiate(this.lod.High.GetComponent<FireDamage>().MyBurnt, base.transform.position, base.transform.rotation);
					this.Burnt(trunk);
				}
				base.enabled = false;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	public void CheckBurning()
	{
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"CheckBurning ",
			base.name,
			" : ",
			base.state.Burning
		}));
		base.enabled = base.state.Burning;
	}

	private void Burnt(GameObject trunk)
	{
		if (!BoltNetwork.isClient && this.lod)
		{
			UnityEngine.Debug.Log("Burnt " + base.name);
			if (this.lod.OnTreeCutDownTarget != null)
			{
				this.lod.OnTreeCutDownTarget.SendMessage("OnTreeCutDown", trunk);
			}
			if (BoltNetwork.isRunning)
			{
				this.Goto_Removed();
			}
			UnityEngine.Object.Destroy(this.lod);
		}
	}

	public override void Attached()
	{
		base.state.AddCallback("State", new PropertyCallbackSimple(this.State));
		base.state.AddCallback("Chunk1", this.c1 = new PropertyCallbackSimple(this.ChunkUpdater1));
		base.state.AddCallback("Chunk2", this.c2 = new PropertyCallbackSimple(this.ChunkUpdater2));
		base.state.AddCallback("Chunk3", this.c3 = new PropertyCallbackSimple(this.ChunkUpdater3));
		base.state.AddCallback("Chunk4", this.c4 = new PropertyCallbackSimple(this.ChunkUpdater4));
		base.state.AddCallback("Damage", this.dmg = new PropertyCallbackSimple(this.Damage));
		if (BoltNetwork.isServer)
		{
			base.state.AddCallback("Burning", new PropertyCallbackSimple(this.CheckBurning));
		}
		base.enabled = false;
		if (BoltNetwork.isServer)
		{
			if (!this.lod)
			{
				base.state.State = 4;
			}
			else if (!this.lod.enabled)
			{
				base.state.State = 3;
			}
		}
		else
		{
			this.State();
		}
	}

	private void Damage()
	{
		if (base.state.Damage >= 16 && this.entity.isOwner && base.state.State == 1)
		{
			base.state.State = 2;
		}
	}

	private void Goto_Destroyed()
	{
		base.state.State = 3;
	}

	public void Goto_Removed()
	{
		if (this.entity.isAttached)
		{
			base.state.State = 4;
			this.entity.Freeze(false);
		}
	}

	public void State()
	{
		switch (base.state.State)
		{
		case 1:
			this.State_Damaged();
			break;
		case 2:
			this.State_Falling();
			break;
		case 3:
			this.State_Destroyed();
			break;
		case 4:
			this.State_Removed();
			break;
		}
	}

	private void State_Destroyed()
	{
		base.state.FallingTransform.SetTransforms(null);
		if (this.entity.isOwner)
		{
			return;
		}
		if (this.fall)
		{
			this.fall.BroadcastMessage("ActivateLeafParticles", SendMessageOptions.DontRequireReceiver);
			UnityEngine.Object.Destroy(this.fall);
			this.fall = null;
		}
		if (this.lod)
		{
			this.lod.enabled = false;
			this.lod.DontSpawn = true;
			if (this.lod.CurrentLodTransform)
			{
				UnityEngine.Object.Destroy(this.lod.CurrentLodTransform.gameObject);
			}
		}
		this.SpawnCutTree();
		this.cut.GetComponent<TreeHealth>().DestroyTrunk();
		this.HideAllChunks();
		this.FinalCleanup();
	}

	private void State_Removed()
	{
		base.state.FallingTransform.SetTransforms(null);
		if (this.entity.isOwner)
		{
			return;
		}
		if (this.fall)
		{
			UnityEngine.Object.Destroy(this.fall);
			this.fall = null;
		}
		if (this.cut)
		{
			UnityEngine.Object.Destroy(this.cut);
			this.cut = null;
		}
		if (this.lod)
		{
			this.lod.enabled = false;
			this.lod.DontSpawn = true;
			if (this.lod.CurrentLodTransform)
			{
				this.lod.DespawnCurrent();
			}
		}
		this.FinalCleanup();
	}

	private void FinalCleanup()
	{
		if (!this.entity.IsOwner())
		{
			this.cut_chunks = null;
			base.state.RemoveCallback("Chunk1", this.c1);
			base.state.RemoveCallback("Chunk1", this.c2);
			base.state.RemoveCallback("Chunk1", this.c3);
			base.state.RemoveCallback("Chunk1", this.c4);
			base.state.RemoveCallback("Damaged", this.dmg);
		}
	}

	private void SpawnCutTree()
	{
		if (this.cut)
		{
			return;
		}
		this.cut = (GameObject)UnityEngine.Object.Instantiate(this.NetworkPrefab, base.transform.position, base.transform.rotation);
		TreeHealth component = this.cut.GetComponent<TreeHealth>();
		component.LodEntity = this.entity;
		component.SetLodBase(this.lod);
		this.cut_chunks = (from x in this.cut.GetComponentsInChildren<TreeCutChunk>()
		orderby int.Parse(x.transform.parent.gameObject.name)
		select x).ToArray<TreeCutChunk>();
	}

	private void State_Damaged()
	{
		FMOD.Studio.EventInstance windEvent = TreeWindSfx.BeginTransfer(this.lod.CurrentLodTransform);
		this.SpawnCutTree();
		if (this.lod.CurrentLodTransform)
		{
			UnityEngine.Object.Destroy(this.lod.CurrentLodTransform.gameObject);
			this.lod.CurrentLodTransform = null;
		}
		if (this.cut)
		{
			TreeWindSfx.CompleteTransfer(this.cut.transform, windEvent);
			this.ChunkUpdater1();
			this.ChunkUpdater2();
			this.ChunkUpdater3();
			this.ChunkUpdater4();
		}
	}

	private int GetChunk1()
	{
		return base.state.Chunk1;
	}

	private int GetChunk2()
	{
		return base.state.Chunk2;
	}

	private int GetChunk3()
	{
		return base.state.Chunk3;
	}

	private int GetChunk4()
	{
		return base.state.Chunk4;
	}

	private void HideAllChunks()
	{
		this.UpdateChunks(0, 4);
		this.UpdateChunks(1, 4);
		this.UpdateChunks(2, 4);
		this.UpdateChunks(3, 4);
	}

	private void State_Falling()
	{
		this.State_Damaged();
		this.HideAllChunks();
		if (!this.fall)
		{
			this.fall = this.cut.GetComponent<TreeHealth>().DoFallTree();
			this.fall.gameObject.AddComponent<CoopOnDestroyCallback>().Callback = new Action(this.OnDestroyCallback);
			if (BoltNetwork.isClient)
			{
				base.StartCoroutine(this.AssignFallTransformDelayed());
			}
			else
			{
				base.state.FallingTransform.SetTransforms(this.fall.transform);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator AssignFallTransformDelayed()
	{
		CoopTreeId.<AssignFallTransformDelayed>c__Iterator22 <AssignFallTransformDelayed>c__Iterator = new CoopTreeId.<AssignFallTransformDelayed>c__Iterator22();
		<AssignFallTransformDelayed>c__Iterator.<>f__this = this;
		return <AssignFallTransformDelayed>c__Iterator;
	}

	private void OnDestroyCallback()
	{
		if (this && this.entity && this.entity.isOwner)
		{
			base.state.State = 3;
		}
	}

	private void LodChanged(int newLOD)
	{
		if (newLOD == 0 && this.entity.IsAttached() && base.state.Damage > 0)
		{
			this.State_Damaged();
		}
	}

	private void ChunkUpdater1()
	{
		if (base.state.State != 1 || !this.cut || this.cut_chunks == null)
		{
			return;
		}
		this.UpdateChunks(0, this.GetChunk1());
	}

	private void ChunkUpdater2()
	{
		if (base.state.State != 1 || !this.cut || this.cut_chunks == null)
		{
			return;
		}
		this.UpdateChunks(1, this.GetChunk2());
	}

	private void ChunkUpdater3()
	{
		if (base.state.State != 1 || !this.cut || this.cut_chunks == null)
		{
			return;
		}
		this.UpdateChunks(2, this.GetChunk3());
	}

	private void ChunkUpdater4()
	{
		if (base.state.State != 1 || !this.cut || this.cut_chunks == null)
		{
			return;
		}
		this.UpdateChunks(3, this.GetChunk4());
	}

	private void UpdateChunks(int c, int value)
	{
		if (this.cut_chunks != null && c >= 0 && c < this.cut_chunks.Length && this.cut_chunks[c])
		{
			switch (value)
			{
			case 0:
				this.cut_chunks[c].Fake1.SetActive(true);
				this.cut_chunks[c].Fake2.SetActive(false);
				this.cut_chunks[c].Fake3.SetActive(false);
				this.cut_chunks[c].Fake4.SetActive(false);
				break;
			case 1:
				this.cut_chunks[c].Fake1.SetActive(false);
				this.cut_chunks[c].Fake2.SetActive(true);
				break;
			case 2:
				this.cut_chunks[c].Fake1.SetActive(false);
				this.cut_chunks[c].Fake2.SetActive(false);
				this.cut_chunks[c].Fake3.SetActive(true);
				break;
			case 3:
				this.cut_chunks[c].Fake1.SetActive(false);
				this.cut_chunks[c].Fake2.SetActive(false);
				this.cut_chunks[c].Fake3.SetActive(false);
				this.cut_chunks[c].Fake4.SetActive(true);
				break;
			case 4:
				UnityEngine.Object.Destroy(this.cut_chunks[c].transform.parent.gameObject);
				break;
			}
		}
	}
}
