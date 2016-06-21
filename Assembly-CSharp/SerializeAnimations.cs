using Serialization;
using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

[ComponentSerializerFor(typeof(Animation))]
public class SerializeAnimations : IComponentSerializer
{
	public class StoredState
	{
		public byte[] data;

		public string name;

		public SaveGameManager.AssetReference asset;
	}

	public byte[] Serialize(Component component)
	{
		return UnitySerializer.Serialize((from AnimationState a in (Animation)component
		select new SerializeAnimations.StoredState
		{
			data = UnitySerializer.SerializeForDeserializeInto(a),
			name = a.name,
			asset = SaveGameManager.Instance.GetAssetId(a.clip)
		}).ToList<SerializeAnimations.StoredState>());
	}

	public void Deserialize(byte[] data, Component instance)
	{
		UnitySerializer.AddFinalAction(delegate
		{
			Animation animation = (Animation)instance;
			animation.Stop();
			Dictionary<string, AnimationState> dictionary = animation.Cast<AnimationState>().ToDictionary((AnimationState a) => a.name);
			List<SerializeAnimations.StoredState> list = UnitySerializer.Deserialize<List<SerializeAnimations.StoredState>>(data);
			foreach (SerializeAnimations.StoredState current in list)
			{
				if (current.asset != null && !dictionary.ContainsKey(current.name))
				{
					animation.AddClip(SaveGameManager.Instance.GetAsset(current.asset) as AnimationClip, current.name);
				}
				if (current.name.Contains(" - Queued Clone"))
				{
					AnimationState instance2 = animation.PlayQueued(current.name.Replace(" - Queued Clone", string.Empty));
					UnitySerializer.DeserializeInto(current.data, instance2);
				}
				else
				{
					UnitySerializer.DeserializeInto(current.data, animation[current.name]);
				}
			}
		});
	}
}
