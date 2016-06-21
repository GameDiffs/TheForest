using System;
using System.IO;
using UnityEngine;

namespace CetoTF
{
	public class ExportShoreMask : MonoBehaviour
	{
		public float shoreLevel = 41.5f;

		private void Start()
		{
			Terrain component = base.GetComponent<Terrain>();
			if (component == null)
			{
				Debug.Log("No Terrain Present");
				return;
			}
			TerrainData terrainData = component.terrainData;
			int heightmapWidth = terrainData.heightmapWidth;
			int heightmapWidth2 = terrainData.heightmapWidth;
			int heightmapResolution = terrainData.heightmapResolution;
			Vector3 heightmapScale = terrainData.heightmapScale;
			Debug.Log("Height map width: " + heightmapWidth);
			Debug.Log("Height map height: " + heightmapWidth2);
			Debug.Log("Height map resolution: " + heightmapResolution);
			Debug.Log("Height map scale: " + heightmapScale);
			float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapWidth2);
			Texture2D texture2D = new Texture2D(heightmapWidth, heightmapWidth2);
			for (int i = 0; i < heightmapWidth; i++)
			{
				for (int j = 0; j < heightmapWidth2; j++)
				{
					float num = heights[j, i] * heightmapScale.y + base.transform.position.y;
					float num2 = 0f;
					if (num > this.shoreLevel)
					{
						num2 = 1f;
					}
					texture2D.SetPixel(i, j, new Color(num2, num2, num2, 1f));
				}
			}
			texture2D.Apply();
			byte[] bytes = texture2D.EncodeToPNG();
			string text = Application.dataPath + "/CetoTheForest/ExportedShoreMask.png";
			File.WriteAllBytes(text, bytes);
			Debug.Log("Saved shore mask to file " + text);
		}
	}
}
