using System;
using TheForest.Buildings.World;
using TheForest.Utils;
using UnityEngine;

namespace TheForest
{
	public class DeathMPTut : MonoBehaviour
	{
		private void Start()
		{
			LocalPlayer.Tuts.ShowDeathMP();
			GameObject gameObject = new GameObject("LastBuiltLocation");
			gameObject.transform.localScale = new Vector3(0f, 0f, 1f);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.one;
			GUITexture gUITexture = gameObject.AddComponent<GUITexture>();
			gUITexture.texture = Resources.Load<Texture>("PlayerIcon");
			gUITexture.pixelInset = new Rect(-64f, -29f, 58f, 58f);
			Color color = gUITexture.color;
			color.a = 0.2f;
			gUITexture.color = color;
			LastBuiltLocation lastBuiltLocation = gameObject.AddComponent<LastBuiltLocation>();
			lastBuiltLocation.target = base.transform;
		}

		private void OnDestroy()
		{
			if (Scene.HudGui)
			{
				LocalPlayer.Tuts.HideDeathMP();
			}
		}

		private void OnLevelWasLoaded(int level)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
