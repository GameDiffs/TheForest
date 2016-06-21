using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[AddComponentMenu("Pathfinding/Seeker")]
public class Seeker : MonoBehaviour, ISerializationCallbackReceiver
{
	public enum ModifierPass
	{
		PreProcess,
		PostProcess = 2
	}

	public bool drawGizmos = true;

	public bool detailedGizmos;

	public StartEndModifier startEndModifier = new StartEndModifier();

	[HideInInspector]
	public int traversableTags = -1;

	[FormerlySerializedAs("traversableTags"), SerializeField]
	protected TagMask traversableTagsCompatibility = new TagMask(-1, -1);

	[HideInInspector]
	public int[] tagPenalties = new int[32];

	public OnPathDelegate pathCallback;

	public OnPathDelegate preProcessPath;

	public OnPathDelegate postProcessPath;

	[NonSerialized]
	private List<Vector3> lastCompletedVectorPath;

	[NonSerialized]
	private List<GraphNode> lastCompletedNodePath;

	[NonSerialized]
	protected Path path;

	[NonSerialized]
	private Path prevPath;

	private OnPathDelegate onPathDelegate;

	private OnPathDelegate onPartialPathDelegate;

	private OnPathDelegate tmpPathCallback;

	protected uint lastPathID;

	private List<IPathModifier> modifiers = new List<IPathModifier>();

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		if (this.traversableTagsCompatibility != null && this.traversableTagsCompatibility.tagsChange != -1)
		{
			this.traversableTags = this.traversableTagsCompatibility.tagsChange;
			this.traversableTagsCompatibility = new TagMask(-1, -1);
		}
	}

	public Path GetCurrentPath()
	{
		return this.path;
	}

	private void Awake()
	{
		this.onPathDelegate = new OnPathDelegate(this.OnPathComplete);
		this.onPartialPathDelegate = new OnPathDelegate(this.OnPartialPathComplete);
		this.startEndModifier.Awake(this);
	}

	public void OnDestroy()
	{
		this.ReleaseClaimedPath();
		this.startEndModifier.OnDestroy(this);
	}

	public void ReleaseClaimedPath()
	{
		if (this.prevPath != null)
		{
			this.prevPath.ReleaseSilent(this);
			this.prevPath = null;
		}
	}

	public void RegisterModifier(IPathModifier mod)
	{
		if (this.modifiers == null)
		{
			this.modifiers = new List<IPathModifier>(1);
		}
		this.modifiers.Add(mod);
	}

	public void DeregisterModifier(IPathModifier mod)
	{
		if (this.modifiers == null)
		{
			return;
		}
		this.modifiers.Remove(mod);
	}

	public void PostProcess(Path p)
	{
		this.RunModifiers(Seeker.ModifierPass.PostProcess, p);
	}

	public void RunModifiers(Seeker.ModifierPass pass, Path p)
	{
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = 0; i < this.modifiers.Count - 1; i++)
			{
				if (this.modifiers[i].Priority < this.modifiers[i + 1].Priority)
				{
					IPathModifier value = this.modifiers[i];
					this.modifiers[i] = this.modifiers[i + 1];
					this.modifiers[i + 1] = value;
					flag = true;
				}
			}
		}
		switch (pass)
		{
		case Seeker.ModifierPass.PreProcess:
			if (this.preProcessPath != null)
			{
				this.preProcessPath(p);
			}
			break;
		case Seeker.ModifierPass.PostProcess:
			if (this.postProcessPath != null)
			{
				this.postProcessPath(p);
			}
			break;
		}
		if (this.modifiers.Count == 0)
		{
			return;
		}
		ModifierData modifierData = ModifierData.All;
		IPathModifier pathModifier = this.modifiers[0];
		for (int j = 0; j < this.modifiers.Count; j++)
		{
			MonoModifier monoModifier = this.modifiers[j] as MonoModifier;
			if (!(monoModifier != null) || monoModifier.enabled)
			{
				switch (pass)
				{
				case Seeker.ModifierPass.PreProcess:
					this.modifiers[j].PreProcess(p);
					break;
				case Seeker.ModifierPass.PostProcess:
				{
					ModifierData modifierData2 = ModifierConverter.Convert(p, modifierData, this.modifiers[j].input);
					if (modifierData2 != ModifierData.None)
					{
						this.modifiers[j].Apply(p, modifierData2);
						modifierData = this.modifiers[j].output;
					}
					else
					{
						Debug.Log(string.Concat(new string[]
						{
							"Error converting ",
							(j <= 0) ? "original" : pathModifier.GetType().Name,
							"'s output to ",
							this.modifiers[j].GetType().Name,
							"'s input.\nTry rearranging the modifier priorities on the Seeker."
						}));
						modifierData = ModifierData.None;
					}
					pathModifier = this.modifiers[j];
					break;
				}
				}
				if (modifierData == ModifierData.None)
				{
					break;
				}
			}
		}
	}

	public bool IsDone()
	{
		return this.path == null || this.path.GetState() >= PathState.Returned;
	}

	private void OnPathComplete(Path p)
	{
		this.OnPathComplete(p, true, true);
	}

	private void OnPathComplete(Path p, bool runModifiers, bool sendCallbacks)
	{
		if (p != null && p != this.path && sendCallbacks)
		{
			return;
		}
		if (this == null || p == null || p != this.path)
		{
			return;
		}
		if (!this.path.error && runModifiers)
		{
			this.RunModifiers(Seeker.ModifierPass.PostProcess, this.path);
		}
		if (sendCallbacks)
		{
			p.Claim(this);
			this.lastCompletedNodePath = p.path;
			this.lastCompletedVectorPath = p.vectorPath;
			if (this.tmpPathCallback != null)
			{
				this.tmpPathCallback(p);
			}
			if (this.pathCallback != null)
			{
				this.pathCallback(p);
			}
			if (this.prevPath != null)
			{
				this.prevPath.ReleaseSilent(this);
			}
			this.prevPath = p;
			if (!this.drawGizmos)
			{
				this.ReleaseClaimedPath();
			}
		}
	}

	private void OnPartialPathComplete(Path p)
	{
		this.OnPathComplete(p, true, false);
	}

	private void OnMultiPathComplete(Path p)
	{
		this.OnPathComplete(p, false, true);
	}

	public ABPath GetNewPath(Vector3 start, Vector3 end)
	{
		return ABPath.Construct(start, end, null);
	}

	public Path StartPath(Vector3 start, Vector3 end)
	{
		return this.StartPath(start, end, null, -1);
	}

	public Path StartPath(Vector3 start, Vector3 end, OnPathDelegate callback)
	{
		return this.StartPath(start, end, callback, -1);
	}

	public Path StartPath(Vector3 start, Vector3 end, OnPathDelegate callback, int graphMask)
	{
		Path newPath = this.GetNewPath(start, end);
		return this.StartPath(newPath, callback, graphMask);
	}

	public Path StartPath(Path p, OnPathDelegate callback = null, int graphMask = -1)
	{
		p.enabledTags = this.traversableTags;
		p.tagPenalties = this.tagPenalties;
		if (this.path != null && this.path.GetState() <= PathState.Processing && this.lastPathID == (uint)this.path.pathID)
		{
			this.path.Error();
			this.path.LogError("Canceled path because a new one was requested.\nThis happens when a new path is requested from the seeker when one was already being calculated.\nFor example if a unit got a new order, you might request a new path directly instead of waiting for the now invalid path to be calculated. Which is probably what you want.\nIf you are getting this a lot, you might want to consider how you are scheduling path requests.");
		}
		this.path = p;
		Path expr_72 = this.path;
		expr_72.callback = (OnPathDelegate)Delegate.Combine(expr_72.callback, this.onPathDelegate);
		this.path.nnConstraint.graphMask = graphMask;
		this.tmpPathCallback = callback;
		this.lastPathID = (uint)this.path.pathID;
		this.RunModifiers(Seeker.ModifierPass.PreProcess, this.path);
		AstarPath.StartPath(this.path, false);
		return this.path;
	}

	public MultiTargetPath StartMultiTargetPath(Vector3 start, Vector3[] endPoints, bool pathsForAll, OnPathDelegate callback = null, int graphMask = -1)
	{
		MultiTargetPath multiTargetPath = MultiTargetPath.Construct(start, endPoints, null, null);
		multiTargetPath.pathsForAll = pathsForAll;
		return this.StartMultiTargetPath(multiTargetPath, callback, graphMask);
	}

	public MultiTargetPath StartMultiTargetPath(Vector3[] startPoints, Vector3 end, bool pathsForAll, OnPathDelegate callback = null, int graphMask = -1)
	{
		MultiTargetPath multiTargetPath = MultiTargetPath.Construct(startPoints, end, null, null);
		multiTargetPath.pathsForAll = pathsForAll;
		return this.StartMultiTargetPath(multiTargetPath, callback, graphMask);
	}

	public MultiTargetPath StartMultiTargetPath(MultiTargetPath p, OnPathDelegate callback = null, int graphMask = -1)
	{
		if (this.path != null && this.path.GetState() <= PathState.Processing && this.lastPathID == (uint)this.path.pathID)
		{
			this.path.ForceLogError("Canceled path because a new one was requested");
		}
		OnPathDelegate[] array = new OnPathDelegate[p.targetPoints.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.onPartialPathDelegate;
		}
		p.callbacks = array;
		p.callback = (OnPathDelegate)Delegate.Combine(p.callback, new OnPathDelegate(this.OnMultiPathComplete));
		p.nnConstraint.graphMask = graphMask;
		this.path = p;
		this.tmpPathCallback = callback;
		this.lastPathID = (uint)this.path.pathID;
		this.RunModifiers(Seeker.ModifierPass.PreProcess, this.path);
		AstarPath.StartPath(this.path, false);
		return p;
	}

	public void OnDrawGizmos()
	{
		if (this.lastCompletedNodePath == null || !this.drawGizmos)
		{
			return;
		}
		if (this.detailedGizmos)
		{
			Gizmos.color = new Color(0.7f, 0.5f, 0.1f, 0.5f);
			if (this.lastCompletedNodePath != null)
			{
				for (int i = 0; i < this.lastCompletedNodePath.Count - 1; i++)
				{
					Gizmos.DrawLine((Vector3)this.lastCompletedNodePath[i].position, (Vector3)this.lastCompletedNodePath[i + 1].position);
				}
			}
		}
		Gizmos.color = new Color(0f, 1f, 0f, 1f);
		if (this.lastCompletedVectorPath != null)
		{
			for (int j = 0; j < this.lastCompletedVectorPath.Count - 1; j++)
			{
				Gizmos.DrawLine(this.lastCompletedVectorPath[j], this.lastCompletedVectorPath[j + 1]);
			}
		}
	}
}
