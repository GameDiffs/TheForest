using Serialization;
using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

[ComponentSerializerFor(typeof(ParticleRenderer)), ComponentSerializerFor(typeof(Renderer)), ComponentSerializerFor(typeof(LineRenderer)), ComponentSerializerFor(typeof(TrailRenderer)), ComponentSerializerFor(typeof(Cloth)), ComponentSerializerFor(typeof(SkinnedMeshRenderer)), ComponentSerializerFor(typeof(MeshRenderer))]
public class SerializeRenderer : IComponentSerializer
{
	public class StoredInformation
	{
		public bool Enabled;

		public List<Material> materials = new List<Material>();
	}

	public static StoreMaterials Store;

	public byte[] Serialize(Component component)
	{
		byte[] result;
		using (new UnitySerializer.SerializationSplitScope())
		{
			Renderer renderer = (Renderer)component;
			SerializeRenderer.StoredInformation storedInformation = new SerializeRenderer.StoredInformation();
			storedInformation.Enabled = renderer.enabled;
			if ((SerializeRenderer.Store = renderer.GetComponent<StoreMaterials>()) != null)
			{
				storedInformation.materials = renderer.materials.ToList<Material>();
			}
			byte[] array = UnitySerializer.Serialize(storedInformation);
			SerializeRenderer.Store = null;
			result = array;
		}
		return result;
	}

	public void Deserialize(byte[] data, Component instance)
	{
		Renderer renderer = (Renderer)instance;
		renderer.enabled = false;
		UnitySerializer.AddFinalAction(delegate
		{
			SerializeRenderer.Store = renderer.GetComponent<StoreMaterials>();
			using (new UnitySerializer.SerializationSplitScope())
			{
				SerializeRenderer.StoredInformation storedInformation = UnitySerializer.Deserialize<SerializeRenderer.StoredInformation>(data);
				if (storedInformation == null)
				{
					Debug.LogError("An error occured when getting the stored information for a renderer");
					return;
				}
				renderer.enabled = storedInformation.Enabled;
				if (storedInformation.materials.Count > 0 && SerializeRenderer.Store != null)
				{
					renderer.materials = storedInformation.materials.ToArray();
				}
			}
			SerializeRenderer.Store = null;
		});
	}
}
