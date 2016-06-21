using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class TreeWindSfxManager : MonoBehaviour
{
	private class TreeInfo
	{
		public TreeWindSfx tree;

		public Vector3 position;

		public float sqrDistance;

		public float direction;

		public TreeInfo(TreeWindSfx tree)
		{
			this.tree = tree;
			this.position = tree.transform.position;
			this.sqrDistance = 0f;
			this.direction = 0f;
		}
	}

	private const float TWO_PI = 6.28318548f;

	[Tooltip("The target number of active trees")]
	public int TargetActiveCount = 10;

	[Tooltip("The width used when occluding distant trees with close trees")]
	public float TreeOcclusionWidth = 10f;

	[Tooltip("The time in seconds that trees persist after going inactive before stopping their SFX")]
	public float TreePersistTime = 1f;

	[Tooltip("The time in seconds between updates")]
	public float UpdatePeriod = 0.25f;

	private static HashSet<TreeWindSfx> sTrees = new HashSet<TreeWindSfx>();

	private static List<TreeWindSfxManager.TreeInfo> sTreeInfoList = new List<TreeWindSfxManager.TreeInfo>();

	private static Texture2D activeTexture = null;

	private static Texture2D inactiveTexture = null;

	private static Texture2D playerTexture = null;

	public static void Add(TreeWindSfx tree)
	{
		if (!TreeWindSfxManager.sTrees.Contains(tree))
		{
			TreeWindSfxManager.sTrees.Add(tree);
			TreeWindSfxManager.sTreeInfoList.Add(new TreeWindSfxManager.TreeInfo(tree));
		}
	}

	public static void Remove(TreeWindSfx tree)
	{
		if (TreeWindSfxManager.sTrees.Contains(tree))
		{
			TreeWindSfxManager.sTrees.Remove(tree);
			TreeWindSfxManager.sTreeInfoList.RemoveAll((TreeWindSfxManager.TreeInfo info) => info.tree == tree);
		}
	}

	private static void Swap(List<TreeWindSfxManager.TreeInfo> trees, int a, int b)
	{
		if (a != b)
		{
			TreeWindSfxManager.TreeInfo value = trees[a];
			trees[a] = trees[b];
			trees[b] = value;
		}
	}

	private void Start()
	{
		if (!CoopPeerStarter.DedicatedHost)
		{
			base.InvokeRepeating("UpdateVirtualisation", 0f, this.UpdatePeriod);
			FMOD_Listener.DrawTreeDebug = new Action(TreeWindSfxManager.DrawDebug);
		}
		else
		{
			base.enabled = false;
		}
	}

	private static void Fill(Texture2D texture, Color color)
	{
		for (int i = 0; i < texture.width; i++)
		{
			for (int j = 0; j < texture.height; j++)
			{
				texture.SetPixel(i, j, color);
			}
		}
	}

	private static void DrawDiamond(Texture2D texture, Color color)
	{
		int num = texture.width / 2;
		int num2 = texture.height / 2;
		for (int i = 0; i < num2; i++)
		{
			int num3 = i;
			for (int j = num - num3; j < num + num3; j++)
			{
				texture.SetPixel(j, i, color);
			}
		}
		for (int k = num2; k < texture.height; k++)
		{
			int num4 = texture.height - k;
			for (int l = num - num4; l < num + num4; l++)
			{
				texture.SetPixel(l, k, color);
			}
		}
	}

	private static void CreateDebugTextures()
	{
		TreeWindSfxManager.activeTexture = new Texture2D(10, 10, TextureFormat.ARGB32, false);
		TreeWindSfxManager.Fill(TreeWindSfxManager.activeTexture, new Color(0f, 0f, 0f, 0f));
		TreeWindSfxManager.DrawDiamond(TreeWindSfxManager.activeTexture, new Color(0f, 1f, 0f, 0.5f));
		TreeWindSfxManager.activeTexture.Apply();
		TreeWindSfxManager.inactiveTexture = new Texture2D(10, 10, TextureFormat.ARGB32, false);
		TreeWindSfxManager.Fill(TreeWindSfxManager.inactiveTexture, new Color(0f, 0f, 0f, 0f));
		TreeWindSfxManager.DrawDiamond(TreeWindSfxManager.inactiveTexture, new Color(1f, 1f, 1f, 0.5f));
		TreeWindSfxManager.inactiveTexture.Apply();
		TreeWindSfxManager.playerTexture = new Texture2D(10, 10, TextureFormat.ARGB32, false);
		TreeWindSfxManager.Fill(TreeWindSfxManager.playerTexture, new Color(0f, 0f, 0f, 0f));
		TreeWindSfxManager.DrawDiamond(TreeWindSfxManager.playerTexture, new Color(1f, 1f, 1f, 1f));
		TreeWindSfxManager.playerTexture.Apply();
	}

	private static void DrawDebug()
	{
		if (TreeWindSfxManager.activeTexture == null)
		{
			TreeWindSfxManager.CreateDebugTextures();
		}
		List<TreeWindSfxManager.TreeInfo> list = TreeWindSfxManager.sTreeInfoList;
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			TreeWindSfxManager.TreeInfo treeInfo = list[i];
			TreeWindSfx tree = treeInfo.tree;
			Vector3 vector = LocalPlayer.Transform.InverseTransformPoint(tree.transform.position);
			num = Math.Max(num, vector.sqrMagnitude);
			if (tree.IsActive)
			{
				num2++;
			}
			treeInfo.sqrDistance = vector.sqrMagnitude;
			treeInfo.direction = Mathf.Atan2(vector.z, vector.x);
		}
		num = Mathf.Sqrt(num);
		Vector2 vector2 = new Vector2(110f, (float)(Camera.main.pixelHeight - 110));
		GUI.Box(new Rect(vector2.x - 105f, vector2.y - 105f - 35f, 210f, 245f), string.Format("Active trees: {0}  Total trees: {1}", num2, list.Count));
		foreach (TreeWindSfxManager.TreeInfo current in list)
		{
			float num3 = Mathf.Sqrt(current.sqrDistance) / num * 100f;
			float num4 = vector2.x + num3 * Mathf.Cos(current.direction);
			float num5 = vector2.y - num3 * Mathf.Sin(current.direction);
			Texture2D image = (!current.tree.IsActive) ? TreeWindSfxManager.inactiveTexture : TreeWindSfxManager.activeTexture;
			GUI.DrawTexture(new Rect(num4 - 5f, num5 - 5f, 10f, 10f), image);
		}
		GUI.DrawTexture(new Rect(vector2.x - 5f, vector2.y - 5f, 10f, 10f), TreeWindSfxManager.playerTexture);
	}

	private static void OccludeTrees(TreeWindSfxManager.TreeInfo occluder, float occlusionWidth, List<TreeWindSfxManager.TreeInfo> trees, int startIndex)
	{
		if (startIndex < trees.Count)
		{
			float num;
			if (occluder.sqrDistance > 0f)
			{
				num = Mathf.Atan(occlusionWidth / 2f / Mathf.Sqrt(occluder.sqrDistance));
			}
			else
			{
				num = 3.14159274f;
			}
			float num2 = occluder.direction - num;
			float num3 = occluder.direction + num;
			if (num2 < 0f)
			{
				num2 += 6.28318548f;
				num3 += 6.28318548f;
			}
			for (int i = startIndex; i < trees.Count; i++)
			{
				float num4 = trees[i].direction;
				if (num4 < num2)
				{
					num4 += 6.28318548f;
				}
				if (num2 <= num4 && num4 <= num3)
				{
					float num5 = (num4 - num2) / num;
					num5 -= 1f;
					num5 = 3f - 2f * Math.Abs(num5);
					trees[i].sqrDistance *= num5 * num5;
				}
			}
		}
	}

	private void UpdateVirtualisation()
	{
		if (TreeWindSfxManager.sTrees.Count <= this.TargetActiveCount)
		{
			foreach (TreeWindSfx current in TreeWindSfxManager.sTrees)
			{
				current.Activate();
			}
		}
		else
		{
			List<TreeWindSfxManager.TreeInfo> list = TreeWindSfxManager.sTreeInfoList;
			Vector3 position = LocalPlayer.Transform.position;
			for (int i = 0; i < list.Count; i++)
			{
				TreeWindSfxManager.TreeInfo treeInfo = list[i];
				Vector3 vector = treeInfo.position - position;
				treeInfo.sqrDistance = vector.sqrMagnitude;
				treeInfo.direction = Mathf.Atan2(vector.z, vector.x);
			}
			for (int j = 0; j < this.TargetActiveCount; j++)
			{
				int num = j;
				for (int k = j + 1; k < list.Count; k++)
				{
					if (list[k].sqrDistance < list[num].sqrDistance)
					{
						num = k;
					}
				}
				TreeWindSfxManager.Swap(list, j, num);
				list[j].tree.Activate();
				TreeWindSfxManager.OccludeTrees(list[j], this.TreeOcclusionWidth, list, j + 1);
			}
			for (int l = this.TargetActiveCount; l < list.Count; l++)
			{
				list[l].tree.Deactivate(this.TreePersistTime);
			}
		}
	}
}
