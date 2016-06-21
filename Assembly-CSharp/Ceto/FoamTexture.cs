using System;
using UnityEngine;

namespace Ceto
{
	[Serializable]
	public class FoamTexture
	{
		public Texture tex;

		public Vector2 scale = Vector2.one;

		public float scrollSpeed = 1f;
	}
}
