using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class PlaceGhostSfx : MonoBehaviour
	{
		private void Awake()
		{
			if (!LevelSerializer.IsDeserializing)
			{
				LocalPlayer.Sfx.PlayPlaceGhost();
			}
			UnityEngine.Object.Destroy(this);
		}
	}
}
