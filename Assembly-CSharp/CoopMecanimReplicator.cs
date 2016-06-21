using Bolt;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CoopMecanimReplicator : EntityBehaviour<IWorldCharacter>
{
	private Dictionary<int, MecanimTransitionOverride> overrideLookup;

	[SerializeField]
	public Animator TargetAnimator;

	[SerializeField]
	public CoopMecanimReplicatorTransitionData TransitionData;

	[SerializeField]
	public MecanimLayerConfig[] LayersToSync;

	[SerializeField]
	public MecanimTransitionOverride[] TransitionOverrides;

	[Range(0f, 1f), SerializeField]
	public float CrossFadeMultiplier = 0.9f;

	public bool isPlayer;

	private int intoBlock = Animator.StringToHash("intoBlock");

	private void UpdateOverrideLookupTable()
	{
		this.overrideLookup = new Dictionary<int, MecanimTransitionOverride>();
		MecanimTransitionOverride[] transitionOverrides = this.TransitionOverrides;
		for (int i = 0; i < transitionOverrides.Length; i++)
		{
			MecanimTransitionOverride mecanimTransitionOverride = transitionOverrides[i];
			this.overrideLookup.Add(Animator.StringToHash(mecanimTransitionOverride.FullStateName), mecanimTransitionOverride);
		}
	}

	private void Awake()
	{
		if (BoltNetwork.isRunning)
		{
			this.UpdateOverrideLookupTable();
		}
		else
		{
			base.enabled = false;
		}
	}

	public override void Attached()
	{
		for (int i = 0; i < this.LayersToSync.Length; i++)
		{
			this.LayersToSync[i].LayerIndex = this.TargetAnimator.GetLayerIndex(this.LayersToSync[i].Name);
			if (this.LayersToSync[i].LayerIndex == -1)
			{
				Debug.LogErrorFormat("mecanim layer {0} not found on {1}", new object[]
				{
					this.LayersToSync[i].Name,
					this.TargetAnimator.gameObject.name
				});
			}
		}
		base.state.SetAnimator(this.TargetAnimator);
		if (!this.entity.IsOwner())
		{
			base.state.AddCallback("MecanimLayerHashes[]", new PropertyCallbackSimple(this.UpdateMecanimPropertiesFromState));
			base.state.AddCallback("MecanimLayerTimes[]", new PropertyCallbackSimple(this.UpdateMecanimPropertiesFromState));
		}
		if (this.TransitionData)
		{
			this.TransitionData.Init();
		}
	}

	public override void Detached()
	{
		base.StopAllCoroutines();
	}

	public void ApplyHashToRemote(int layer, int hash, float transitionTime, float normalizedTime)
	{
		MecanimLayerConfig arg_19_0 = this.LayersToSync[layer];
		this.LayersToSync[layer].Hash = hash;
		arg_19_0.Hash_Recv = hash;
		this.UpdateRemote(new float?(transitionTime), new float?(normalizedTime));
	}

	private void UpdateMecanimPropertiesFromState()
	{
		for (int i = 0; i < this.LayersToSync.Length; i++)
		{
			if (this.LayersToSync[i].Hash_Recv != base.state.MecanimLayerHashes[i])
			{
				this.LayersToSync[i].Hash_Recv = (this.LayersToSync[i].Hash = base.state.MecanimLayerHashes[i]);
			}
			this.LayersToSync[i].NormalizedTime = base.state.MecanimLayerTimes[i];
		}
	}

	private void LateUpdate()
	{
		if (this.entity.IsAttached())
		{
			if (this.entity.isOwner)
			{
				this.UpdateOwner();
			}
			else
			{
				this.UpdateRemote(null, null);
			}
		}
	}

	private void UpdateOwner()
	{
		if (this.entity.IsOwner())
		{
			base.state.MecanimSpeed = this.TargetAnimator.speed;
			for (int i = 0; i < this.LayersToSync.Length; i++)
			{
				MecanimLayerConfig mecanimLayerConfig = this.LayersToSync[i];
				AnimatorStateInfo currentAnimatorStateInfo = this.TargetAnimator.GetCurrentAnimatorStateInfo(mecanimLayerConfig.LayerIndex);
				if (currentAnimatorStateInfo.fullPathHash != mecanimLayerConfig.Hash)
				{
					base.state.MecanimLayerHashes[i] = (mecanimLayerConfig.Hash = currentAnimatorStateInfo.fullPathHash);
					base.state.MecanimLayerTimes[i] = (mecanimLayerConfig.NormalizedTime = currentAnimatorStateInfo.normalizedTime);
				}
				if (mecanimLayerConfig.SyncWeight)
				{
					float layerWeight = this.TargetAnimator.GetLayerWeight(mecanimLayerConfig.LayerIndex);
					if (layerWeight != mecanimLayerConfig.Weight)
					{
						base.state.MecanimLayerWeights[i] = (mecanimLayerConfig.Weight = layerWeight);
					}
				}
			}
		}
	}

	private void UpdateRemote(float? transitionDuration, float? stateNormalizedTime)
	{
		if (this.entity.IsAttached())
		{
			this.TargetAnimator.speed = base.state.MecanimSpeed;
			for (int i = 0; i < this.LayersToSync.Length; i++)
			{
				MecanimLayerConfig mecanimLayerConfig = this.LayersToSync[i];
				if (mecanimLayerConfig.Hash != 0)
				{
					AnimatorStateInfo currentAnimatorStateInfo = this.TargetAnimator.GetCurrentAnimatorStateInfo(mecanimLayerConfig.LayerIndex);
					if (!this.TargetAnimator.IsInTransition(mecanimLayerConfig.LayerIndex))
					{
						if (currentAnimatorStateInfo.fullPathHash != mecanimLayerConfig.Hash)
						{
							MecanimTransitionOverride mecanimTransitionOverride;
							Dictionary<int, float> dictionary;
							float value;
							if (this.overrideLookup.TryGetValue(mecanimLayerConfig.Hash, out mecanimTransitionOverride))
							{
								if (!transitionDuration.HasValue)
								{
									transitionDuration = new float?(mecanimLayerConfig.TransitionTime);
								}
								if (stateNormalizedTime == 0f)
								{
									stateNormalizedTime = new float?(mecanimTransitionOverride.TransitionOffset);
								}
							}
							else if (this.TransitionData && !transitionDuration.HasValue && this.TransitionData.Lookup != null && this.TransitionData.Lookup.TryGetValue(currentAnimatorStateInfo.fullPathHash, out dictionary) && dictionary.TryGetValue(mecanimLayerConfig.Hash, out value))
							{
								transitionDuration = new float?(value);
							}
						}
						if (transitionDuration.HasValue)
						{
							transitionDuration = new float?(transitionDuration.Value * this.CrossFadeMultiplier);
						}
						else
						{
							transitionDuration = new float?(mecanimLayerConfig.TransitionTime);
						}
						if (!stateNormalizedTime.HasValue)
						{
							stateNormalizedTime = new float?(mecanimLayerConfig.NormalizedTime);
						}
						if (this.isPlayer)
						{
							if (this.TargetAnimator.GetCurrentAnimatorStateInfo(1).tagHash != this.intoBlock)
							{
								this.TargetAnimator.CrossFade(mecanimLayerConfig.Hash, Mathf.Max(0f, transitionDuration.Value), mecanimLayerConfig.LayerIndex, stateNormalizedTime.Value);
							}
						}
						else
						{
							this.TargetAnimator.CrossFade(mecanimLayerConfig.Hash, Mathf.Max(0f, transitionDuration.Value), mecanimLayerConfig.LayerIndex, stateNormalizedTime.Value);
						}
						mecanimLayerConfig.Hash = 0;
					}
				}
				if (mecanimLayerConfig.SyncWeight && mecanimLayerConfig.Weight != base.state.MecanimLayerWeights[i])
				{
					float num = mecanimLayerConfig.Weight;
					num = Mathf.Lerp(num, base.state.MecanimLayerWeights[i], Time.deltaTime * 10f);
					this.TargetAnimator.SetLayerWeight(mecanimLayerConfig.LayerIndex, mecanimLayerConfig.Weight = num);
				}
			}
		}
	}
}
