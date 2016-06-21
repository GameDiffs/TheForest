using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class SpawnMutantsSerializerMessageProxy : MonoBehaviour
	{
		private void OnSerializing()
		{
			for (int i = 0; i < Scene.MutantControler.allCaveSpawns.Count; i++)
			{
				GameObject gameObject = Scene.MutantControler.allCaveSpawns[i];
				if (gameObject)
				{
					gameObject.SendMessage("OnSerializing", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}
