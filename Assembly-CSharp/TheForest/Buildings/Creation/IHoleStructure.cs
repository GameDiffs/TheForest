using System;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public interface IHoleStructure
	{
		void CreateStructure(bool isRepair = false);

		Hole AddSquareHole(Vector3 position, float yRotation, Vector2 size);

		void RemoveHole(Hole hole);
	}
}
