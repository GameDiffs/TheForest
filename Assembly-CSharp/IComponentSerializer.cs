using System;
using UnityEngine;

public interface IComponentSerializer
{
	byte[] Serialize(Component component);

	void Deserialize(byte[] data, Component instance);
}
