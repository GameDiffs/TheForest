using System;

[Serializable]
public class MecanimLayerConfig
{
	public string Name = "LayerName";

	public bool SyncWeight = true;

	public float TransitionTime = 0.05f;

	[NonSerialized]
	public float NormalizedTime;

	[NonSerialized]
	public float Weight;

	[NonSerialized]
	public float Time;

	[NonSerialized]
	public int Hash;

	[NonSerialized]
	public int Hash_Recv;

	[NonSerialized]
	public int LayerIndex;
}
